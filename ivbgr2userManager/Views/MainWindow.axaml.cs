using Avalonia.Controls;
using ivbgr2userManager.ViewModels;

namespace ivbgr2userManager.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        DataContext = new UserViewModel();
    }
}