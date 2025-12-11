using System.Windows;
using System.Windows.Controls;
using CitySkylines_REMAKE;
using CitySimulatorWPF.ViewModels;
using Domain.Map.Generation;
using Microsoft.Extensions.DependencyInjection;
using Services;
using Domain.Citizens;
using Services.CitizensSimulation;
using Domain.Common.Base;
using Domain.Map;

namespace CitySimulatorWPF.Views.components
{
    /// <summary>
    /// ������ �������������� ��� Header.xaml
    /// </summary>
    public partial class Header : UserControl
    {
        public Header()
        {
            InitializeComponent();
        }

        // ���� ������ ����� - �� �������
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // ���� ��� ��� ��������
        }

        // ������ ����������
        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var app = Application.Current as App;
                var simulation = app?._serviceProvider?.GetService<Simulation>();
                var saveManager = app?._serviceProvider?.GetService<GameSaveManager>();

                if (simulation == null || saveManager == null)
                {
                    MessageBox.Show("�� ���� �������� ������ � ��������", "������",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // ��������� ������� ���������
                saveManager.SaveCurrentState(simulation.MapModel);
                MessageBox.Show("?? ���� ���������!", "����������",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"? ������ ����������: {ex.Message}", "������",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ������ ��������
        private void LoadButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var app = Application.Current as App;
                var simulation = app?._serviceProvider?.GetService<Simulation>();
                var saveManager = app?._serviceProvider?.GetService<GameSaveManager>();
                var mapVM = app?._serviceProvider?.GetService<MapVM>();

                if (simulation == null || saveManager == null)
                {
                    MessageBox.Show("�� ���� �������� ������ � ��������", "������",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!saveManager.HasSavedGame())
                {
                    MessageBox.Show("��� ����������� ����", "��������",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    return;
                }

                // ��������� ����������� ���������
                saveManager.LoadSavedState(simulation.MapModel);

                // ��������� ������ ������ � MapVM
                if (mapVM != null)
                {
                    // ������� ��� ������
                    mapVM.BuildingIcons.Clear();

                    // ��������� ������ ��� ���� �������� �� �����
                    for (int x = 0; x < simulation.MapModel.Width; x++)
                    {
                        for (int y = 0; y < simulation.MapModel.Height; y++)
                        {
                            var mapObject = simulation.MapModel[x, y].MapObject;
                            if (mapObject != null)
                            {
                                var (placement, found) = simulation.GetMapObjectPlacement(mapObject);
                                if (found && placement != null)
                                {
                                    const int tileSize = 20;
                                    var iconVm = new BuildingIconVM(mapObject, (Placement)placement, tileSize);
                                    mapVM.BuildingIcons.Add(iconVm);
                                }
                            }
                        }
                    }
                }

                MessageBox.Show("?? ���� ���������!", "��������",
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"? ������ ��������: {ex.Message}", "������",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // ������ ������� �����
        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // ���������� �������������
                var result = MessageBox.Show(
                    "������� ��� ������ � �����?\n\n��� �������� ������ ��������!",
                    "������� �����",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Warning);

                if (result != MessageBoxResult.Yes)
                    return;

                // �������� ������ � ��������� ����� App
                var app = Application.Current as App;
                var simulation = app?._serviceProvider?.GetService<Simulation>();

                if (simulation == null)
                {
                    MessageBox.Show("�� ���� �������� ������ � ���������");
                    return;
                }

                // �������� ��� ������� �� �����
                var objectsToRemove = new List<MapObject>();
                for (int x = 0; x < simulation.MapModel.Width; x++)
                {
                    for (int y = 0; y < simulation.MapModel.Height; y++)
                    {
                        var mapObject = simulation.MapModel[x, y].MapObject;
                        if (mapObject != null && !objectsToRemove.Contains(mapObject))
                        {
                            objectsToRemove.Add(mapObject);
                        }
                    }
                }

                // ������� ��� ������� ��������� TryRemove (��� ������� ������� � ������ ������)
                int removedCount = 0;
                foreach (var mapObject in objectsToRemove)
                {
                    if (simulation.TryRemove(mapObject))
                    {
                        removedCount++;
                    }
                }

                // ����� ������� ���� ������� � ������ ����� �������
                var citizenService = app?._serviceProvider?.GetService<CitizenSimulationService>();
                var transportService = app?._serviceProvider?.GetService<global::Services.TransportSimulation.TransportSimulationService>();

                int citizensRemoved = 0;
                int transportsRemoved = 0;

                if (citizenService != null)
                {
                    var citizens = citizenService.Citizens.ToList();
                    foreach (var citizen in citizens)
                    {
                        simulation.RemoveCitizen(citizen);
                        citizensRemoved++;
                    }
                }

                if (transportService != null)
                {
                    var transports = transportService.Transports.ToList();
                    foreach (var transport in transports)
                    {
                        simulation.RemoveTransport(transport);
                        transportsRemoved++;
                    }
                }

                MessageBox.Show($"? ����� �������!\n������� ��������: {removedCount}\n������� �������: {citizensRemoved}\n������� �����: {transportsRemoved}",
                    "�����", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"? ������: {ex.Message}", "������",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}