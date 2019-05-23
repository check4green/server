using SensorsManager.DataLayer;
using SensorsManager.DomainClasses;
using System.Data.Entity;
using System.Linq;

namespace SensorsManager.Web.Api.Repository
{
    public class UserRepository : IUserRepository
    {
       public User Get(string email, string password)
        {
            using (DataContext db = new DataContext())
            {
                var user = db.Users
                    .SingleOrDefault(
                    u => u.Email == email 
                    && u.Password == password);
                return user;
            }
        }

        public IQueryable<User> GetAll()
        {
            using (DataContext db = new DataContext())
            {
                return db.Users.ToList().AsQueryable();
            }
        }

        public User Add(User user)
        {
            using (DataContext db = new DataContext())
            {
                var newUser = db.Users.Add(user);
                db.SaveChanges();
                return newUser;
            }
        }

       public void Update(User user)
        {
            using (DataContext db = new DataContext())
            {
                db.Entry(user).State = EntityState.Modified;
                db.SaveChanges();
            }
        }
    }
}