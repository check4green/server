using SensorsManager.DomainClasses;
using SensorsManager.Web.Api.Pending;
using System;


namespace SensorsManager.Web.Api.Models
{
    public class ModelMapper
    {
        public void MapToEntity(
          SensorPendingModel sensorPendingModel, Sensor sensor)
        {
            sensor.UploadInterval = sensorPendingModel.UploadInterval;
        }

        public void MapToEntity(SensorModelPut sensorModel, Sensor sensor)
        {
            sensor.Name = sensorModel.Name;
            sensor.UploadInterval = sensorModel.UploadInterval;
            sensor.Latitude = sensorModel.Latitude;
            sensor.Longitude = sensorModel.Longitude;
        }

        public Sensor MapToEntity(SensorModelPost sensorModel)
        {
            return new Sensor
            {
                Name = sensorModel.Name,
                SensorType_Id = sensorModel.SensorTypeId,
                Network_Id = sensorModel.NetworkId,
                ProductionDate = sensorModel.ProductionDate,
                UploadInterval = sensorModel.UploadInterval,
                Address = sensorModel.Address,
                Latitude = sensorModel.Latitude,
                Longitude = sensorModel.Longitude
            };
        }

        public SensorReading MapToEntity(SensorReadingModelPost sensorReadingModel, int sensorId)
        {
            return new SensorReading
            {
                Sensor_Id = sensorId,
                GatewayAddress = sensorReadingModel.GatewayAddress,
                Value = sensorReadingModel.Value,
                ReadingDate = sensorReadingModel.ReadingDate,
                InsertDate = DateTime.UtcNow
            };
        }

        public Measurement MapToEntity(MeasurementModel measurementModel)
        {
            return new Measurement
            {
                Description = measurementModel.Description,
                UnitOfMeasure = measurementModel.UnitOfMeasure
            };
        }

        public void MapToEntity(MeasurementModel measurementModel, Measurement measurement)
        {
            measurement.Description = measurementModel.Description;
            measurement.UnitOfMeasure = measurementModel.UnitOfMeasure;
        }

        public SensorType MapToEntity(SensorTypeModel sensorTypeModel)
        {
            return new SensorType
            {
                Name = sensorTypeModel.Name,
                Description = sensorTypeModel.Description,
                MinValue = sensorTypeModel.MinValue,
                MaxValue = sensorTypeModel.MaxValue,
                Measure_Id = sensorTypeModel.MeasureId,
            };
        }

        public void MapToEntity(SensorTypeModel sensorTypeModel, SensorType sensorType)
        {
            sensorType.Name = sensorTypeModel.Name;
            sensorType.Description = sensorTypeModel.Description;
            sensorType.MinValue = sensorTypeModel.MinValue;
            sensorType.MaxValue = sensorTypeModel.MaxValue;
            sensorType.Measure_Id = sensorTypeModel.MeasureId;
        }

        public User MapToEntity(UserModel userModel)
        {
            return new User
            {
                FirstName = userModel.FirstName,
                LastName = userModel.LastName,
                Email = userModel.Email,
                Password = userModel.Password,
                CompanyName = userModel.CompanyName ?? "",
                Country = userModel.Country,
                PhoneNumber = userModel.PhoneNumber
            };
        }

        public void MapToEntity(UserModel userModel, User result)
        {
            result.FirstName = userModel.FirstName;
            result.LastName = userModel.LastName;
            result.Email = userModel.Email;
            result.Password = userModel.Password;
            result.CompanyName = userModel.CompanyName ?? "";
            result.Country = userModel.Country;
            result.PhoneNumber = userModel.PhoneNumber;
        }

        public Network MapToEntity(NetworkModelPost networkModel)
        {
            return new Network
            {
                Name = networkModel.Name,
                Address = networkModel.Address,
                User_Id = networkModel.User_Id
            };
        }

        public void MapToEntity(NetworkModelPut networkModel, Network network)
        {
            network.Name = networkModel.Name;
        }

        public Gateway MapToEntity(GatewayModelPost gatewayModel)
        {
            return new Gateway
            {
                Name = gatewayModel.Name,
                Address = gatewayModel.Address,
                Network_Id = gatewayModel.Network_Id,
                Latitude = gatewayModel.Latitude,
                Longitude = gatewayModel.Longitude,
                UploadInterval = gatewayModel.UploadInterval
            };
        }

        public void MapToEntity(GatewayModelPut gatewayModel, Gateway gateway)
        {
            gateway.Name = gatewayModel.Name;
            gateway.Latitude = gatewayModel.Latitude;
            gateway.Longitude = gatewayModel.Longitude;
        }
    }
}