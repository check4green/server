using SensorsManager.DataLayer;
using SensorsManager.DomainClasses;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace SensorsManager.Web.Api.Repository
{
    public class UserRepository
    {
       public User GetUser(string email, string password)
        {
            using (DataContext db = new DataContext())
            {
                var user = db.Database.SqlQuery<User>(
                    "SELECT Id,FirstName,LastName,Email,Password," +
                    "CompanyName,Country,PhoneNumber" +
                    " FROM dbo.Users " +
                    "WHERE Email = @email" +
                    " AND Password = @password",
                    new SqlParameter("email", email),
                    new SqlParameter("password", password))
                    .SingleOrDefault();
                return user;
            }
        }

        public List<User> GetAllUsers()
        {
  
            using (DataContext db = new DataContext())
            {

               var users = db.Database.SqlQuery<User>(
                "SELECT Id,FirstName,LastName,Email,Password," +
                "CompanyName,Country,PhoneNumber" +
                " FROM dbo.Users ").ToList();
                if(users.Any() == false)
                {
                    return null;
                }
                else
                {
                    return users;
                }
            }
        }

        public User AddUser(User user)
        {
            using (DataContext db = new DataContext())
            {
                db.Database.ExecuteSqlCommand(
               "INSERT INTO dbo.Users" +
               "(FirstName,LastName,Email,Password,CompanyName,Country,PhoneNumber)" +
               "VALUES(@firstName,@lastName,@email,@password,@companyName,@country,@phoneNumber)",
               new SqlParameter("firstName", user.FirstName),
               new SqlParameter("lastName", user.LastName),
               new SqlParameter("email", user.Email),
               new SqlParameter("password", user.Password),
               new SqlParameter("companyName", user.CompanyName),
               new SqlParameter("country", user.Country),
               new SqlParameter("phoneNumber", user.PhoneNumber)
               );
                var newUser = GetUser(user.Email, user.Password);
                return newUser;
            }
        }

       public void UpdateUser(User user)
        {
            using (DataContext db = new DataContext())
            {
                db.Database.ExecuteSqlCommand(
               "UPDATE dbo.Users " +
               "SET " +
               "FirstName = @firstName," +
               "LastName = @lastName," +
               "Email = @email," +
               "Password = @password," +
               "CompanyName = @companyName," +
               "Country = @country," +
               "PhoneNumber = @phoneNumber," +
               " WHERE Id = @id",
               new SqlParameter("firstName", user.FirstName),
               new SqlParameter("lastName", user.LastName),
               new SqlParameter("email", user.Email),
               new SqlParameter("password", user.Password),
               new SqlParameter("companyName", user.CompanyName),
               new SqlParameter("country", user.Country),
               new SqlParameter("phoneNumber", user.PhoneNumber),
               new SqlParameter("id", user.Id)
               );
   
            }
        }

       public void DeleteUser(string email, string password)
       {
            using(DataContext db = new DataContext())
            {
                db.Database.ExecuteSqlCommand(
                    "DELETE FROM dbo.Users " +
                    "WHERE Email = @email " +
                    "AND Password = @password",
                    new SqlParameter("email", email),
                    new SqlParameter("password", password));
            }
       }
    }
}