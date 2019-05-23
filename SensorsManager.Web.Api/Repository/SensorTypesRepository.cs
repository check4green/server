﻿using SensorsManager.DataLayer;
using SensorsManager.DomainClasses;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace SensorsManager.Web.Api.Repository
{
    public class SensorTypesRepository : ISensorTypesRepository
    {
        public SensorType Add(SensorType sensorType)
        {
            using (DataContext db = new DataContext())
            {
                var res = db.SensorsTypes.Add(sensorType);
                db.SaveChanges();
                return res;
            }
        }

        public SensorType Get(int id)
        {
            using (DataContext db = new DataContext())
            {
                return db.SensorsTypes.Include(p => p.Measurement).Where(p => p.Id == id).SingleOrDefault();
            }
        }

        public IQueryable<SensorType> GetAll()
        {
            using (DataContext db = new DataContext())
            {
                return db.SensorsTypes.ToList().AsQueryable();
            }
        }

        public void Update(SensorType sensorType)
        {
            using (DataContext db = new DataContext())
            {
                db.Entry(sensorType).State = EntityState.Modified;
                db.SaveChanges();
            }
        }

        public void Delete(int id)
        {
            using (DataContext db = new DataContext())
            {
                var sensorType = new SensorType() { Id = id };
                bool saveFailed;
                do
                {
                    saveFailed = false;
                    db.Entry(sensorType).State = EntityState.Deleted;
                    try
                    {

                        db.SaveChanges();
                    }

                    catch (DbUpdateConcurrencyException ex)
                    {
                        saveFailed = true;
                        var entry = ex.Entries.Single();
                        if (entry.State == EntityState.Deleted)
                        {
                            entry.State = EntityState.Detached;
                        }
                        else
                        {
                            entry.OriginalValues.SetValues(entry.GetDatabaseValues());
                        }
                    }

                } while (saveFailed);
            }
        }
    }
}