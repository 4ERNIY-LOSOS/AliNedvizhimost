using System.Collections.Generic;
using System.Collections.ObjectModel;
using AliNedvizhimostApp.Models;
using AliNedvizhimostApp.Services;
using System.Windows.Input;
using AliNedvizhimostApp.Commands;
using System.Linq;
using System;
using System.Threading;
using System.Windows;

namespace AliNedvizhimostApp.ViewModels
{
    public class PropertiesViewModel : BaseViewModel
    {
        private readonly DatabaseService _databaseService;
        private readonly ApplicationViewModel _appViewModel;
        private Timer _debounceTimer;

        private List<Property> _allProperties; // Master list
        private ObservableCollection<Property> _properties; // Displayed list
        public ObservableCollection<Property> Properties
        {
            get => _properties;
            set { _properties = value; OnPropertyChanged(nameof(Properties)); }
        }

        // Filter Properties
        private string _searchText;
        public string SearchText
        {
            get => _searchText;
            set { SetProperty(ref _searchText, value); TriggerDebouncedFilter(); }
        }

        private string _minPrice;
        public string MinPrice
        {
            get => _minPrice;
            set { SetProperty(ref _minPrice, value); TriggerDebouncedFilter(); }
        }

        private string _maxPrice;
        public string MaxPrice
        {
            get => _maxPrice;
            set { SetProperty(ref _maxPrice, value); TriggerDebouncedFilter(); }
        }

        private string _minArea;
        public string MinArea
        {
            get => _minArea;
            set { SetProperty(ref _minArea, value); TriggerDebouncedFilter(); }
        }

        private string _maxArea;
        public string MaxArea
        {
            get => _maxArea;
            set { SetProperty(ref _maxArea, value); TriggerDebouncedFilter(); }
        }

        private string _minRooms;
        public string MinRooms
        {
            get => _minRooms;
            set { SetProperty(ref _minRooms, value); TriggerDebouncedFilter(); }
        }

        // Sorting Properties
        public ObservableCollection<string> SortOptions { get; set; }
        private string _selectedSortOption;
        public string SelectedSortOption
        {
            get => _selectedSortOption;
            set { SetProperty(ref _selectedSortOption, value); TriggerDebouncedFilter(); }
        }

        public ICommand SelectPropertyCommand { get; }

        public PropertiesViewModel(DatabaseService databaseService, ApplicationViewModel appViewModel)
        {
            _databaseService = databaseService;
            _appViewModel = appViewModel;
            
            _allProperties = new List<Property>();
            Properties = new ObservableCollection<Property>();

            SortOptions = new ObservableCollection<string> { "Сначала дешевле", "Сначала дороже" };
            _selectedSortOption = SortOptions.First();

            SelectPropertyCommand = new RelayCommand<Property>(p => _appViewModel.GoToPropertyDetails(p));
            
            _debounceTimer = new Timer(ApplyFilters, null, Timeout.Infinite, Timeout.Infinite);

            LoadProperties();
        }

        private void TriggerDebouncedFilter()
        {
            // Restart the timer to apply filters after 300ms of inactivity
            _debounceTimer.Change(300, Timeout.Infinite);
        }

        public void LoadProperties()
        {
            var allDbProperties = _databaseService.GetProperties();
            _allProperties = allDbProperties.Where(p => p.Status == "Активно").ToList();
            ApplyFilters(null); // Apply initial filters immediately
        }

        private void ApplyFilters(object state)
        {
            IEnumerable<Property> filteredProperties = _allProperties;

            // Keyword Search
            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                filteredProperties = filteredProperties.Where(p =>
                    (p.Title?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (p.Address?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (p.Description?.Contains(SearchText, StringComparison.OrdinalIgnoreCase) ?? false));
            }

            // Price Filter
            if (decimal.TryParse(MinPrice, out decimal minPrice))
                filteredProperties = filteredProperties.Where(p => p.Price >= minPrice);
            if (decimal.TryParse(MaxPrice, out decimal maxPrice) && maxPrice > 0)
                filteredProperties = filteredProperties.Where(p => p.Price <= maxPrice);

            // Area Filter
            if (double.TryParse(MinArea, out double minArea))
                filteredProperties = filteredProperties.Where(p => p.Area >= minArea);
            if (double.TryParse(MaxArea, out double maxArea) && maxArea > 0)
                filteredProperties = filteredProperties.Where(p => p.Area <= maxArea);

            // Rooms Filter
            if (int.TryParse(MinRooms, out int minRooms) && minRooms > 0)
                filteredProperties = filteredProperties.Where(p => p.Rooms >= minRooms);

            // Sorting
            if (SelectedSortOption == "Сначала дешевле")
                filteredProperties = filteredProperties.OrderBy(p => p.Price);
            else if (SelectedSortOption == "Сначала дороже")
                filteredProperties = filteredProperties.OrderByDescending(p => p.Price);

            // The timer callback runs on a background thread.
            // We must update the ObservableCollection on the UI thread.
            Application.Current.Dispatcher.Invoke(() =>
            {
                Properties = new ObservableCollection<Property>(filteredProperties.ToList());
            });
        }
    }
}