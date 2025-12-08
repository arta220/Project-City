using CommunityToolkit.Mvvm.ComponentModel;
using Domain.Common.Enums;

namespace CitySimulatorWPF.ViewModels
{
    /// <summary>
    /// ViewModel для элемента материала в диалоге покупки материалов
    /// </summary>
    /// <remarks>
    /// Класс представляет отдельный тип материала с его свойствами:
    /// - Название и тип ресурса
    /// - Цена за единицу
    /// - Количество в наличии
    /// - Выбранное для покупки количество
    /// - Общая стоимость выбранного количества
    /// </remarks>
    public partial class MaterialItemViewModel : ObservableObject
    {
        /// <summary>
        /// Получает тип природного ресурса
        /// </summary>
        /// <value>
        /// Тип природного ресурса из перечисления <see cref="NaturalResourceType"/>
        /// </value>
        public NaturalResourceType ResourceType { get; }

        /// <summary>
        /// Получает или задает название материала
        /// </summary>
        [ObservableProperty]
        private string _name;

        /// <summary>
        /// Получает или задает цену за единицу материала
        /// </summary>
        [ObservableProperty]
        private float _price;

        /// <summary>
        /// Получает или задает количество материала, уже имеющегося в инвентаре
        /// </summary>
        [ObservableProperty]
        private int _owned;

        /// <summary>
        /// Получает или задает количество материала, выбранное для покупки
        /// </summary>
        [ObservableProperty]
        private int _selectedQuantity;

        /// <summary>
        /// Получает общую стоимость выбранного количества материала
        /// </summary>
        /// <value>
        /// Произведение цены за единицу на выбранное количество
        /// </value>
        public float Total => Price * SelectedQuantity;

        /// <summary>
        /// Инициализация нового экземпляра класса <see cref="MaterialItemViewModel"/>.
        /// </summary>
        /// <param name="type">Тип природного ресурса</param>
        /// <param name="name">Название материала</param>
        /// <param name="price">Цена за единицу материала</param>
        /// <param name="owned">Количество материала в наличии</param>
        public MaterialItemViewModel(NaturalResourceType type, string name, float price, int owned)
        {
            ResourceType = type;
            _name = name;
            _price = price;
            _owned = owned;
            _selectedQuantity = 0;
        }

        /// <summary>
        /// Обработка изменения выбранного количества материала
        /// </summary>
        /// <param name="value">Новое значение выбранного количества</param>
        /// <remarks>
        /// При изменении выбранного количества автоматически обновляется свойство <see cref="Total"/>
        /// </remarks>
        partial void OnSelectedQuantityChanged(int value)
        {
            OnPropertyChanged(nameof(Total));
        }
    }
}