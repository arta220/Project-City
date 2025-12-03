using Domain.Common.Base;
using Domain.Common.Enums;
using Domain.Map;

namespace Domain.Buildings
{
    public class IndustrialBuilding : Building
    {
        public IndustrialBuilding(int floors, int maxOccupancy, Area area)
            : base(floors, maxOccupancy, area)
        {
        }

        /// <summary>
        /// ѕреобразует входные ресурсы в выходные ресурсы на основе прайс-листа.
        /// </summary>
        /// <param name="inputRsourse">входные ресурсы</param>
        /// <param name="outputResourse">выходные ресурсы</param>
        /// <returns>словарь из выходных ресурсов</returns>
        /// <exception cref="NotImplementedException"></exception>
        public virtual Dictionary<ResourseType, int> ResourseConverter(
            Dictionary<ResourseType, int> inputRsourse,
            Dictionary<ResourseType, int> outputResourse)
        {
            throw new NotImplementedException();
        }
    }
}
