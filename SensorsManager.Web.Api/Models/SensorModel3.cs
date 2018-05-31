using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SensorsManager.Web.Api.Repository.Models
{
    public class SensorModel3
    {
        public int SensorTypeId { get; set; }
        public DateTime ProductionDate { get; set; }
        public int UploadInterval { get; set; }
        public int BatchSize { get; set; }
        public string GatewayAddress { get; set; }
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