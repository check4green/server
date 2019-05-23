using System;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Cors;
using SensorsManager.Web.Api.DependencyBlocks;
using SensorsManager.Web.Api.Models;
using SensorsManager.Web.Api.Repository;
using SensorsManager.Web.Api.Validations;

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

        public MeasurementsController(IMeasurementsControllerDependencyBlock dependencyBlock)
        {
            _measureRep = dependencyBlock.MeasurementRepository;
        }

        [HttpPost,Route(""),ValidateModel]
        public IHttpActionResult Add(MeasurementModel measureModel)
        {
            if (measureModel == null)
            {
                return BadRequest("You have sent an empty object.");
            }

            var measure = _measureRep
                .GetAll()
                .Where(m => m.UnitOfMeasure == measureModel.UnitOfMeasure)
                .SingleOrDefault();

            if (measure != null)
            {
                return Conflict("This unit of measure already exists!");
            }

            var newMeasure = ModelToEntityMap
                .MapToEntity(measureModel);
            _measureRep.Add(newMeasure);

            return CreatedAtRoute("GetMeasurement",
                new { id = newMeasure.Id }, newMeasure);
        }

        [HttpGet,Route("{id:int}", Name = "GetMeasurement")]
        public IHttpActionResult Get(int id)
        {

            var measurement = _measureRep.Get(id);

            if (measurement == null)
            {
                return NotFound();
            }

            var measurementModel = ModelFactory.CreateModel(measurement);

            return Ok(measurementModel);
        }

        [HttpGet,Route("", Name = "GetAllMeasurements")]
        public IHttpActionResult GetAll(int page = 1, int pageSize = 30)
        {
            var totalCount = _measureRep.GetAll().Count();

            if (page < 1) { page = 1; }
            if (pageSize < 1) { pageSize = 30; }

            var pageCount = (int)Math.Ceiling((float)totalCount / pageSize);

            var results = _measureRep.GetAll()
                 .Skip(pageSize * (page - 1))
                 .Take(pageSize).Select(m => ModelFactory.CreateModel(m))
                 .OrderBy(m => m.Id).ToList();

            return Ok("GetAllMeasurements", page, pageSize, pageCount, totalCount, results);

        }

        [HttpPut,Route("{id:int}"),ValidateModel]
        public IHttpActionResult Update(int id, MeasurementModel measureModel)
        {

            if (measureModel == null)
            {
                return BadRequest("You have sent an empty object.");
            }

            var checkedMeasure = _measureRep.GetAll()
                .Where(m =>
                m.UnitOfMeasure == measureModel.UnitOfMeasure
                && m.Id != id)
                .SingleOrDefault();

            if (checkedMeasure != null)
            {
                return Conflict("This unit of measure already exists!");
            }

            var measurement = _measureRep.Get(id);
            if (measurement == null)
            {
                return NotFound();
            }

            ModelToEntityMap.MapToEntity(measureModel, measurement);
            _measureRep.Update(measurement);

            return StatusCode(HttpStatusCode.NoContent);
        }

        [HttpDelete,Route("{id:int}")]
        public void Delete(int id)
        {
           _measureRep.Delete(id);
        }
    }
}