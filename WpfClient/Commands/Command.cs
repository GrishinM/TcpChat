using System;
using System.Windows.Input;
using WpfClient.Models;

namespace WpfClient.Commands
{
    public abstract class Command : ICommand
    {
        protected readonly Client client;

        protected Command(Client client)
        {
            this.client = client;
        }
        
        public abstract bool CanExecute(object parameter);
        public abstract void Execute(object parameter);
        public abstract event EventHandler CanExecuteChanged;
    }
}