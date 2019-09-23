using System.Collections.Generic;

namespace SensorsManager.Web.Api.Validations
{
    public interface IVibrationFilter
    {
        List<int> ValidValues();
    }
}
