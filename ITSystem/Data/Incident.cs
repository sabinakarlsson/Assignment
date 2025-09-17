using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITSystem.Data
{
    public class Incident
    {
        public int Id { get; set; }
        public DateTime IncidentTimestamp { get; set; } = DateTime.Now;
        public string Type { get; set; } = null!;
        public string Message { get; set; } = null!;
        public string? Username { get; set; }
    }
}
