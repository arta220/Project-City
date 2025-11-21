using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace CitySkylines_REMAKE.Views.components
{
    public partial class Map : UserControl
    {
        // Поля для логики перетаскивания
        private Point? _lastMousePosition;
        private bool _isDragging = false;

        public Map()
        {
            InitializeComponent();
        }

        // --- ЗУМ КАРТЫ ---
        private void MapScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Keyboard.Modifiers != ModifierKeys.Control) return;
            var scale = e.Delta > 0 ? 1.1 : 0.9;
            ScaleTransform.ScaleX *= scale;
            ScaleTransform.ScaleY *= scale;
            e.Handled = true;
        }

        // --- ПЕРЕТАСКИВАНИЕ (ПКМ) ---
        private void MapScrollViewer_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Начинаем перетаскивание только по Правой Кнопке Мыши
            if (e.ChangedButton == MouseButton.Right)
            {
                if (e.Handled) return;

                // Сохраняем начальную позицию мыши относительно UserControl
                _lastMousePosition = e.GetPosition(this);
                _isDragging = true;

                MapScrollViewer.CaptureMouse();
                MapScrollViewer.Cursor = Cursors.SizeAll;
                e.Handled = true;
            }
        }

        // --- ПЕРЕМЕЩЕНИЯ (ДРАГ) ---
        private void MapScrollViewer_MouseMove(object sender, MouseEventArgs e)
        {
            // Если активно перетаскивание
            if (_isDragging && _lastMousePosition.HasValue)
            {
                Point currentPosition = e.GetPosition(this);

                Vector delta = currentPosition - _lastMousePosition.Value;

                // Применяем смещение к TranslateTransform
                TranslateTransform.X += delta.X;
                TranslateTransform.Y += delta.Y;

                _lastMousePosition = currentPosition;
                MapScrollViewer.Cursor = Cursors.SizeAll;
            }
            // В неактивном режиме перетаскивания курсор должен быть стрелкой
            else if (!_isDragging)
            {
                MapScrollViewer.Cursor = Cursors.Arrow;
            }
        }

        // --- ЗАВЕРШЕНИЕ ПЕРЕТАСКИВАНИЯ ---   
        private void MapScrollViewer_MouseUp(object sender, MouseButtonEventArgs e)
        {
            // Завершаем перетаскивание только если оно было активно
            if (_isDragging && e.ChangedButton == MouseButton.Right)
            {
                _isDragging = false;
                _lastMousePosition = null;

                MapScrollViewer.ReleaseMouseCapture();
                MapScrollViewer.Cursor = Cursors.Arrow;
                e.Handled = true;
            }
        }
    }
}