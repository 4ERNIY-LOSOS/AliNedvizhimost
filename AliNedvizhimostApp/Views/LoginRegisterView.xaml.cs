using System.Windows.Controls;
using AliNedvizhimostApp.ViewModels;
using System.Windows;

namespace AliNedvizhimostApp.Views
{
    public partial class LoginRegisterView : UserControl
    {
        public LoginRegisterView()
        {
            InitializeComponent();
        }

        private void TabControl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is LoginRegisterViewModel viewModel)
            {
                if (viewModel.TabChangedCommand.CanExecute(null))
                {
                    viewModel.TabChangedCommand.Execute(null);
                }
            }
        }
    }
}