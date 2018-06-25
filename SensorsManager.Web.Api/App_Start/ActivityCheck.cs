using SensorsManager.Web.Api.Repository;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

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
                var sensors = sensorRep.GetAllSensors();
                if(sensors == null)
                {
                    continue;
                }
                else
                {
                    Parallel.ForEach(sensors,
                        sensor =>
                        {
                                if (sensor.Activ)
                                {
                                    var wait = (sensor.UploadInterval + 1);
                                    var readingDate = readingRep
                                    .GetSensorReadingBySensorId(sensor.Id)
                                    .OrderByDescending(s => s.InsertDate)
                                    .FirstOrDefault().InsertDate;

                                    if ((DateTime.UtcNow - readingDate).TotalMinutes > wait)
                                    {
                                        sensor.Activ = false;
                                        sensorRep.UpdateSensor(sensor);
                                    }
                                }
                        });
                    Thread.Sleep(60000);
                }





            }
        }
    }
}