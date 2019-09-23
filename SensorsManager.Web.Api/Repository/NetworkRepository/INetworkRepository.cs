using SensorsManager.DomainClasses;
using System.Linq;

namespace SensorsManager.Web.Api.Repository
{
    public interface INetworkRepository
    {
        Network Add(Network network);
        Network Get(int id);
        IQueryable<Network> GetAll();
        void Update(Network network);
        void Delete(int id);
        bool Exists(int id);
    }
}
