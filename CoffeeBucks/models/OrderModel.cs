using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeBucks.models

{
    public class OrderedAddins
    {
        public Guid Id { get; set; }
        public int qty { get; set; }
		public string Name { get; set; }
		public decimal price { get; set; }
    }
    public class OrderedProducts
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int qty { get; set; }
        public decimal price { get; set; }
        public List<OrderedAddins> topings { get; set; }
        
       
    }

    public class TotalProd
    {
        public List<OrderedProducts> prod { get; set; } = new List<OrderedProducts>();
        public decimal total { get; set; }
    }

    public class OrderModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
		public Guid customerId { get; set; }

        public TotalProd product { get; set; } = new TotalProd();

        public string type {  get; set; }
		public int discount { get; set; }
        public decimal net_total { get; set; }
        public Guid orderBy { get; set; }

        public DateTime DateCreated { get; set; }
    }

}
