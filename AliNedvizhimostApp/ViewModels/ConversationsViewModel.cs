using AliNedvizhimostApp.Commands;
using AliNedvizhimostApp.Models;
using AliNedvizhimostApp.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace AliNedvizhimostApp.ViewModels
{
    public class ConversationsViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;
        private readonly ApplicationViewModel _appViewModel;
        private readonly User _currentUser;

        private ObservableCollection<Conversation> _conversations;
        public ObservableCollection<Conversation> Conversations
        {
            get => _conversations;
            set { _conversations = value; OnPropertyChanged(); }
        }

        public ICommand OpenConversationCommand { get; }
        public ICommand GoBackCommand { get; }

        public ConversationsViewModel(DatabaseService databaseService, ApplicationViewModel appViewModel, User currentUser, Action goBack)
        {
            _databaseService = databaseService;
            _appViewModel = appViewModel;
            _currentUser = currentUser;

            OpenConversationCommand = new RelayCommand<Conversation>(OpenConversation);
            GoBackCommand = new RelayCommand(goBack);

            LoadConversations();
        }

        private void OpenConversation(Conversation conversation)
        {
            if (conversation != null)
            {
                _appViewModel.GoToChat(conversation);
            }
        }

        public void LoadConversations()
        {
            try
            {
                var conversationsList = _databaseService.GetConversations(_currentUser.UserId);
                Conversations = new ObservableCollection<Conversation>(conversationsList);
            }
            catch (Exception ex)
            {
                // Handle error, e.g., display a message
                System.Windows.MessageBox.Show($"Ошибка загрузки чатов: {ex.Message}", "Ошибка", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }
    }
}
