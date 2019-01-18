using System;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Cors;
using SensorsManager.Web.Api.Models;
using SensorsManager.Web.Api.Repository;

namespace SensorsManager.Web.Api.Controllers
{
    [EnableCors("*", "*", "*", 
        exposedHeaders: "X-Tracker-Pagination-Page,X-Tracker-Pagination-PageSize," +
        "X-Tracker-Pagination-PageCount,X-Tracker-Pagination-TotalCount," +
        "X-Tracker-Pagination-PrevPage,X-Tracker-Pagination-NextPage")]
    [RoutePrefix("api/measurements")]
    public class MeasurementsController : BaseApiController
    {
        IMeasurementRepository _measureRep;
       
        public MeasurementsController(IMeasurementRepository measureRep)
        {
            _measureRep = measureRep;
        }

        [Route("", Name = "AddMeasurementRoute")]
        [HttpPost]
        public IHttpActionResult AddMeasurement(MeasurementModel measureModel)
        {
            if (measureModel == null)
            {
                return BadRequest("You have sent an empty object.");
            }

            if(ModelState.IsValid == false)
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

            var measure = _measureRep
                .GetAllMeasurements()
                .Where(m => m.UnitOfMeasure == measureModel.UnitOfMeasure)
                .SingleOrDefault();

            if(measure != null)
            {
                return Conflict("This unit of measure already exists!");
            }

            var newMeasure = TheModelToEntityMap
                .MapMeasurementModelToMeasurementEntity(measureModel);
            _measureRep.AddMeasurement(newMeasure);

            return CreatedAtRoute("GetMeasurementByIdRoute",
                new { id = newMeasure.Id }, newMeasure);
        }

        [Route("{id:int}", Name = "GetMeasurementByIdRoute")]
        [HttpGet]
        public IHttpActionResult GetMeasurementById(int id)
        {
       
            var measurement = _measureRep.GetMeasurementById(id);

            if (measurement == null)
            {
                return NotFound();
            }

            var measurementModel = TheModelFactory.CreateMeasurementModel(measurement);
           
            return Ok(measurementModel);
        }

        [Route("", Name = "GetAllMeasurementsRoute")]
        [HttpGet]
        public IHttpActionResult GetAllMeasurements(int page = 1, int pageSize = 30)
        {
            var totalCount = _measureRep.GetAllMeasurements().Count();

            if (totalCount == 0)
            {
                return NotFound();
            }

            if (page < 1) { page = 1; }
            if (pageSize < 1) { pageSize = 30; }

            var pageCount = (int)Math.Ceiling((float)totalCount / pageSize);

            var results = _measureRep.GetAllMeasurements()
                 .Skip(pageSize * (page - 1))
                 .Take(pageSize).Select(m => TheModelFactory.CreateMeasurementModel(m))
                 .OrderBy(m => m.Id).ToList();

            if (results.Count == 0)
            {
                return NotFound();
            }

            return Ok("GetAllMeasurementsRoute", page, pageSize, pageCount, totalCount, results);

        }

        [Route("{id:int}")]
        [HttpPut]
        public IHttpActionResult UpdateMeasurement(int id, MeasurementModel measureModel)
        {

            if (measureModel == null)
            {
                return BadRequest("You have sent an empty object.");
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

            var checkedMeasure = _measureRep.GetAllMeasurements()
                .Where(m =>
                m.UnitOfMeasure == measureModel.UnitOfMeasure
                && m.Id != id)
                .SingleOrDefault();

            if(checkedMeasure != null)
            {
                return Conflict("This unit of measure already exists!");
            }

            var measurement = _measureRep.GetMeasurementById(id);
            if (measurement == null)
            {
                return NotFound();
            }

            TheModelToEntityMap.MapMeasurementModelToMeasurementEntity(measureModel, measurement);
            _measureRep.UpdateMeasurement(measurement);

            return StatusCode(HttpStatusCode.NoContent);
        }

        [Route("{id:int}", Name = "DeleteMeasurementRoute")]
        [HttpDelete]
        public void DeleteMeasurement(int id)
        {
            if (_measureRep.GetMeasurementById(id) != null)
            {
                _measureRep.DeleteMeasurement(id);
            }
        }

    }
}