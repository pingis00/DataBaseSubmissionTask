﻿using PresentationAppWpf.Mvvm.ViewModels;
using System.Windows;

namespace PresentationAppWpf.Mvvm.Views;

public partial class MainWindow : Window
{
    public MainWindow(MainViewModel viewModel)
    {
        InitializeComponent();
        DataContext = viewModel;
    }
}