﻿using System;

namespace SensorsManager.Web.Api.Models
{
    public class SensorModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int SensorTypeId { get; set; }
        public DateTime ProductionDate { get; set; }
        public int UploadInterval { get; set; }
        public int BatchSize { get; set; }
        public string GatewayAddress { get; set; }
        public string ClientAddress { get; set; }
        public bool Active { get; set; }
    }
}