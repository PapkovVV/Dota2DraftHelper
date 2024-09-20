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
    public HeroControl(OwnPick ownPick)
    {
        InitializeComponent();
        var parent = Window.GetWindow(this);
        if (parent != null) MessageBox.Show("@");
        DataContext = new HeroViewModel(parent,ownPick);
    }
}
