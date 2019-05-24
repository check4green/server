using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorsManager.Web.Api.Services
{
    public interface IRandomService
    {
        int GetRandomNumber(int minValue, int maxValue);
    }
}
