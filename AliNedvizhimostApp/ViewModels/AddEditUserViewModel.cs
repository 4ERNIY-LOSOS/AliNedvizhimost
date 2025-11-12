using AliNedvizhimostApp.Commands;
using AliNedvizhimostApp.Models;
using AliNedvizhimostApp.Services;
using System;
using System.Windows.Input;

namespace AliNedvizhimostApp.ViewModels
{
    public class AddEditUserViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;
        private readonly Action _onSaveOrCancel;
        private readonly User _userToEdit;

        public string ViewTitle => "Редактирование пользователя";

        private string _firstName;
        public string FirstName { get => _firstName; set { _firstName = value; OnPropertyChanged(); } }

        private string _lastName;
        public string LastName { get => _lastName; set { _lastName = value; OnPropertyChanged(); } }

        private string _email;
        public string Email { get => _email; set { _email = value; OnPropertyChanged(); } }

        private string _role;
        public string Role { get => _role; set { _role = value; OnPropertyChanged(); } }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        public AddEditUserViewModel(DatabaseService databaseService, User userToEdit, Action onSaveOrCancel)
        {
            _databaseService = databaseService;
            _userToEdit = userToEdit;
            _onSaveOrCancel = onSaveOrCancel;

            // Populate fields
            FirstName = _userToEdit.FirstName;
            LastName = _userToEdit.LastName;
            Email = _userToEdit.Email;
            Role = _userToEdit.Role;

            SaveCommand = new RelayCommand(SaveUser);
            CancelCommand = new RelayCommand(onSaveOrCancel);
        }

        private void SaveUser()
        {
            // Update user object
            _userToEdit.FirstName = this.FirstName;
            _userToEdit.LastName = this.LastName;
            _userToEdit.Email = this.Email;
            _userToEdit.Role = this.Role;

            _databaseService.UpdateUser(_userToEdit);

            _onSaveOrCancel(); // Navigate back
        }
    }
}