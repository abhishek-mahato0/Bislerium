using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeBucks.models
{
    public class CustomerModel
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        //public string MembershipType { get; set; }
        public List<Guid> orders { get; set; }
        public int orders_before_complementary { get; set; }

        public Guid createdBy { get; set; }
    }
}
