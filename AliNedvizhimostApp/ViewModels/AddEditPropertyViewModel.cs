using AliNedvizhimostApp.Commands;
using AliNedvizhimostApp.Models;
using AliNedvizhimostApp.Services;
using System;
using System.Windows.Input;

namespace AliNedvizhimostApp.ViewModels
{
    public class AddEditPropertyViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;
        private readonly User _currentUser;
        private readonly Action _onSaveOrCancel;
        private readonly Property _propertyToEdit;

        public bool IsEditMode => _propertyToEdit != null;
        public string ViewTitle => IsEditMode ? "Редактирование объявления" : "Новое объявление";

        private string _title;
        public string Title { get => _title; set { _title = value; OnPropertyChanged(); } }

        private string _address;
        public string Address { get => _address; set { _address = value; OnPropertyChanged(); } }

        private decimal _price;
        public decimal Price { get => _price; set { _price = value; OnPropertyChanged(); } }

        private double _area;
        public double Area { get => _area; set { _area = value; OnPropertyChanged(); } }

        private int _rooms;
        public int Rooms { get => _rooms; set { _rooms = value; OnPropertyChanged(); } }

        private string _description;
        public string Description { get => _description; set { _description = value; OnPropertyChanged(); } }

        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }

        // Constructor for adding
        public AddEditPropertyViewModel(DatabaseService databaseService, User currentUser, Action onSaveOrCancel)
        {
            _databaseService = databaseService;
            _currentUser = currentUser;
            _onSaveOrCancel = onSaveOrCancel;

            SaveCommand = new RelayCommand(SaveProperty);
            CancelCommand = new RelayCommand(onSaveOrCancel);
        }

        // Constructor for editing
        public AddEditPropertyViewModel(DatabaseService databaseService, User currentUser, Property propertyToEdit, Action onSaveOrCancel)
            : this(databaseService, currentUser, onSaveOrCancel)
        {
            _propertyToEdit = propertyToEdit;
            // Populate fields
            Title = _propertyToEdit.Title;
            Address = _propertyToEdit.Address;
            Price = _propertyToEdit.Price;
            Area = _propertyToEdit.Area;
            Rooms = _propertyToEdit.Rooms;
            Description = _propertyToEdit.Description;
        }

        private void SaveProperty()
        {
            if (string.IsNullOrWhiteSpace(Title) || string.IsNullOrWhiteSpace(Address) || Price <= 0 || Area <= 0 || Rooms <= 0)
            {
                System.Windows.MessageBox.Show("Please fill in all required fields.", "Validation Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                return;
            }

            if (IsEditMode)
            {
                // Update existing property
                _propertyToEdit.Title = this.Title;
                _propertyToEdit.Address = this.Address;
                _propertyToEdit.Price = this.Price;
                _propertyToEdit.Area = this.Area;
                _propertyToEdit.Rooms = this.Rooms;
                _propertyToEdit.Description = this.Description;
                _databaseService.UpdateProperty(_propertyToEdit);
            }
            else
            {
                // Add new property
                var newProperty = new Property
                {
                    Title = this.Title,
                    Address = this.Address,
                    Price = this.Price,
                    Area = this.Area,
                    Rooms = this.Rooms,
                    Description = this.Description,
                    UserId = _currentUser.UserId,
                    Status = "Активно"
                };
                _databaseService.AddProperty(newProperty);
            }

            _onSaveOrCancel(); // Navigate back
        }
    }
}