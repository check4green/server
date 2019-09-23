namespace SensorsManager.Web.Api.Security
{
    public interface IUserLogIn
    {
        bool LogIn(string email, string password);
    }
}
