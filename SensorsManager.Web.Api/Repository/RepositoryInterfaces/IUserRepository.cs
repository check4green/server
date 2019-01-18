using SensorsManager.DomainClasses;
using System.Collections.Generic;

namespace SensorsManager.Web.Api.Repository
{
    public interface IUserRepository
    {
        User GetUser(string email, string password);
        List<User> GetAllUsers();
        User AddUser(User user);
        void UpdateUser(User user);
        void DeleteUser(string email, string password);
    }
}
