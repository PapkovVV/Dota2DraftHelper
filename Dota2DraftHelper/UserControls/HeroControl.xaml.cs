using Dota2DraftHelper.Models;
using Dota2DraftHelper.ViewModels;
using System.Windows;
using System.Windows.Controls;

namespace Dota2DraftHelper.UserControls;

/// <summary>
/// Логика взаимодействия для HeroControl.xaml
/// </summary>
public partial class HeroControl : UserControl
{
    private OwnPick ownPick = null!;
    public HeroControl(OwnPick ownPick)
    {
        InitializeComponent();
        this.ownPick = ownPick;
    }

    private void UserControl_Loaded(object sender, RoutedEventArgs e)
    {
        var parent = Window.GetWindow(this);
        if (parent != null && parent.DataContext is MainWindowViewModel parentViewModel)
        {
            DataContext = new HeroViewModel(parentViewModel, ownPick);
        }
        else
        {
            MessageBox.Show("Не удалось получить ViewModel");
        }
    }
}
