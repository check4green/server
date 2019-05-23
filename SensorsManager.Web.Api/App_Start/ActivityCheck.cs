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
            var sensorWorker = new BackgroundWorker();
            var gatewayWorker = new BackgroundWorker();
            sensorWorker.DoWork += SensorWorker_DoWork;
            gatewayWorker.DoWork += GatewayWorker_DoWork;
            sensorWorker.RunWorkerAsync();
            gatewayWorker.RunWorkerAsync();
        }

        private static void SensorWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            SensorRepository sensorRep = new SensorRepository();
            SensorReadingRepository readingRep = new SensorReadingRepository();
          
            while (true)
            {
                
                    var sensors = sensorRep.GetAll().Where(p => p.Active == true);
                    if (sensors.Count() == 0)
                    {
                        Thread.Sleep(60000);
                        continue;
                    }
                    else
                    {
                        Parallel.ForEach(sensors,
                            sensor =>
                            {
                                var wait = (sensor.UploadInterval + 1);
                                var readingDate = readingRep
                                .Get(sensor.Id)
                                .OrderByDescending(s => s.InsertDate)
                                .FirstOrDefault().InsertDate;

                                if (Math.Ceiling((DateTime.UtcNow - readingDate).TotalMinutes) > wait)
                                {
                                    sensor.Active = false;
                                    sensorRep.Update(sensor);
                                }
                            });
                        Thread.Sleep(60000);
                    }
            }
        }

        private static void GatewayWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            var gatewayRep = new GatewayRepository();
            while (true)
            {
                var gateways = gatewayRep.GetAll().Where(p => p.Active == true);
                if(gateways.Count() == 0)
                {
                    Thread.Sleep(60000);
                    continue;
                }
                else
                {
                    Parallel.ForEach(gateways,
                        gateway => {
                            var wait = gateway.UploadInterval;
                            var lastSignalDate = gateway.LastSignalDate;
                            if (lastSignalDate.HasValue)
                            {
                                if (Math.Ceiling((DateTime.UtcNow - lastSignalDate.Value).TotalMinutes) > wait)
                                {
                                    gateway.Active = false;
                                    gatewayRep.Update(gateway);
                                }
                            }
                        });
                }   
            }
        }
    }
}