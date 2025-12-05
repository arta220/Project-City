using Domain.Common.Base;
using Domain.Map;
using System.Security.AccessControl;
using static Domain.Buildings.IndustrialBuilding;

namespace Domain.Buildings
{
    /// <summary>
    /// ������������� ����� �������
    /// �������� ����� �������� � ������ ��� ���� ����� ������������ ������
    /// ��������� ����� ������� ���������� � <see cref="AllFactories"/>
    /// </summary>
    public class IndustrialBuilding : Building
    {
        /// <summary>
        /// ���������� ����� � ��� ������ ������.
        /// ��������� ����������� ������ ���� ������� � ������.
        /// </summary>
        public class Workshop
        {
            private readonly IndustrialBuilding _parent;

            public Enum InputMaterial { get; }
            public Enum OutputProduct { get; }
            public int ProductionCoefficient { get; }

            public Workshop(
                IndustrialBuilding parent,
                Enum inputMaterial,
                Enum outputProduct,
                int productionCoefficient = 1)
            {
                _parent = parent;
                InputMaterial = inputMaterial;
                OutputProduct = outputProduct;
                ProductionCoefficient = productionCoefficient;
            }

            /// <summary>
            /// ��������� ����������� ��������:
            /// �������� ProductionCoefficient ��������� � ���������� ProductionCoefficient ������ ���������.
            /// </summary>
            /// <returns> True, ���� ����������� ������ �������, ����� false </returns>
            public bool Process()
            {
                // ���������, ���� �� ������� ��������
                int amount = 0;
                bool foundInMaterials = false;

                // Сначала проверка MaterialsBank
                if (_parent.MaterialsBank.TryGetValue(InputMaterial, out amount) && amount > 0)
                {
                    foundInMaterials = true;
                }
                // Если не найдено в MaterialsBank, проверка ProductsBank (продукты могут быть использованы как материалы)
                else if (_parent.ProductsBank.TryGetValue(InputMaterial, out amount) && amount > 0)
                {
                    foundInMaterials = false;
                }
                else
                {
                    return false; // Материал не найден ни в одном банке
                }

                // Уменьшение количества материала
                if (foundInMaterials)
                {
                    _parent.MaterialsBank[InputMaterial] = amount - ProductionCoefficient;
                }
                else
                {
                    _parent.ProductsBank[InputMaterial] = amount - ProductionCoefficient;
                }

                // Увеличение количества продукта
                if (!_parent.ProductsBank.ContainsKey(OutputProduct))
                    _parent.ProductsBank[OutputProduct] = 0;

                _parent.ProductsBank[OutputProduct] += ProductionCoefficient;

                return true;
            }
        }

        public Dictionary<Enum, int> MaterialsBank = new Dictionary<Enum, int>(); // ��������� �� ������
        public Dictionary<Enum, int> ProductsBank = new Dictionary<Enum, int>(); // ��������� �� ������
        public List<Workshop> Workshops = new(); // ������ ����� �� ������

        public IndustrialBuilding(int floors, int maxOccupancy, Area area)
            : base(floors, maxOccupancy, area)
        {
        }


        /// <summary>
        /// �������� ����� ���.
        /// </summary>
        public void AddWorkshop(Enum input, Enum output, int coeff = 1)
        {
            var ws = new Workshop(this, input, output, coeff);
            Workshops.Add(ws);
        }

        /// <summary>
        /// ��������� ��� ���� ���� ���.
        /// </summary>
        public void RunOnce()
        {
            foreach (var ws in Workshops)
                ws.Process();
        }
    }
}
