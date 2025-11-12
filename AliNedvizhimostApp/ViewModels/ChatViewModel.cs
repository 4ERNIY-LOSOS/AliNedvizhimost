using AliNedvizhimostApp.Commands;
using AliNedvizhimostApp.Models;
using AliNedvizhimostApp.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Windows; // For MessageBox

namespace AliNedvizhimostApp.ViewModels
{
    public class ChatViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;
        private readonly User _currentUser;
        private readonly Conversation _conversation;
        private readonly Action _goBack;

        public Conversation Conversation => _conversation;

        private ObservableCollection<Message> _messages;
        public ObservableCollection<Message> Messages
        {
            get => _messages;
            set { _messages = value; OnPropertyChanged(); }
        }

        private string _newMessageContent;
        public string NewMessageContent
        {
            get => _newMessageContent;
            set { _newMessageContent = value; OnPropertyChanged(); }
        }

        public ICommand SendNewMessageCommand { get; }
        public ICommand GoBackCommand { get; }

        public ChatViewModel(DatabaseService databaseService, User currentUser, Conversation conversation, Action goBack)
        {
            _databaseService = databaseService;
            _currentUser = currentUser;
            _conversation = conversation;
            _goBack = goBack;

            SendNewMessageCommand = new RelayCommand(SendNewMessage, CanSendNewMessage);
            GoBackCommand = new RelayCommand(_goBack);

            LoadMessages();
        }

        private bool CanSendNewMessage()
        {
            return !string.IsNullOrWhiteSpace(NewMessageContent);
        }

        private void SendNewMessage()
        {
            if (string.IsNullOrWhiteSpace(NewMessageContent))
            {
                MessageBox.Show("Сообщение не может быть пустым.", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                var message = new Message
                {
                    SenderId = _currentUser.UserId,
                    ReceiverId = _conversation.OtherUserId,
                    PropertyId = _conversation.PropertyId,
                    Content = NewMessageContent,
                    Timestamp = DateTime.Now
                };

                _databaseService.SendMessage(message);
                NewMessageContent = string.Empty; // Clear input
                LoadMessages(); // Refresh messages
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка при отправке сообщения: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void LoadMessages()
        {
            try
            {
                var messagesList = _databaseService.GetMessages(_currentUser.UserId, _conversation.OtherUserId, _conversation.PropertyId);
                Messages = new ObservableCollection<Message>(messagesList);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Ошибка загрузки сообщений: {ex.Message}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
