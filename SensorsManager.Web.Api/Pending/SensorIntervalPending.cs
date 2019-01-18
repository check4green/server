using System.Linq;
using System.Collections.Generic;

namespace SensorsManager.Web.Api.Pending
{
    public class SensorIntervalPending
    {
        private static List<SensorPendingModel> sensors;

        public SensorIntervalPending()
        {
            sensors = new List<SensorPendingModel>();
        }

        public void AddToPending(SensorPendingModel sensorPendingModel)
        {
            var oldSensor =
                GetPendingMember(sensorPendingModel.Id);
                  
            if (oldSensor != null)
            {
                oldSensor.UploadInterval = sensorPendingModel.UploadInterval;       
            }
            else
            {
                sensors.Add(sensorPendingModel);
            }
        }

        public SensorPendingModel GetPendingMember(int id)
        {
            return sensors.Where(s => s.Id == id).SingleOrDefault();
        }

        public void ClearPending(SensorPendingModel sensorPendingModel)
        {
            sensors.Remove(sensorPendingModel);
        }
    }
}