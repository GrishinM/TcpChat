using System;
using System.Windows;
using WpfClient.Models;
using WpfClient.ViewModels;

namespace WpfClient.Views
{
    public partial class Login
    {
        private ClientViewModel Client { get; }

        public Login()
        {
            InitializeComponent();
            Client = new ClientViewModel(new Client());
            Client.AuthorizationSucceeded += () => Do(() => NavigationService.Navigate(new MainPage(Client)));
            Client.AuthorizationFailed += message => Do(() => MessageBox.Show(message));
            Client.RegistrationSucceeded += () => Do(() =>
            {
                MessageBox.Show("Успешная регистрация");
                Client.AuthorizationCommand.Execute((LoginBox.Text, PasswordBox.Password));
            });
            Client.RegistrationFailed += message => Do(() => MessageBox.Show(message));
            DataContext = Client;
            LoginBox.Focus();
        }

        private void Do(Action action)
        {
            Dispatcher.Invoke(action);
        }

        private void PasswordChanged(object sender, RoutedEventArgs e)
        {
            Client.Password = PasswordBox.Password;
        }
    }
}