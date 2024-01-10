using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeBucks.models
{
	public class MostOrderedModel
	{
		public string CoffeeName { get; set; }
		public int CoffeeQty { get; set; }
		public string AddinName { get; set; }
		public int AddinQty { get; set; }
	}
	public class CustomerSummary
	{
		public Guid CustomerId { get; set; }
		public string CustomerName { get; set; }
		public int TotalOrders { get; set; }
		public decimal TotalCashPaid { get; set; }
	}

	public class CoffeeStats
	{
		public string Name { get; set; }
		public int Quantity { get; set; }
		public decimal TotalRevenue { get; set; }
	}

	public class AddinStats
	{
		public string Name { get; set; }
		public int Quantity { get; set; }
		public decimal TotalRevenue { get; set; }
	}

	public class TopOrderStats
	{
		public List<CoffeeStats> TopCoffees { get; set; }
		public List<AddinStats> TopAddins { get; set; }
	}

	public class OrderRevenueModel
	{
		public string selectedDate { get; set; }
		public decimal revenue { get; set; }

		public List<CoffeeStats> mostOrderedCoffee { get; set; }
		public List<AddinStats> mostOrderdAddins { get; set; }

		public int totalorders { get; set; }
		public List<OrderModel> filtorders { get; set; }
	};
}
