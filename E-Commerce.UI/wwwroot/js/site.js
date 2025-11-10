
    * {
        margin: 0;
    padding: 0;
    box-sizing: border-box;
        }

    body {
        font - family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
    background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
    min-height: 100vh;
    display: flex;
    align-items: center;
    justify-content: center;
    padding: 20px;
        }

    .auth-container {
        background: rgba(255, 255, 255, 0.95);
    backdrop-filter: blur(10px);
    border-radius: 20px;
    box-shadow: 0 20px 40px rgba(0, 0, 0, 0.1);
    overflow: hidden;
    width: 100%;
    max-width: 900px;
    min-height: 600px;
    display: flex;
    position: relative;
        }

    .auth-side {
        flex: 1;
    padding: 60px 50px;
    display: flex;
    flex-direction: column;
    justify-content: center;
    transition: all 0.6s ease;
        }

    .auth-form {
        max - width: 350px;
    margin: 0 auto;
    width: 100%;
        }

    .brand-side {
        background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
    color: white;
    text-align: center;
    display: flex;
    flex-direction: column;
    justify-content: center;
    align-items: center;
    position: relative;
    overflow: hidden;
        }

    .brand-side::before {
        content: '';
    position: absolute;
    width: 200%;
    height: 200%;
    background: url('data:image/svg+xml,<svg xmlns="http://www.w3.org/2000/svg" viewBox="0 0 100 100"><circle cx="50" cy="50" r="2" fill="rgba(255,255,255,0.1)" /></svg>') repeat;
    animation: float 20s linear infinite;
        }

    @keyframes float {
        0 % { transform: translate(-50 %, -50 %) rotate(0deg); }
            100% {transform: translate(-50%, -50%) rotate(360deg); }
        }

    .brand-content {
        z - index: 2;
    position: relative;
        }

    .logo {
        font - size: 3rem;
    margin-bottom: 20px;
    opacity: 0;
    animation: fadeInUp 0.8s ease forwards;
        }

    .brand-title {
        font - size: 2.5rem;
    font-weight: 300;
    margin-bottom: 15px;
    opacity: 0;
    animation: fadeInUp 0.8s ease 0.2s forwards;
        }

    .brand-subtitle {
        font - size: 1.1rem;
    opacity: 0.9;
    line-height: 1.6;
    opacity: 0;
    animation: fadeInUp 0.8s ease 0.4s forwards;
        }

    @keyframes fadeInUp {
        from {
        opacity: 0;
    transform: translateY(30px);
            }
    to {
        opacity: 1;
    transform: translateY(0);
            }
        }

    .auth-title {
        font - size: 2rem;
    font-weight: 600;
    color: #333;
    margin-bottom: 10px;
    text-align: center;
        }

    .auth-subtitle {
        color: #666;
    text-align: center;
    margin-bottom: 40px;
    font-size: 0.95rem;
        }

    .form-group {
        margin - bottom: 25px;
    position: relative;
        }

    .form-control {
        width: 100%;
    padding: 15px 20px;
    border: 2px solid #e1e5e9;
    border-radius: 12px;
    font-size: 1rem;
    transition: all 0.3s ease;
    background: #f8f9fa;
    outline: none;
        }

    .form-control:focus {
        border - color: #667eea;
    background: white;
    box-shadow: 0 0 0 3px rgba(102, 126, 234, 0.1);
    transform: translateY(-2px);
        }

    .form-control::placeholder {
        color: #adb5bd;
        }

    .input-icon {
        position: absolute;
    right: 15px;
    top: 50%;
    transform: translateY(-50%);
    color: #adb5bd;
    transition: color 0.3s ease;
        }

    .form-control:focus + .input-icon {
        color: #667eea;
        }

    .btn {
        width: 100%;
    padding: 15px;
    border: none;
    border-radius: 12px;
    font-size: 1rem;
    font-weight: 600;
    cursor: pointer;
    transition: all 0.3s ease;
    text-transform: uppercase;
    letter-spacing: 0.5px;
    position: relative;
    overflow: hidden;
        }

    .btn-primary {
        background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
    color: white;
        }

    .btn-primary:hover {
        transform: translateY(-2px);
    box-shadow: 0 10px 25px rgba(102, 126, 234, 0.3);
        }

    .btn-primary::before {
        content: '';
    position: absolute;
    top: 0;
    left: -100%;
    width: 100%;
    height: 100%;
    background: linear-gradient(90deg, transparent, rgba(255, 255, 255, 0.2), transparent);
    transition: left 0.5s ease;
        }

    .btn-primary:hover::before {
        left: 100%;
        }

    .btn-outline {
        background: transparent;
    border: 2px solid #667eea;
    color: #667eea;
        }

    .btn-outline:hover {
        background: #667eea;
    color: white;
    transform: translateY(-2px);
        }

    .social-login {
        margin: 30px 0;
        }

    .social-divider {
        text - align: center;
    margin: 30px 0;
    position: relative;
    color: #adb5bd;
        }

    .social-divider::before {
        content: '';
    position: absolute;
    top: 50%;
    left: 0;
    right: 0;
    height: 1px;
    background: #e1e5e9;
        }

    .social-divider span {
        background: white;
    padding: 0 20px;
    position: relative;
    z-index: 1;
        }

    .social-buttons {
        display: flex;
    gap: 15px;
    margin-bottom: 20px;
        }

    .btn-social {
        flex: 1;
    padding: 12px;
    border: 2px solid #e1e5e9;
    background: white;
    color: #666;
    border-radius: 12px;
    font-weight: 500;
    transition: all 0.3s ease;
        }

    .btn-social:hover {
        border - color: #667eea;
    color: #667eea;
    transform: translateY(-2px);
        }

    .auth-links {
        text - align: center;
    margin-top: 30px;
        }

    .auth-links a {
        color: #667eea;
    text-decoration: none;
    font-weight: 500;
    transition: color 0.3s ease;
        }

    .auth-links a:hover {
        color: #764ba2;
    text-decoration: underline;
        }

    .checkbox-group {
        display: flex;
    align-items: center;
    gap: 10px;
    margin-bottom: 25px;
        }

    .checkbox {
        width: 18px;
    height: 18px;
    accent-color: #667eea;
        }

    .checkbox-label {
        font - size: 0.9rem;
    color: #666;
        }

    .view {
        display: none;
        }

    .view.active {
        display: block;
        }

    .view-switcher {
        position: absolute;
    top: 20px;
    right: 20px;
    z-index: 10;
        }

    .switch-btn {
        background: rgba(255, 255, 255, 0.2);
    border: 1px solid rgba(255, 255, 255, 0.3);
    color: white;
    padding: 8px 16px;
    margin: 0 5px;
    border-radius: 20px;
    cursor: pointer;
    transition: all 0.3s ease;
    font-size: 0.85rem;
        }

    .switch-btn.active {
        background: white;
    color: #667eea;
        }

    .switch-btn:hover {
        background: rgba(255, 255, 255, 0.3);
        }

    .switch-btn.active:hover {
        background: white;
        }

    .error-message {
        color: #dc3545;
    font-size: 0.85rem;
    margin-top: 5px;
    display: none;
        }

    .success-message {
        color: #28a745;
    font-size: 0.85rem;
    margin-top: 5px;
    display: none;
        }

    @media (max-width: 768px) {
            .auth - container {
        flex - direction: column;
    max-width: 400px;
            }

    .brand-side {
        padding: 40px 30px;
    min-height: 200px;
            }

    .auth-side {
        padding: 40px 30px;
            }

    .brand-title {
        font - size: 2rem;
            }

    .logo {
        font - size: 2rem;
            }
        }

    .loading {
        display: none;
    width: 20px;
    height: 20px;
    border: 2px solid transparent;
    border-top: 2px solid currentColor;
    border-radius: 50%;
    animation: spin 1s linear infinite;
    margin-left: 10px;
        }

    @keyframes spin {
        0 % { transform: rotate(0deg); }
            100% {transform: rotate(360deg); }
        }

    .two-factor-code {
        display: flex;
    gap: 10px;
    justify-content: center;
    margin: 20px 0;
        }

    .code-input {
        width: 50px;
    height: 50px;
    text-align: center;
    border: 2px solid #e1e5e9;
    border-radius: 8px;
    font-size: 1.5rem;
    font-weight: 600;
        }

    .code-input:focus {
        border - color: #667eea;
    outline: none;
        }
