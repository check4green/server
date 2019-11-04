using SensorsManager.Web.Api.Repository;
using System.Net;
using System.Web.Http;
using System.Linq;
using System.Web.Http.Cors;
using SensorsManager.Web.Api.Models;
using System;
using SensorsManager.Web.Api.Validations;
using AutoMapper;
using SensorsManager.DomainClasses;
using Microsoft.AspNetCore.JsonPatch;
using SensorsManager.Web.Api.Services;
using Microsoft.AspNetCore.JsonPatch.Exceptions;

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
        IMapper _mapper;
        IMessageService _messages;

        public SensorTypesController(IMeasurementRepository measureRep, ISensorTypesRepository typeRep, 
            IMapper mapper, IMessageService messages)
        {
            _measureRep = measureRep;
            _typeRep = typeRep;
            _mapper = mapper;
            _messages = messages;
        }

        [HttpPost,Route(""),ValidateModel]
        public IHttpActionResult Add(SensorTypeModelPost sensorTypeModel)
        {
            if (sensorTypeModel == null)
            {
                var errorMessage = _messages.GetMessage(Generic.NullObject);
                return BadRequest(errorMessage);
            }
     
            if (!_measureRep.Exists(sensorTypeModel.MeasureId))
            {
                var errorMessage = _messages.GetMessage(Custom.NotFound, "Measurement", "Id");
                return NotFound(errorMessage);
            }

            if (_typeRep.GetAll().Any(st => st.Name == sensorTypeModel.Name))
            {
                var errorMessage = _messages.GetMessage(Custom.Conflict, "Sensor Type", "Name");
                return Conflict(errorMessage);
            }

            var sensorType = _mapper.Map<SensorType>(sensorTypeModel);
            _typeRep.Add(sensorType);

            var createdType = _mapper.Map<SensorTypeModelGet>(sensorType);

            return CreatedAtRoute("GetSensorType", new { id = createdType.Id }, createdType);
        }

        [HttpGet,Route("{id:int}", Name = "GetSensorType")]
        public IHttpActionResult Get(int id)
        {
            var sensorType = _typeRep.Get(id);
            if (sensorType == null)
            {
                var errorMessage = _messages.GetMessage(Custom.NotFound, "Sensor Type");
                return NotFound(errorMessage);
            }
            var sensorTypeModel = _mapper.Map<SensorTypeModelGet>(sensorType);

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
                    .Select(st => _mapper.Map<SensorTypeModelGet>(st)).ToList();

            return Ok("GetAllSensorTypes", page, pageSize, pageCount, totalCount, results);
        }

        [HttpPut,Route("{id:int}"),ValidateModel]
        public IHttpActionResult Update(int id, SensorTypeModelUpdate sensorTypeModel)
        {
            if (sensorTypeModel == null)
            {
                var errorMessage = _messages.GetMessage(Generic.NullObject);
                return BadRequest(errorMessage);
            }

            var sensorType = _typeRep.Get(id);
            if (sensorType == null)
            {
                var errorMessage = _messages.GetMessage(Custom.NotFound, "Sensor Type");
                return NotFound(errorMessage);
            }

            if (_typeRep.GetAll().Any(st => st.Name == sensorTypeModel.Name
                && st.Id != id))
            {
                var errorMessage = _messages.GetMessage(Custom.Conflict, "Sensor Type", "Name");
                return Conflict(errorMessage);
            }

            _mapper.Map(sensorTypeModel, sensorType);

            _typeRep.Update(sensorType);

            return StatusCode(HttpStatusCode.NoContent);
        }

        [HttpPatch,Route("{id:int}")]
        public IHttpActionResult PartialUpdate(int id, [FromBody] JsonPatchDocument<SensorTypeModelUpdate> patchDoc)
        {
            if(patchDoc == null)
            {
                var errorMessage = _messages.GetMessage(Generic.NullObject);
                return BadRequest(errorMessage);
            }

            var type = _typeRep.Get(id);
            if (type == null)
            {
                var errorMessage = _messages.GetMessage(Custom.NotFound, "Sensor Type");
                return NotFound(errorMessage);
            }

            var typeModel = _mapper.Map<SensorTypeModelUpdate>(type);
            try
            {
                patchDoc.ApplyTo(typeModel);
            }
            catch (JsonPatchException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }

            Validate(typeModel);

            if (!ModelState.IsValid)
            {
                var error = ModelState.SelectMany(m => m.Value.Errors)
                    .Where(m => m.ErrorMessage != "")
                    .FirstOrDefault();
                var errorMessage = error != null ?
                    error.ErrorMessage : _messages.GetMessage(Generic.InvalidRequest);
                return BadRequest(errorMessage);
            }

            if (_typeRep.GetAll().Any(st => st.Name == typeModel.Name
                && st.Id != id))
            {
                var errorMessage = _messages.GetMessage(Custom.Conflict, "Sensor Type", "Name");
                return Conflict(errorMessage);
            }

            _mapper.Map(typeModel, type);
            _typeRep.Update(type);

            return StatusCode(HttpStatusCode.NoContent);
        }

        [HttpDelete,Route("{id:int}")]
        public void Delete(int id)
        {
            if(_typeRep.Exists(id))
            {
                _typeRep.Delete(id);
            }          
        }
    }
}