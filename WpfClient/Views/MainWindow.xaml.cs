using System;

namespace WpfClient.Views
{
    public sealed partial class MainWindow
    {
        public MainWindow()
        {
            InitializeComponent();
            Frame.NavigationService.Navigate(new Login());
            Closing += (s, e) => Environment.Exit(0);
        }
    }
}