using CommunityToolkit.Mvvm.ComponentModel;
using Domain.Common.Base;
using Domain.Map;

namespace Domain.Base;

public class Road : MapObject
{
    private bool _isBlocked = false;

    /// <summary>
    /// Заблокирована ли дорога (например, из-за метели).
    /// </summary>
    public bool IsBlocked
    {
        get => _isBlocked;
        set
        {
            _isBlocked = value;
            OnPropertyChanged();
        }
    }

    public Road(Area area) : base(area)
    {
        
    }
}