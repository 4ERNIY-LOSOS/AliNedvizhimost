using AliNedvizhimostApp.Commands;
using AliNedvizhimostApp.Models;
using AliNedvizhimostApp.Services;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using System.Windows; // For MessageBox

namespace AliNedvizhimostApp.ViewModels
{
    public class AdminViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;
        private readonly ApplicationViewModel _appViewModel;

        // User Management
        private ObservableCollection<User> _users;
        public ObservableCollection<User> Users
        {
            get => _users;
            set { _users = value; OnPropertyChanged(); }
        }

        // Property Management
        private ObservableCollection<Property> _properties;
        public ObservableCollection<Property> Properties
        {
            get => _properties;
            set { _properties = value; OnPropertyChanged(); }
        }

        public ICommand GoBackCommand { get; }
        public ICommand EditPropertyCommand { get; }
        public ICommand EditUserCommand { get; }
        public ICommand DeleteUserCommand { get; }
        public ICommand DeletePropertyCommand { get; }

        public AdminViewModel(DatabaseService databaseService, ApplicationViewModel appViewModel, Action goBack)
        {
            _databaseService = databaseService;
            _appViewModel = appViewModel;
            GoBackCommand = new RelayCommand(goBack);
            EditPropertyCommand = new RelayCommand<Property>(p => _appViewModel.GoToEditProperty(p));
            EditUserCommand = new RelayCommand<User>(u => _appViewModel.GoToEditUser(u));
            DeleteUserCommand = new RelayCommand<User>(DeleteUser);
            DeletePropertyCommand = new RelayCommand<Property>(DeleteProperty);

            LoadData();
        }

        private void DeleteUser(User user)
        {
            if (user == null) return;
            var result = MessageBox.Show($"Вы уверены, что хотите удалить пользователя {user.Email}?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                _databaseService.DeleteUser(user.UserId);
                LoadData(); // Refresh data
            }
        }

        private void DeleteProperty(Property property)
        {
            if (property == null) return;
            var result = MessageBox.Show($"Выверены, что хотите удалить объявление '{property.Title}'?", "Подтверждение удаления", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (result == MessageBoxResult.Yes)
            {
                _databaseService.DeleteProperty(property.Id);
                LoadData(); // Refresh data
            }
        }

        private void LoadData()
        {
            try
            {
                // Load users
                var userList = _databaseService.GetUsers();
                Users = new ObservableCollection<User>(userList);

                // Load properties
                var propertyList = _databaseService.GetProperties();
                Properties = new ObservableCollection<Property>(propertyList);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"An error occurred while loading admin data: {ex.ToString()}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}