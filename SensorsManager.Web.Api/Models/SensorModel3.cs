using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace SensorsManager.Web.Api.Repository.Models
{
    public class SensorModel3
    {
        [Required]
        public int SensorTypeId { get; set; }
        [Required]
        public DateTime ProductionDate { get; set; }
        [Required]
        public int UploadInterval { get; set; }
        [Required]
        public int BatchSize { get; set; }
        [Required]
        public string GatewayAddress { get; set; }
        [Required]
        public string ClientAddress { get; set; }

        public bool AddressValidation (string address)
        {
            if(address.Length == 4)
            {
                if(address.Substring(0, 2) == "0x")
                {
                    if (int.TryParse(address.Substring(2, 2),
                             System.Globalization.NumberStyles.HexNumber,
                             System.Globalization.CultureInfo.InvariantCulture, out int res))
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
          
        }
    }
}