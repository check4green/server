using SensorsManager.DomainClasses;
using System.Linq;

namespace SensorsManager.Web.Api.Repository
{
    public interface IGatewayRepository
    {
        Gateway Add(Gateway gateway);
        Gateway Get(int id);
        Gateway Get(string address);
        IQueryable<Gateway> GetAll();
        void Update(Gateway gateway);
        void Delete(int id);
        bool Exists(int id);
    }
}
