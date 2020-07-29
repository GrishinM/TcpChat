using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using WpfClient.ViewModels;

namespace WpfClient.Views
{
    public partial class MainPage
    {
        private ClientViewModel Client { get; }

        public MainPage(ClientViewModel client)
        {
            InitializeComponent();
            Client = client;
            Client.Message += () => Do(() => ScrollViewer.ScrollToEnd());
            DataContext = Client;
            MsgBox.Focus();
        }

        private void Do(Action action)
        {
            Dispatcher.Invoke(action);
        }

        private void UIElement_OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter)
                return;
            if (e.KeyboardDevice.Modifiers == ModifierKeys.Shift)
            {
                MsgBox.AppendText(Environment.NewLine);
                MsgBox.CaretIndex = MsgBox.Text.Length;
            }
            else
            {
                Client.SendMessageCommand.Execute(MsgBox.Text);
                Clear();
            }
        }

        private void ButtonClick(object sender, RoutedEventArgs e)
        {
            Task.Run(() =>
            {
                Thread.Sleep(10);
                Do(Clear);
            });
        }

        private void Clear()
        {
            MsgBox.Text = "";
            MsgBox.Focus();
        }
    }
}