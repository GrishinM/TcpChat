using System;
using System.Windows.Input;
using WpfClient.Models;

namespace WpfClient.Commands
{
    public class SendMessageCommand : Command
    {
        public SendMessageCommand(Client client) : base(client)
        {
        }

        public override bool CanExecute(object parameter)
        {
            var message = (string) parameter;
            return !string.IsNullOrEmpty(message);
        }

        public override void Execute(object parameter)
        {
            var message = (string) parameter;
            client.SendMessage(message);
        }

        public override event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }
    }
}