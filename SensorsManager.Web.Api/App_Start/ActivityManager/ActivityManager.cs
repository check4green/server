using SensorsManager.DataLayer;
using SensorsManager.DomainClasses;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace SensorsManager.Web.Api
{
    public class ActivityManager : IActivityManager
    {

        private List<Sensor> _sensorList;
        private List<Gateway> _gatewayList;

        public void CheckActivity()
        {
            var sensorWorker = new BackgroundWorker();
            var gatewayWorker = new BackgroundWorker();
            sensorWorker.DoWork += SensorWorker_DoWork;
            gatewayWorker.DoWork += GatewayWorker_DoWork;
            sensorWorker.RunWorkerAsync();
            gatewayWorker.RunWorkerAsync();
        }

        private void SensorWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                using(DataContext dc = new DataContext())
                {
                    _sensorList = dc.Sensors.Where(s => s.Active == true).ToList();
                }

                if (_sensorList.Count() == 0)
                {
                    Thread.Sleep(60000);
                    continue;
                }
                else
                {
                    Parallel.ForEach(_sensorList,
                        sensor =>
                        {
                            var wait = (sensor.UploadInterval + 1);
                            var lastInsertDate = sensor.LastInsertDate.Value;

                            if (Math.Ceiling((DateTime.UtcNow - lastInsertDate).TotalMinutes) > wait)
                            {
                                sensor.Active = false;
                                using(DataContext dc = new DataContext())
                                {
                                    dc.Entry(sensor).State = EntityState.Modified;
                                    dc.SaveChanges();
                                }
                            }
                        });
                    Thread.Sleep(60000);
                }
            }
        }
        private void GatewayWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (true)
            {
                using(DataContext dc = new DataContext())
                {
                    _gatewayList = dc.Gateways.Where(g => g.Active == true).ToList();
                }

                if (_gatewayList.Count() == 0)
                {
                    Thread.Sleep(60000);
                    continue;
                }
                else
                {
                    Parallel.ForEach(_gatewayList,
                        gateway => {
                            var wait = gateway.UploadInterval;
                            var lastSignalDate = gateway.LastSignalDate.Value;

                            if (Math.Ceiling((DateTime.UtcNow - lastSignalDate).TotalMinutes) > wait)
                            {
                                gateway.Active = false;
                                using(DataContext dc = new DataContext())
                                {
                                    dc.Entry(gateway).State = EntityState.Modified;
                                    dc.SaveChanges();
                                }
                            }
                        });
                }
            }
        }
    }
}