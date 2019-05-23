using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SensorsManager.Web.Api.Services
{
    public interface IGuidService
    {
        Guid GetGuid();
        string GetAddress();
    }
}