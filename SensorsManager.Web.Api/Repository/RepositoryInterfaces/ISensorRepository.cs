using SensorsManager.DomainClasses;
using System.Linq;

namespace SensorsManager.Web.Api.Repository
{
    public interface ISensorRepository
    {
        Sensor AddSensor(Sensor sensor);
        Sensor GetSensorByAddress(string gatewayAdress, string clientAdress);
        IQueryable<Sensor> GetSensosByGatewayAddress(string gatewayAdress);
        Sensor GetSensorById(int id);
        IQueryable<Sensor> GetAllSensors();
        void DeleteSensor(int id);
        void UpdateSensor(Sensor sensor);
    }
}
