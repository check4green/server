using SensorsManager.DomainClasses;
using System.Linq;

namespace SensorsManager.Web.Api.Repository
{
    public interface IUserRepository
    {
        User Get(string email, string password);
        IQueryable<User> GetAll();
        User Add(User user);
        void Update(User user);
    }
}
