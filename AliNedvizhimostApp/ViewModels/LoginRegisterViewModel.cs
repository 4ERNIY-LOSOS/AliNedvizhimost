using AliNedvizhimostApp.Services;
using AliNedvizhimostApp.Models;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using System;
using AliNedvizhimostApp.Commands;

namespace AliNedvizhimostApp.ViewModels
{
    public class LoginRegisterViewModel : INotifyPropertyChanged
    {
        private readonly DatabaseService _databaseService;

        // Properties for Login
        private string _loginEmail;
        public string LoginEmail
        {
            get => _loginEmail;
            set
            {
                if (SetProperty(ref _loginEmail, value))
                {
                    ClearMessages();
                }
            }
        }

        private string _loginPassword;
        public string LoginPassword
        {
            get => _loginPassword;
            set => SetProperty(ref _loginPassword, value);
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set => SetProperty(ref _errorMessage, value);
        }

        private string _infoMessage;
        public string InfoMessage
        {
            get => _infoMessage;
            set => SetProperty(ref _infoMessage, value);
        }

        private bool _isLoading;
        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        // Properties for Registration
        private string _registerEmail;
        public string RegisterEmail
        {
            get => _registerEmail;
            set
            {
                if (SetProperty(ref _registerEmail, value))
                {
                    ClearMessages();
                }
            }
        }

        private string _registerPassword;
        public string RegisterPassword
        {
            get => _registerPassword;
            set => SetProperty(ref _registerPassword, value);
        }

        private string _registerFirstName;
        public string RegisterFirstName
        {
            get => _registerFirstName;
            set
            {
                if (SetProperty(ref _registerFirstName, value))
                {
                    ClearMessages();
                }
            }
        }

        private string _registerLastName;
        public string RegisterLastName
        {
            get => _registerLastName;
            set
            {
                if (SetProperty(ref _registerLastName, value))
                {
                    ClearMessages();
                }
            }
        }

        public ICommand LoginCommand { get; }
        public ICommand RegisterCommand { get; }
        public ICommand TabChangedCommand { get; }

        private readonly Action<User> _onLoginSuccess;

        public LoginRegisterViewModel(DatabaseService databaseService, Action<User> onLoginSuccess)
        {
            _databaseService = databaseService;
            _onLoginSuccess = onLoginSuccess;
            LoginCommand = new RelayCommand(Login);
            RegisterCommand = new RelayCommand(Register);
            TabChangedCommand = new RelayCommand(ClearMessages);
        }

        private void ClearMessages()
        {
            ErrorMessage = string.Empty;
            InfoMessage = string.Empty;
        }

        public void Login()
        {
            ClearMessages();
            IsLoading = true;

            if (string.IsNullOrWhiteSpace(LoginEmail) || string.IsNullOrWhiteSpace(LoginPassword))
            {
                ErrorMessage = "Пожалуйста, введите Email и пароль для входа.";
                IsLoading = false;
                return;
            }

            try
            {
                User user = _databaseService.LoginUser(LoginEmail.Trim(), LoginPassword);
                if (user != null)
                {
                    _onLoginSuccess?.Invoke(user);
                }
                else
                {
                    ErrorMessage = "Неверный Email или пароль.";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Произошла ошибка при входе: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }

        private void Register()
        {
            ClearMessages();
            IsLoading = true;

            if (string.IsNullOrWhiteSpace(RegisterEmail) || string.IsNullOrWhiteSpace(RegisterPassword) ||
                string.IsNullOrWhiteSpace(RegisterFirstName) || string.IsNullOrWhiteSpace(RegisterLastName))
            {
                ErrorMessage = "Пожалуйста, заполните все поля для регистрации.";
                IsLoading = false;
                return;
            }

            try
            {
                User newUser = _databaseService.RegisterUser(RegisterEmail.Trim(), RegisterPassword, RegisterFirstName.Trim(), RegisterLastName.Trim(), "Customer");
                var successMessage = $"Пользователь {newUser.Email} успешно зарегистрирован! Теперь вы можете войти.";
                
                // Очистка полей после успешной регистрации
                RegisterEmail = string.Empty;
                RegisterFirstName = string.Empty;
                RegisterLastName = string.Empty;
                RegisterPassword = string.Empty;

                InfoMessage = successMessage;
            }
            catch (InvalidOperationException ex)
            {
                ErrorMessage = ex.Message;
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Произошла непредвиденная ошибка при регистрации: {ex.Message}";
            }
            finally
            {
                IsLoading = false;
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        protected bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = null)
        {
            if (Equals(storage, value)) return false;
            storage = value;
            OnPropertyChanged(propertyName);
            return true;
        }
    }
}