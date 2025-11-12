using AliNedvizhimostApp.Commands;
using AliNedvizhimostApp.Models;
using AliNedvizhimostApp.Services;
using System;
using System.Windows.Input;
using System.Windows; // For MessageBox, will replace with ErrorMessage later

namespace AliNedvizhimostApp.ViewModels
{
    public class SendMessageViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;
        private readonly User _currentUser;
        private readonly Property _property;
        private readonly Action<Property> _onMessageSent; // Callback to go back to property details

        public Property Property => _property;

        private string _messageContent;
        public string MessageContent
        {
            get => _messageContent;
            set { _messageContent = value; OnPropertyChanged(); }
        }

        private string _errorMessage;
        public string ErrorMessage
        {
            get => _errorMessage;
            set { _errorMessage = value; OnPropertyChanged(); }
        }

        private string _infoMessage;
        public string InfoMessage
        {
            get => _infoMessage;
            set { _infoMessage = value; OnPropertyChanged(); }
        }

        public ICommand SendMessageCommand { get; }
        public ICommand CancelCommand { get; }

        public SendMessageViewModel(DatabaseService databaseService, User currentUser, Property property, Action<Property> onMessageSent)
        {
            _databaseService = databaseService;
            _currentUser = currentUser;
            _property = property;
            _onMessageSent = onMessageSent;

            SendMessageCommand = new RelayCommand(SendMessage, CanSendMessage);
            CancelCommand = new RelayCommand(Cancel);
        }

        private bool CanSendMessage()
        {
            return !string.IsNullOrWhiteSpace(MessageContent);
        }

        private void SendMessage()
        {
            ErrorMessage = string.Empty;
            InfoMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(MessageContent))
            {
                ErrorMessage = "Сообщение не может быть пустым.";
                return;
            }

            try
            {
                var message = new Message
                {
                    SenderId = _currentUser.UserId,
                    ReceiverId = _property.UserId,
                    PropertyId = _property.Id,
                    Content = MessageContent,
                    Timestamp = DateTime.Now
                };

                _databaseService.SendMessage(message);
                InfoMessage = "Сообщение успешно отправлено!";
                MessageContent = string.Empty; // Clear message box
                _onMessageSent?.Invoke(_property); // Go back to property details
            }
            catch (Exception ex)
            {
                ErrorMessage = $"Ошибка при отправке сообщения: {ex.Message}";
            }
        }

        private void Cancel()
        {
            _onMessageSent?.Invoke(_property); // Go back to property details without sending
        }
    }
}
