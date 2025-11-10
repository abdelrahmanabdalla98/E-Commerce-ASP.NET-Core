using AutoMapper;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using E_Commerce.BLL.Mapper;
using E_Commerce.BLL.Repository;
using E_Commerce.BLL.RepositoryPattern;
using E_Commerce.BLL.Services;
using E_Commerce.DAL.Database;
using E_Commerce.DAL.Entity;
using E_Commerce.DAL.Entity_Extension;
using E_Commerce.DAL.ExtendedEntity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Serilog;
using Serilog.Sinks.MSSqlServer;
using System.Data;
using System.Threading.RateLimiting;
using YourApp.Services;

namespace E_Commerce.PL
{
    public class Program
    {
        [Obsolete]
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            #region DB_connection

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            builder.Services.AddDbContext<ApplicationContext>(options =>
                options.UseSqlServer(connectionString));

            #endregion

            #region Application Services

            builder.Services.AddScoped<IGenericRepository,GenericRepository>();
            builder.Services.AddScoped<ICartService, CartService>();
            
            //builder.Services.AddScoped<IProfileService, ProfileService>();
            builder.Services.AddScoped<IOtpService, OtpService>();
            builder.Services.AddScoped<ISmsService, SmsService>();
            builder.Services.AddScoped<IAddressService, AddressService>();

            builder.Services.AddScoped(typeof(INormalRepository<,>), typeof(NormalRepository<,>));
            // this service will null out any properties that would cause a cycle, rather than throwing an exception.
            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
                });

            #endregion

            #region AutoMapper
            builder.Services.AddAutoMapper(config => config.AddProfile(new DBMapper()));
            #endregion

            #region Logging Service
            builder.Host.UseSerilog((context, configuration) =>
            {
                configuration
                    .ReadFrom.Configuration(context.Configuration)
                    .WriteTo.Console()
                    .WriteTo.MSSqlServer(
                        connectionString: context.Configuration.GetConnectionString("DefaultConnection"),
                        tableName: "Logs",
                        autoCreateSqlTable: true,
                        columnOptions: GetColumnOptions());
            });

            #endregion
            #region this service to use for saving vars in user session for refernce 
            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30); // Optional: set timeout
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });
            #endregion

            #region this service to limit user from interact with specific view 
            builder.Services.AddRateLimiter(options =>
            {
                options.OnRejected = async (context, token) =>
                {
                    context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;

                    // Check if this is a browser request (HTML), not an API request
                    var acceptHeader = context.HttpContext.Request.Headers["Accept"].ToString();
                    bool isHtml = acceptHeader.Contains("text/html");

                    if (isHtml)
                    {
                        context.HttpContext.Response.Redirect("/Error/RateLimit");
                    }
                    else
                    {
                        // Fallback for API clients: return JSON
                        context.HttpContext.Response.ContentType = "application/json";
                        var json = System.Text.Json.JsonSerializer.Serialize(new
                        {
                            error = "Too many requests. Please try again later."
                        });
                        await context.HttpContext.Response.WriteAsync(json, token);
                    }
                };
                options.AddPolicy("AccountPolicy", context =>
                    RateLimitPartition.GetFixedWindowLimiter(
                        // Key selector
                        partitionKey: context.User.Identity?.Name
                                   ?? context.Connection.RemoteIpAddress?.ToString()
                                   ?? "anonymous",
                        // Limiter options per key
                        factory: key => new FixedWindowRateLimiterOptions
                        {
                            PermitLimit = 10,
                            Window = TimeSpan.FromMinutes(1),
                            QueueLimit = 0, // No queuing
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst
                        }));
            });

            #endregion


            builder.Services.AddIdentity<ApplicationUser, ApplicationRole>(options =>
            {
                // Password settings
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 6;
                options.Password.RequiredUniqueChars = 1;

                // Lockout settings
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;

                // User settings
                options.User.AllowedUserNameCharacters =
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true;

                // Sign-in settings
                options.SignIn.RequireConfirmedEmail = true;
                options.SignIn.RequireConfirmedPhoneNumber = false;
                //options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromSeconds(2);

            })
            .AddEntityFrameworkStores<ApplicationContext>()
            .AddDefaultTokenProviders();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                // Cookie settings
                options.Cookie.HttpOnly = true;

                options.ExpireTimeSpan = TimeSpan.FromMinutes(30);

                options.LoginPath = "/Account/Login";
                options.AccessDeniedPath = "/Account/AccessDenied";
                options.SlidingExpiration = true;

            });

            // Add email service
            builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
            builder.Services.AddTransient<IEmailService, EmailService>();

            // Add QR Code service for 2FA
            builder.Services.AddTransient<IQrCodeService, QrCodeService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSession();
            app.UseRouting();
            app.UseRateLimiter(); // must come after routing
            app.UseSerilogRequestLogging();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Base}/{action=Home}/{id?}");

            app.Run();
        }
        static ColumnOptions GetColumnOptions()
        {
            var columnOptions = new ColumnOptions();
            columnOptions.Store.Remove(StandardColumn.MessageTemplate);
            columnOptions.Store.Remove(StandardColumn.Properties);

            columnOptions.AdditionalColumns = new List<SqlColumn>
              {
                  new SqlColumn { ColumnName = "Username", DataType = SqlDbType.NVarChar, DataLength = 100 },
                  new SqlColumn { ColumnName = "IpAddress", DataType = SqlDbType.NVarChar, DataLength = 50 },
                  new SqlColumn { ColumnName = "UserAgent", DataType = SqlDbType.NVarChar, DataLength = 500 },
                  new SqlColumn { ColumnName = "RequestPath", DataType = SqlDbType.NVarChar, DataLength = 200 }
              };

            return columnOptions;
        }
    }
}
