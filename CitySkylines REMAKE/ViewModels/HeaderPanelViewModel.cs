using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Windows;
using System.Windows.Media.Media3D;

namespace CitySkylines_REMAKE.ViewModels
{
    public partial class HeaderPanelViewModel : ObservableObject
    {
        public event Action RemoveModeOn;

        [RelayCommand]
        private void SetRemoveMode()
        {
            MessageBox.Show("отладка");
            RemoveModeOn?.Invoke();
        }
    }
}
