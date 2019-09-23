using SensorsManager.Web.Api.Repository;
using System.Web.Http;

namespace SensorsManager.Web.Api.Security
{
    public class UserLogIn : IUserLogIn
    {
        private IUserRepository _userRep => (IUserRepository)GlobalConfiguration.Configuration.DependencyResolver.GetService(typeof(IUserRepository));
    
        public bool LogIn(string email, string password)
        {
            var user = _userRep.Get(email, password);
            if (user != null)
            {
                return true;
            }
            return false;
        }

    }
}
