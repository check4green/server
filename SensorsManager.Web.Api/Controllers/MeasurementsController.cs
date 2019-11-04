using System;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Cors;
using AutoMapper;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.JsonPatch.Exceptions;
using SensorsManager.DomainClasses;
using SensorsManager.Web.Api.Models;
using SensorsManager.Web.Api.Repository;
using SensorsManager.Web.Api.Services;
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
        private readonly IMeasurementRepository _measureRep;
        private readonly IMapper _mapper;
        private readonly IMessageService _messages;

        public MeasurementsController(IMeasurementRepository measureRep, IMapper mapper, IMessageService messages)
        {
            _measureRep = measureRep;
            _mapper = mapper;
            _messages = messages;
        }

        [HttpPost,Route(""),ValidateModel]
        public IHttpActionResult Add(MeasurementModel measureModel)
        {
            if (measureModel == null)
            {
                var errorMessage = _messages.GetMessage(Generic.NullObject);
                return BadRequest(errorMessage);
            }

            if (_measureRep.GetAll().Any(m => m.UnitOfMeasure == measureModel.UnitOfMeasure))
            {
                var errorMessage = _messages.GetMessage(Custom.Conflict, "Measurement", "Unit Of Measure");
                return Conflict(errorMessage);
            }

            var newMeasure = _mapper.Map<Measurement>(measureModel);
            _measureRep.Add(newMeasure);


            var createdMeasure = _mapper.Map<MeasurementModelGet>(newMeasure);

            return CreatedAtRoute("GetMeasurement",
                new { id = newMeasure.Id }, createdMeasure);
        }

        [HttpGet,Route("{id:int}", Name = "GetMeasurement")]
        public IHttpActionResult Get(int id)
        {
            var measurement = _measureRep.Get(id);

            if (measurement == null)
            {
                var errorMessage = _messages.GetMessage(Custom.NotFound, "Measurement");
                return NotFound(errorMessage);
            }

            var measurementModel = _mapper.Map<MeasurementModelGet>(measurement);

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
                 .Take(pageSize).Select(m => _mapper.Map<MeasurementModelGet>(m))
                 .OrderBy(m => m.Id).ToList();

            return Ok("GetAllMeasurements", page, pageSize, pageCount, totalCount, results);

        }

        [HttpPut,Route("{id:int}"), ValidateModel]
        public IHttpActionResult Update(int id, MeasurementModel measureModel)
        {
            if (measureModel == null)
            {
                var errorMessage = _messages.GetMessage(Generic.NullObject);
                return BadRequest(errorMessage);
            }

            if (_measureRep.GetAll().Any(m => m.UnitOfMeasure == measureModel.UnitOfMeasure
                && m.Id != id))
            {
                var errorMessage = _messages.GetMessage(Custom.Conflict, "Measurement", "Unit Of Measure");
                return Conflict(errorMessage);
            }

            var measurement = _measureRep.Get(id);
            if (measurement == null)
            {
                var errorMessage = _messages.GetMessage(Custom.NotFound, "Measurement");
                return NotFound(errorMessage);
            }

            _mapper.Map(measureModel, measurement);
            _measureRep.Update(measurement);

            return StatusCode(HttpStatusCode.NoContent);
        }

        [HttpPatch, Route("{id:int}")]
        public IHttpActionResult PartialUpdate(int id, [FromBody] JsonPatchDocument<MeasurementModel> patchDoc)
        {
            if(patchDoc == null)
            {
                var errorMessage = _messages.GetMessage(Generic.NullObject);
                return BadRequest(errorMessage);
            }

            var measure = _measureRep.Get(id);
            if(measure == null)
            {
                var errorMessage = _messages.GetMessage(Custom.NotFound, "Measurement");
                return NotFound(errorMessage);
            }

            var measureModel = _mapper.Map<MeasurementModel>(measure);

            try
            {
                patchDoc.ApplyTo(measureModel);
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

            Validate(measureModel);
            if (!ModelState.IsValid)
            {
                var error = ModelState.SelectMany(m => m.Value.Errors)
                    .Where(m => m.ErrorMessage != "")
                    .FirstOrDefault();
                var errorMessage = error != null ? 
                    error.ErrorMessage : _messages.GetMessage(Generic.InvalidRequest);
                return BadRequest(errorMessage);
            }

            if (_measureRep.GetAll().Any(m => m.UnitOfMeasure == measureModel.UnitOfMeasure
                && m.Id != id))
            {
                var errorMessage = _messages.GetMessage(Custom.Conflict, "Measurement", "Unit Of Measure");
                return Conflict(errorMessage);
            }

            _mapper.Map(measureModel, measure);
            _measureRep.Update(measure);


            return StatusCode(HttpStatusCode.NoContent);
        }

        [HttpDelete,Route("{id:int}")]
        public void Delete(int id)
        {
            if (_measureRep.Exists(id))
            {
                _measureRep.Delete(id);
            }
        }
    }
}