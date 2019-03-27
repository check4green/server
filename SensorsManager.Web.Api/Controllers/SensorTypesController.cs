using SensorsManager.Web.Api.Repository;
using System.Net;
using System.Web.Http;
using System.Linq;
using System.Web.Http.Cors;
using SensorsManager.Web.Api.Models;
using System;


namespace SensorsManager.Web.Api.Controllers
{
    [EnableCors("*", "*", "*",
        exposedHeaders: "X-Tracker-Pagination-Page,X-Tracker-Pagination-PageSize," +
        "X-Tracker-Pagination-PageCount,X-Tracker-Pagination-TotalCount," +
        "X-Tracker-Pagination-PrevPage,X-Tracker-Pagination-NextPage")]
    [RoutePrefix("api/sensor-types")]
    public class SensorTypesController : BaseApiController
    {
        ISensorTypesRepository _sensorTypeRep;
        IMeasurementRepository _measureRep;

        public SensorTypesController(
            ISensorTypesRepository sensorTypeRep,
            IMeasurementRepository measureRep
            )
        {
            _sensorTypeRep = sensorTypeRep;
            _measureRep = measureRep;
        }

        [Route("", Name = "AddSensorTypeRoute")]
        [HttpPost]
        public IHttpActionResult AddSensorType(SensorTypeModel sensorTypeModel)
        {
          
            if (sensorTypeModel == null)
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
            var measure = _measureRep.GetMeasurementById(sensorTypeModel.MeasureId);
            if (measure == null)
            {
                return NotFound($"There is no measurement with the id {sensorTypeModel.MeasureId}");
            }

            var checkedSensorType =
                _sensorTypeRep.GetAllSensorTypes()
                .Where(st => st.Code == sensorTypeModel.Code)
                .SingleOrDefault();

            if(checkedSensorType != null)
            {
                return Conflict("The code filed must be unique");
            }
       
            var sensorType = TheModelToEntityMap.MapSensorTypeModelToSensorTypeEnrity(sensorTypeModel);
            _sensorTypeRep.AddSensorType(sensorType);



            return CreatedAtRoute("GetSensorTypeByIdRoute", new { id = sensorType.Id }, sensorType);
        }

        [Route("{id:int}", Name = "GetSensorTypeByIdRoute")]
        [HttpGet]
        public IHttpActionResult GetSensorTypeById(int id)
        {
            var sensorType = _sensorTypeRep.GetSensorTypeById(id);
            if (sensorType == null)
            {
                return NotFound();
            }
            var sensorTypeModel = TheModelFactory.CreateSensorTypeModel(sensorType);

            return Ok(sensorTypeModel);
        }


        [Route("", Name = "GetAllSensorsTypeRoute")]
        [HttpGet]
        public IHttpActionResult GetAllSensorTypes(int page = 1, int pageSize = 30)
        {

            var totalCount = _sensorTypeRep.GetAllSensorTypes().Count();

            if (totalCount == 0)
            {
                return NotFound();
            }

            if (page < 1) { page = 1; }
            if (pageSize < 1) { pageSize = 30; }

            var totalPages = Math.Ceiling((float)totalCount / pageSize);


            var pageCount = (int)Math.Ceiling((float)totalCount / pageSize);

            var results = _sensorTypeRep.GetAllSensorTypes()
                    .Skip(pageSize * (page - 1))
                    .Take(pageSize)
                    .OrderBy(st => st.Id)
                    .Select(st => TheModelFactory.CreateSensorTypeModel(st)).ToList();


            if (results.Count == 0)
            {
                return NotFound();
            }

            return Ok("GetAllSensorsTypeRoute", page, pageSize, pageCount, totalCount, results);
        }

        [Route("{id:int}")]
        [HttpPut]
        public IHttpActionResult UpdateSensorType(int id, SensorTypeModel sensorTypeModel)
        {
         
            if (sensorTypeModel == null)
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

            var sensorType = _sensorTypeRep.GetSensorTypeById(id);
            if (sensorType== null)
            {
                return NotFound();
            }

            var checkedSensorType = _sensorTypeRep
                .GetAllSensorTypes()
                .Where(
                st => st.Code == sensorTypeModel.Code
                && st.Id != id).SingleOrDefault();

            if(checkedSensorType != null)
            {
                return Conflict("There already exists a sensor-type with that code.");
            }

            TheModelToEntityMap
                .MapSensorTypeModelToSensorTypeEntity(sensorTypeModel, sensorType);

            _sensorTypeRep.UpdateSensorType(sensorType);

            return StatusCode(HttpStatusCode.NoContent);
        }

        [Route("{id:int}", Name = "DeleteSensorTypeRoute")]
        [HttpDelete]
        public void DeleteSensorType(int id)
        {
            if (_sensorTypeRep.GetSensorTypeById(id) != null)
            {
                _sensorTypeRep.DeleteSensorType(id);
            }
        }
    }
}