using SensorsManager.DomainClasses;
using System;

namespace SensorsManager.Web.Api.Services
{
    public class GatewayConnectionService : IGatewayConnectionService
    {
        public GatewayConnection Create(int gatewayId, int sensorId)
        {
            return new GatewayConnection()
            {
                Gateway_Id = gatewayId,
                Sensor_Id = sensorId
            };
        }
    }
}