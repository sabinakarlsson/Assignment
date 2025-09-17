using ITSystem.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITSystem.Repositories.Interface
{
    public interface IIncidentRepository
    {
        Task AddIncidentAsync(Incident incident);

        Task<List<Incident>> GetAllIncidentsAsync();
    }
}
