﻿using Dota2DraftHelper.ViewModels;
using System.Windows;

namespace Dota2DraftHelper.Views;

/// <summary>
/// Логика взаимодействия для SettingsDialog.xaml
/// </summary>
public partial class SettingsDialog : Window
{
    public SettingsDialog()
    {
        InitializeComponent();
        DataContext = new SettingsViewModel();
    }

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        DialogResult = true;
    }
}
