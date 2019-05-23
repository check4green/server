using SensorsManager.DomainClasses;
using System.Linq;

namespace SensorsManager.Web.Api.Repository
{
    public interface IGatewayConnectionRepository
    {
        GatewayConnection Add(GatewayConnection gatewayConnection);
        IQueryable<GatewayConnection> GetAll();
        void Delete(int sensorId);
    }
}
