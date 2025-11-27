namespace CitySimulatorWPF.Services
{
    public interface IMessageService
    {
        void ShowMessage(string message, string caption = "Info");
    }

    public class MessageService : IMessageService
    {
        public void ShowMessage(string message, string caption = "Info")
        {
            System.Windows.MessageBox.Show(message, caption);
        }
    }
}
