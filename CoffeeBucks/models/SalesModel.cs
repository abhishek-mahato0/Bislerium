using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeBucks.models
{
    internal class SalesModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
		public string Type { get; set; }
        public DateTime Date { get; set; }
        public List<Dictionary<string, int>> TopCoffeeTypes { get; set; } = new List<Dictionary<string, int>>();
        public List<Dictionary<string, int>> TopAddIns { get; set; } = new List<Dictionary<string, int>>();
        public decimal Revenue { get; set; }
    }
}
