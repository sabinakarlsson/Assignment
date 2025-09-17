using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITSystem.Data
{
    public class Order
    {
        public int Id { get; set; }
        public string CompanyId { get; set; } = null!;
        public string CompanyName { get; set; } = null!;
        public DateTime OrderDate { get; set; } 
        public decimal TotalAmount { get; set; }
        public bool SentToOT { get; set; } = false;
    }
}
