using AliNedvizhimostApp.Models;
using AliNedvizhimostApp.Services;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

using AliNedvizhimostApp.Commands;
using System.Windows.Input;
using System;

namespace AliNedvizhimostApp.ViewModels
{
    public class ApplicationViewModel : INotifyPropertyChanged
    {
        private readonly DatabaseService _databaseService;
        private PropertiesViewModel _propertiesViewModel;

        private User _currentUser;
        public User CurrentUser
        {
            get => _currentUser;
            private set
            {
                _currentUser = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(IsAdmin));
            }
        }

        public bool IsAdmin => CurrentUser?.Role == "Admin";

        private object _currentViewModel;
        public object CurrentViewModel
        {
            get => _currentViewModel;
            set
            {
                _currentViewModel = value;
                OnPropertyChanged();
            }
        }

        public ICommand LogoutCommand { get; }
        public ICommand GoToAddPropertyCommand { get; }
        public ICommand GoToProfileCommand { get; }
        public ICommand GoToAdminPanelCommand { get; }
        public ICommand GoToMessagesCommand { get; }

        public ApplicationViewModel(DatabaseService databaseService)
        {
            _databaseService = databaseService;
            LogoutCommand = new RelayCommand(Logout);
            GoToAddPropertyCommand = new RelayCommand(GoToAddProperty, () => CurrentUser != null);
            GoToProfileCommand = new RelayCommand(GoToProfile, () => CurrentUser != null);
            GoToAdminPanelCommand = new RelayCommand(GoToAdminPanel, () => IsAdmin);
            GoToMessagesCommand = new RelayCommand(GoToMessages, () => CurrentUser != null);
            CurrentViewModel = new LoginRegisterViewModel(_databaseService, OnLoginSuccess);
        }

        private void OnLoginSuccess(User user)
        {
            CurrentUser = user;
            CommandManager.InvalidateRequerySuggested();

            if (_propertiesViewModel == null)
            {
                _propertiesViewModel = new PropertiesViewModel(_databaseService, this);
            }
            _propertiesViewModel.LoadProperties();
            CurrentViewModel = _propertiesViewModel;
        }

        public void GoToPropertyDetails(Property property)
        {
            if (property != null)
            {
                CurrentViewModel = new PropertyDetailsViewModel(property, GoToProperties, this);
            }
        }

        public void GoToProperties()
        {
            if (_propertiesViewModel != null)
            {
                _propertiesViewModel.LoadProperties();
            }
            CurrentViewModel = _propertiesViewModel;
        }

        private void GoToAddProperty()
        {
            CurrentViewModel = new AddEditPropertyViewModel(_databaseService, CurrentUser, GoToProperties);
        }

        public void GoToEditProperty(Property property)
        {
            CurrentViewModel = new AddEditPropertyViewModel(_databaseService, CurrentUser, property, GoToProfile);
        }

        public void GoToEditUser(User user)
        {
            CurrentViewModel = new AddEditUserViewModel(_databaseService, user, GoToAdminPanel);
        }

        private void GoToProfile()
        {
            CurrentViewModel = new ProfileViewModel(_databaseService, CurrentUser, this, GoToProperties);
        }

        private void GoToAdminPanel()
        {
            CurrentViewModel = new AdminViewModel(_databaseService, this, GoToProperties);
        }

        public void GoToSendMessage(Property property)
        {
            if (CurrentUser != null && property != null)
            {
                CurrentViewModel = new SendMessageViewModel(_databaseService, CurrentUser, property, GoToPropertyDetails);
            }
        }

        private void GoToMessages()
        {
            if (CurrentUser != null)
            {
                CurrentViewModel = new ConversationsViewModel(_databaseService, this, CurrentUser, GoToProperties);
            }
        }

        public void GoToChat(Conversation conversation)
        {
            if (CurrentUser != null && conversation != null)
            {
                CurrentViewModel = new ChatViewModel(_databaseService, CurrentUser, conversation, GoToMessages);
            }
        }

        private void Logout()
        {
            CurrentUser = null;
            _propertiesViewModel = null;
            CommandManager.InvalidateRequerySuggested();
            CurrentViewModel = new LoginRegisterViewModel(_databaseService, OnLoginSuccess);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
