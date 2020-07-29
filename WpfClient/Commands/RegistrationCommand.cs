using System;
using System.Windows.Input;
using WpfClient.Models;

namespace WpfClient.Commands
{
    public class RegistrationCommand : Command
    {
        public RegistrationCommand(Client client) : base(client)
        {
        }

        public override bool CanExecute(object parameter)
        {
            if (parameter == null)
                return false;
            var (login, password) = ((string, string)) parameter;
            return !(string.IsNullOrEmpty(login) || string.IsNullOrEmpty(password));
        }

        public override void Execute(object parameter)
        {
            var (login, password) = ((string, string)) parameter;
            client.Registration(login, password);
        }

        public override event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}