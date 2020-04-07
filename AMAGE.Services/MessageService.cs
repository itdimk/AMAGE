using System.Windows;

namespace AMAGE.Services
{
    public sealed class MessageService : IMessageService
    {
        public void ShowError(string error)
        {
            MessageBox.Show(error, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }

        public void ShowMessage(string message)
        {
            MessageBox.Show(message, "Message", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        public void ShowWarning(string warning)
        {
            MessageBox.Show(warning, "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}
