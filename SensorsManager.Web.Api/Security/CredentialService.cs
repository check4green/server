
using System;
using System.Text;

namespace SensorsManager.Web.Api.Security
{
    public class CredentialService : ICredentialService
    {
        public string Email { get; private set; }
        public string Password { get; private set; }

        public void SetCredentials(string rawCredentials)
        {
            var encoding = Encoding.GetEncoding("iso-8859-1");
            var credentials = encoding.GetString(Convert.FromBase64String(rawCredentials));
            var split = credentials.Split(':');
            Email = split[0];
            Password = split[1];
        }
    }
}