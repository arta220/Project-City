using CommunityToolkit.Mvvm.ComponentModel;
using Domain.Base;

public partial class MapObjectVM : ObservableObject
{
    public MapObject Model { get; }

    public string Name { get; set; }
    public string IconPath { get; set; }

    public MapObjectVM(MapObject model, string name, string iconPath)
    {
        Model = model;
        Name = name;
        IconPath = iconPath;
    }
}
