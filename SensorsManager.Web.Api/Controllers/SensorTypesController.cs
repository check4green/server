using SensorsManager.Web.Api.Repository;
using System.Net;
using System.Web.Http;
using System.Linq;
using System.Web.Http.Cors;
using SensorsManager.Web.Api.Models;
using System;
using SensorsManager.Web.Api.Validations;
using SensorsManager.Web.Api.DependencyBlocks;

namespace SensorsManager.Web.Api.Controllers
{
    [EnableCors("*", "*", "*",
        exposedHeaders: "X-Tracker-Pagination-Page,X-Tracker-Pagination-PageSize," +
        "X-Tracker-Pagination-PageCount,X-Tracker-Pagination-TotalCount," +
        "X-Tracker-Pagination-PrevPage,X-Tracker-Pagination-NextPage")]
    [RoutePrefix("api/sensor-types")]
    public class SensorTypesController : BaseApiController
    {
        IMeasurementRepository _measureRep;
        ISensorTypesRepository _typeRep;
        
        public SensorTypesController(ISensorTypesControllerDependencyBlock dependencyBlock)
        {
            _measureRep = dependencyBlock.MeasurementRepository;
            _typeRep = dependencyBlock.TypesRepository;
        }

        [HttpPost,Route(""),ValidateModel]
        public IHttpActionResult Add(SensorTypeModel sensorTypeModel)
        {
            if (sensorTypeModel == null)
            {
                return BadRequest("You have sent an empty object.");
            }
            var measure = _measureRep.Get(sensorTypeModel.MeasureId);
            if (measure == null)
            {
                return NotFound($"There is no measurement with the id {sensorTypeModel.MeasureId}");
            }

            var checkedSensorType =
                _typeRep.GetAll()
                .Where(st => st.Name == sensorTypeModel.Name)
                .SingleOrDefault();

            if (checkedSensorType != null)
            {
                return Conflict("The code filed must be unique");
            }

            var sensorType = ModelToEntityMap.MapToEntity(sensorTypeModel);
            _typeRep.Add(sensorType);



            return CreatedAtRoute("GetSensorType", new { id = sensorType.Id }, sensorType);
        }

        [HttpGet,Route("{id:int}", Name = "GetSensorType")]
        public IHttpActionResult Get(int id)
        {
            var sensorType = _typeRep.Get(id);
            if (sensorType == null)
            {
                return NotFound();
            }
            var sensorTypeModel = ModelFactory.CreateModel(sensorType);

            return Ok(sensorTypeModel);
        }

        [HttpGet,Route("", Name = "GetAllSensorTypes")]
        public IHttpActionResult GetAll(int page = 1, int pageSize = 30)
        {

            var totalCount = _typeRep.GetAll().Count();

            if (page < 1) { page = 1; }
            if (pageSize < 1) { pageSize = 30; }

            var totalPages = Math.Ceiling((float)totalCount / pageSize);


            var pageCount = (int)Math.Ceiling((float)totalCount / pageSize);

            var results = _typeRep.GetAll()
                    .Skip(pageSize * (page - 1))
                    .Take(pageSize)
                    .OrderBy(st => st.Id)
                    .Select(st => ModelFactory.CreateModel(st)).ToList();

            return Ok("GetAllSensorTypes", page, pageSize, pageCount, totalCount, results);
        }

        [HttpPut,Route("{id:int}"),ValidateModel]
        public IHttpActionResult Update(int id, SensorTypeModel sensorTypeModel)
        {
            if (sensorTypeModel == null)
            {
                return BadRequest("You have sent an empty object.");
            }

            var sensorType = _typeRep.Get(id);
            if (sensorType == null)
            {
                return NotFound();
            }

            var checkedSensorType = _typeRep
                .GetAll()
                .Where(
                st => st.Name == sensorTypeModel.Name
                && st.Id != id).SingleOrDefault();

            if (checkedSensorType != null)
            {
                return Conflict("There already exists a sensor-type with that code.");
            }

            ModelToEntityMap
                .MapToEntity(sensorTypeModel, sensorType);

            _typeRep.Update(sensorType);

            return StatusCode(HttpStatusCode.NoContent);
        }

        [HttpDelete,Route("{id:int}")]
        public void Delete(int id)
        {
           _typeRep.Delete(id);
        }
    }
}