using SensorsManager.Web.Api.Repository;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace SensorsManager.Web.Api
{
    public static class ActivityCheck
    {

        public static void CheckSensorActivity()
        {
            var worker = new BackgroundWorker();
            worker.DoWork += Worker_DoWork;
            worker.RunWorkerAsync();
        }

        private static void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            SensorRepository sensorRep = new SensorRepository();
            SensorReadingRepository readingRep = new SensorReadingRepository();

            while (true)
            {
                var sensors = sensorRep.GetAllSensors().Where(p => p.Active == true);
                if (sensors == null)
                {
                    continue;
                }
                else
                {
                    Parallel.ForEach(sensors,
                        sensor =>
                        {
                            var wait = (sensor.UploadInterval + 1);
                            var readingDate = readingRep
                            .GetSensorReadingBySensorId(sensor.Id)
                            .OrderByDescending(s => s.InsertDate)
                            .FirstOrDefault().InsertDate;

                            if (Math.Ceiling((DateTime.UtcNow - readingDate).TotalMinutes) > wait)
                            {
                                sensor.Active = false;
                                sensorRep.UpdateSensor(sensor);
                            }
                        });
                    Thread.Sleep(60000);
                }
            }
        }
    }
}