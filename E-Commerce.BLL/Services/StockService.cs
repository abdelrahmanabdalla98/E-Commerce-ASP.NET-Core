using E_Commerce.DAL.DB_Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.BLL.Services
{
    public class StockService<T>: IStockService<T> where T : class
    {
        private readonly ApplicationContext Contx;

        public StockService(ApplicationContext Contx)
        {
            this.Contx = Contx;
        }
    }
    public interface IStockService<T> where T : class 
    {
    }
}
