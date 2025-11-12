using AliNedvizhimostApp.Models;
using AliNedvizhimostApp.Commands;
using System;
using System.Windows.Input;

namespace AliNedvizhimostApp.ViewModels
{
    public class PropertyDetailsViewModel : BaseViewModel
    {
        private readonly ApplicationViewModel _appViewModel;

        private Property _selectedProperty;
        public Property SelectedProperty
        {
            get => _selectedProperty;
            set
            {
                _selectedProperty = value;
                OnPropertyChanged(nameof(SelectedProperty));
            }
        }

        public ICommand GoBackCommand { get; }
        public ICommand ContactSellerCommand { get; }

        public PropertyDetailsViewModel(Property selectedProperty, Action goBack, ApplicationViewModel appViewModel)
        {
            SelectedProperty = selectedProperty;
            GoBackCommand = new RelayCommand(goBack);
            _appViewModel = appViewModel;
            ContactSellerCommand = new RelayCommand(ContactSeller, () => _appViewModel.CurrentUser != null && _appViewModel.CurrentUser.UserId != SelectedProperty.UserId);
        }

        private void ContactSeller()
        {
            _appViewModel.GoToSendMessage(SelectedProperty);
        }
    }
}