using AliNedvizhimostApp.Commands;
using AliNedvizhimostApp.Models;
using AliNedvizhimostApp.Services;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace AliNedvizhimostApp.ViewModels
{
    public class ProfileViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;
        private readonly User _currentUser;

        public User CurrentUser => _currentUser;

        private ObservableCollection<Property> _activeProperties;
        public ObservableCollection<Property> ActiveProperties
        {
            get => _activeProperties;
            set { _activeProperties = value; OnPropertyChanged(); }
        }

        private ObservableCollection<Property> _closedProperties;
        public ObservableCollection<Property> ClosedProperties
        {
            get => _closedProperties;
            set { _closedProperties = value; OnPropertyChanged(); }
        }

        private readonly ApplicationViewModel _appViewModel;

        public ICommand EditPropertyCommand { get; }
        public ICommand ClosePropertyCommand { get; }
        public ICommand GoBackCommand { get; }

        public ProfileViewModel(DatabaseService databaseService, User currentUser, ApplicationViewModel appViewModel, Action goBack)
        {
            _databaseService = databaseService;
            _currentUser = currentUser;
            _appViewModel = appViewModel;

            GoBackCommand = new RelayCommand(goBack);
            ClosePropertyCommand = new RelayCommand<Property>(CloseProperty);
            EditPropertyCommand = new RelayCommand<Property>(p => _appViewModel.GoToEditProperty(p));

            LoadUserProperties();
        }

        private void CloseProperty(Property property)
        {
            if (property != null)
            {
                _databaseService.UpdatePropertyStatus(property.Id, "Закрыто");
                LoadUserProperties(); // Refresh the lists
            }
        }

        private void LoadUserProperties()
        {
            var userProperties = _databaseService.GetPropertiesByUserId(_currentUser.UserId);
            
            ActiveProperties = new ObservableCollection<Property>(userProperties.Where(p => p.Status == "Активно"));
            ClosedProperties = new ObservableCollection<Property>(userProperties.Where(p => p.Status == "Закрыто"));
        }
    }
}