using CitySkylines_REMAKE.ViewModels;
using CitySkylines_REMAKE.Views;
using System.Windows;

namespace CitySkylines_REMAKE
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var vm = new MainVM();

            var mainWindow = new MainWindow(vm);
            mainWindow.Show();
        }
    }

}
