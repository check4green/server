using SensorsManager.DomainClasses;
using System;

namespace SensorsManager.Web.Api.Services
{
    public interface IGatewayConnectionService
    {
        GatewayConnection Create(int gatewayId, int sensorId);
    }
}
