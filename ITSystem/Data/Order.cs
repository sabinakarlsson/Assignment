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
        public int CustomerId { get; set; }
        public string CompanyName { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal TotalAmount { get; set; }
        public bool SentToOT { get; set; } = false;
    }
}
