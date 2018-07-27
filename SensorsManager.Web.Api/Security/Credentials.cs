using System;
using System.Text;

namespace SensorsManager.Web.Api.Security
{
    public class Credentials
    {
        public string Email { get; private set; }
        public string Password { get; private set; }

        public Credentials(string rawCredentials)
        {
            var encoding = Encoding.GetEncoding("iso-8859-1");
            var credentials = encoding.GetString(Convert.FromBase64String(rawCredentials));
            SetCredentials(credentials);
        }

        private void SetCredentials(string credentials)
        {
            var split = credentials.Split(':');
            Email = split[0];
            Password = split[1];
        }
    }
}