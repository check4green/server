using System;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Cors;
using SensorsManager.Web.Api.Models;
using SensorsManager.Web.Api.Repository;
using SensorsManager.Web.Api.Throttling;
using SensorsManager.Web.Api.Security;

namespace SensorsManager.Web.Api.Controllers
{
    [EnableCors("*", "*", "*",
        exposedHeaders: "X-Tracker-Pagination-Page,X-Tracker-Pagination-PageSize," +
        "X-Tracker-Pagination-PageCount,X-Tracker-Pagination-TotalCount," +
        "X-Tracker-Pagination-PrevPage,X-Tracker-Pagination-NextPage")]

    public class ReadingsController : BaseApiController
    {
        ISensorRepository _sensorRep;
        ISensorReadingRepository _readingRep;

        public ReadingsController(
            ISensorReadingRepository readingRep,
            ISensorRepository sensorRep
            )
        {
            _readingRep = readingRep;
            _sensorRep = sensorRep;
        }

        [Route("~/api/readings/address")]
        [HttpPost]
        public IHttpActionResult AddSensorReadingsByAddress(SensorReadingModelPostAddres sensorReadingModel)
        {
            var throttler = new Throttler(sensorReadingModel.SensorClientAddress, 1, 3);
            if (throttler.RequestShouldBeThrottled())
            {
                return TooManyRequests(throttler);
            }

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


                HttpContext.Current.Response.AppendHeader("X-RateLimit-RequestLimit",
                   throttler.RequestLimit.ToString());

                HttpContext.Current.Response.AppendHeader("X-RateLimit-RequestsRemaining",
                  throttler.RequestsRemaining.ToString());

                HttpContext.Current.Response.AppendHeader("X-RateLimit-ExpiresAt",
                                   throttler.ExpiresAt.ToString());


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