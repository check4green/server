using System;
using System.Linq;
using System.Web.Http;
using System.Web.Http.Cors;
using Microsoft.Owin.Cors;
using SensorsManager.Web.Api.Hubs;
using SensorsManager.Web.Api.Models;
using SensorsManager.Web.Api.Repository;
using SensorsManager.Web.Api.Security;
using SensorsManager.Web.Api.ServiceInterfaces;

namespace SensorsManager.Web.Api.Controllers
{
    [EnableCors("*","*","*",
        exposedHeaders: "X-Tracker-Pagination-Page,X-Tracker-Pagination-PageSize," +
        "X-Tracker-Pagination-PageCount,X-Tracker-Pagination-TotalCount," +
        "X-Tracker-Pagination-PrevPage,X-Tracker-Pagination-NextPage")]


    public class ReadingsController : ApiControllerWithHub<ReadingsHub>
    {
        ISensorRepository _sensorRep;
        ISensorReadingRepository _readingRep;
        IThrottlerService _throttler;

        public ReadingsController(
            ISensorReadingRepository readingRep,
            ISensorRepository sensorRep,
            IThrottlerService throttler
            )
        {
            _readingRep = readingRep;
            _sensorRep = sensorRep;
            _throttler = throttler;
        }

        [Route("~/api/readings/address")]
        [HttpPost]
        public IHttpActionResult AddSensorReadingsByAddress(SensorReadingModelPost sensorReadingModel)
        {
            if (sensorReadingModel == null)
            {
                return BadRequest("You have sent an empty object");
            }

            if (ModelState.IsValid == false)
            {
                var error = ModelState.SelectMany(m => m.Value.Errors)
                .Where(m => m.ErrorMessage != "")
                .FirstOrDefault();

                if (error == null)
                {
                    return BadRequest();
                }

                return BadRequest(error.ErrorMessage);
            }

            _throttler.ThrottlerSetup(sensorReadingModel.SensorClientAddress, 1, 3);
            if (_throttler.RequestShouldBeThrottled())
            {
                return TooManyRequests(_throttler);
            }

            var sensor = _sensorRep.
                    GetSensorByAddress(sensorReadingModel.SensorGatewayAddress,
                    sensorReadingModel.SensorClientAddress);

            if (sensor != null)
            {
                var sensorReading = TheModelToEntityMap
                    .MapSensorReadingModelToSensorReadingEntity(sensorReadingModel, sensor.Id);

                //This is temporary
                sensorReading.ReadingDate = sensorReading.ReadingDate.AddHours(-1);

                var reading = _readingRep.AddSensorReading(sensorReading);

                var pending =
                         TheSensorIntervalPending
                         .GetPendingMember(sensor.Id);

                //Check if the penging exists
                if (pending != null)
                {
                    TheModelToEntityMap
                       .MapSensorModelToSensorEntity(pending, sensor);
                    TheSensorIntervalPending.ClearPending(pending);
                }

                sensor.Active = true;
                _sensorRep.UpdateSensor(sensor);
             
                var address = sensorReadingModel.SensorGatewayAddress
                    + "/" + sensorReadingModel.SensorClientAddress;
                Hub.Clients.Group(address).refreshReadings();
               


                return CreatedAtRoute("GetSensorReadingsBySensorAddressRoute",
                      new
                      {
                          gatewayAddress = sensorReadingModel.SensorGatewayAddress,
                          clientAddress = sensorReadingModel.SensorClientAddress
                      }, sensorReadingModel);

            }
            else
            {
                return NotFound($"There is no sensor with the address:" +
                    $"{sensorReadingModel.SensorGatewayAddress}/{sensorReadingModel.SensorClientAddress}");
            }


        }

        [SensorsManagerAuthorize]
        [Route("~/api/sensors/address/{gatewayAddress}/{clientAddress}/readings", Name = "GetSensorReadingsBySensorAddressRoute")]
        [HttpGet]
        public IHttpActionResult GetSensorReadingsBySensorAddress(string gatewayAddress, string clientAddress, int page = 1, int pageSize = 30)
        {
            var sensor = _sensorRep.GetSensorByAddress(gatewayAddress, clientAddress);
            if (sensor == null)
            {
                return NotFound();
            }

            if (page < 1) { page = 1; }
            if (pageSize < 1) { pageSize = 30; }

            var sensorReadings =
                sensor.SensorType.Code != "vibration" ?
                _readingRep.GetSensorReadingBySensorId(sensor.Id)
                : _readingRep.GetSensorReadingBySensorId(sensor.Id).Where(r => r.Value != 0);

            var totalCount = sensorReadings.Count();
            if (totalCount == 0)
            {
                return NotFound();
            }


            var pageCount = (int)Math.Ceiling((float)totalCount / pageSize);

            var results = sensorReadings.OrderByDescending(r => r.InsertDate)
                                        .Skip(pageSize * (page - 1))
                                        .Take(pageSize)
                                        .Select(p => TheModelFactory.CreateSensorReadingModel(p))
                                        .ToList();

            if (results.Count == 0)
            {
                return NotFound();
            }

            return Ok("GetSensorReadingsBySensorAddressRoute", page, pageSize, pageCount, totalCount, results);

        }
    }
}