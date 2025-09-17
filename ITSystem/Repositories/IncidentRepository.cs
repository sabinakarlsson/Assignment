using ITSystem.Data;
using ITSystem.Repositories.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITSystem.Repositories
{
    public class IncidentRepository : IIncidentRepository
    {
        private readonly ShopDbContext _context;
        public IncidentRepository(ShopDbContext context)
        {
            _context = context;
        }

        public async Task AddIncidentAsync(Incident incident)
        {
            _context.Incidents.Add(incident);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Incident>> GetAllIncidentsAsync()
        {
            return await _context.Incidents.ToListAsync();
        }
    }
}
