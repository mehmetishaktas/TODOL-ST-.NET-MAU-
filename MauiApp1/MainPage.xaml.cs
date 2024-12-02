using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using TodoApp;

namespace MauiApp1
{
    public partial class MainPage : ContentPage
    {
        TodoViewModel view = new TodoViewModel();

        public MainPage()
        {
            InitializeComponent();
            BindingContext = view;
        }
    }
}

