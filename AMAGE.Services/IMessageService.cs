namespace AMAGE.Services
{
    public interface IMessageService
    {
        void ShowMessage(string message);
        void ShowWarning(string warning);
        void ShowError(string error);
    }
}
