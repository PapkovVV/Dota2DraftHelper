using Dota2DraftHelper.ViewModels;
using System.Windows;

namespace Dota2DraftHelper.Views;

/// <summary>
/// Логика взаимодействия для AddHeroDialog.xaml
/// </summary>
public partial class AddHeroDialog : Window
{
    public AddHeroDialog()
    {
        InitializeComponent();
        DataContext = new AddHeroDialogViewModel(this);
    }
}
