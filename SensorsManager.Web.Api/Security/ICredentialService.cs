using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SensorsManager.Web.Api.Security
{
    public interface ICredentialService
    {
        string Email { get; }
        string Password { get; }

        void SetCredentials(string rawCredentials);
    }
}
