using SensorsManager.DataLayer;
using SensorsManager.DomainClasses;
using System.Data.Entity;
using System.Linq;

namespace SensorsManager.Web.Api.Repository
{
    public class UserRepository : IUserRepository
    {
        DataContext db;
        public UserRepository(DataContext dataContext)
        {
            db = dataContext;
        }
       public User Get(string email, string password)
        {
            var user = db.Users
                .SingleOrDefault(
                u => u.Email == email 
                && u.Password == password);
            return user;
        }

        public IQueryable<User> GetAll()
        {
            return db.Users.ToList().AsQueryable();
        }

        public User Add(User user)
        {
            var newUser = db.Users.Add(user);
            db.SaveChanges();
            return newUser;
        }

       public void Update(User user)
        {
            db.Entry(user).State = EntityState.Modified;
            db.SaveChanges();
        }

        public bool Exists(int id)
        {
            return db.Users.Any(u => u.Id == id);
        }
    }
}