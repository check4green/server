using System.Collections.Generic;

namespace SensorsManager.Web.Api.Validations
{
    public class VibrationFilter : IVibrationFilter
    {
        public List <int> ValidValues()
        {
            return new List<int> { 100, 200, 300, 400, 500, 600, 700 };
        }
    }
}