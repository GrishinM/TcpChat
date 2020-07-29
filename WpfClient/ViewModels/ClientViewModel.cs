using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Threading;
using WpfClient.Annotations;
using WpfClient.Commands;
using ChatLibrary;
using WpfClient.Models;

namespace WpfClient.ViewModels
{
    public sealed class ClientViewModel : INotifyPropertyChanged, IClient
    {
        private Client Client { get; }

        public ClientInfo ClientInfo => Client.ClientInfo;

        public ObservableCollection<Message> Messages => new ObservableCollection<Message>(Client.Messages);

        public event Action AuthorizationSucceeded;
        public event Action<string> AuthorizationFailed;
        public event Action RegistrationSucceeded;
        public event Action<string> RegistrationFailed;
        public event Action Message;

        private string password;

        public string Password
        {
            get => password;
            set
            {
                password = value;
                OnPropertyChanged(nameof(Password));
            }
        }

        public ClientViewModel(Client client)
        {
            Client = client;
            Client.AuthorizationSucceeded += () => Do(() => AuthorizationSucceeded());
            Client.AuthorizationFailed += message => Do(() => AuthorizationFailed(message));
            Client.RegistrationSucceeded += () => Do(() => RegistrationSucceeded());
            Client.RegistrationFailed += message => Do(() => RegistrationFailed(message));
            Client.Message += () => Do(() =>
            {
                OnPropertyChanged(nameof(Messages));
                Message();
            });
        }

        private static void Do(Action action)
        {
            Dispatcher.CurrentDispatcher.Invoke(action);
        }

        private AuthorizationCommand authorizationCommand;

        public AuthorizationCommand AuthorizationCommand => authorizationCommand ?? (authorizationCommand = new AuthorizationCommand(Client));

        private RegistrationCommand registrationCommand;

        public RegistrationCommand RegistrationCommand => registrationCommand ?? (registrationCommand = new RegistrationCommand(Client));

        private SendMessageCommand sendMessageCommand;

        public SendMessageCommand SendMessageCommand => sendMessageCommand ?? (sendMessageCommand = new SendMessageCommand(Client));

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}