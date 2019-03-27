using SensorsManager.Web.Api.Repository;

namespace SensorsManager.Web.Api.Security
{
    public class UserLogIn
    {
        public static bool LogIn(string email, string password)
        {
            var userRep = new UserRepository();
            var user = userRep.GetUser(email, password);
            if (user != null)
            {
                return true;
            }
            return false;
        }

    }
}
