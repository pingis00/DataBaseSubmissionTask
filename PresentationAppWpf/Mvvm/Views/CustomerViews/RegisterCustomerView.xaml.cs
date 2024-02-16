using PresentationAppWpf.Mvvm.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace PresentationAppWpf.Mvvm.Views.CustomerViews;

public partial class RegisterCustomerView : UserControl
{
    public RegisterCustomerView()
    {
        InitializeComponent();

    }


    private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is RegisterCustomerViewModel viewModel)
        {
            viewModel.Password = ((PasswordBox)sender).Password;
        }
    }

    private void ConfirmPasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
    {
        if (DataContext is RegisterCustomerViewModel viewModel)
        {
            viewModel.ConfirmPassword = ((PasswordBox)sender).Password;
        }
    }
}
