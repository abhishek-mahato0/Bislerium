using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoffeeBucks.models;
using CoffeeBucks.Utils;

namespace CoffeeBucks.Controllers
{
	public class OrderController
	{
		private string appDataDirectoryPath = FilePaths.GetAppDirectoryPath();
		private string appOrdersFilePath = FilePaths.GetJSONFilePath("orders.json");
		public async Task<CustomMessage> addOrder(OrderModel order)
		{
			try
			{
				List<OrderModel> orderList = await readOrderList();
				orderList.Add(order);
				string updatedJson = System.Text.Json.JsonSerializer.Serialize(orderList);
				await File.WriteAllTextAsync(appOrdersFilePath, updatedJson);
				return new CustomMessage
				{
					message = "Order place successfully.",
					success = true
				};
			}
			catch (Exception ex)
			{
				return new CustomMessage
				{
					message = ex.Message,
					success = false
				};
			}

		}

		public async Task<CustomMessage> deleteOrder(Guid orderId)
		{
			try
			{
				List<OrderModel> orderList = await readOrderList();
				OrderModel orderToDelete = orderList.Find(c => c.Id == orderId);

				if (orderToDelete != null)
				{
					orderList.Remove(orderToDelete);
					string updatedJson = System.Text.Json.JsonSerializer.Serialize(orderList);
					await File.WriteAllTextAsync(appOrdersFilePath, updatedJson);
					return new CustomMessage
					{
						message = "Order deleted successfully.",
						success = true
					};
				}
				else
				{
					return new CustomMessage
					{
						message = "No order found.",
						success = false
					}; // Coffee with the specified ID not found
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				return new CustomMessage
				{
					message = e.Message,
					success = false
				};
			}
		}
		public async Task<OrderRevenueModel> getOrderByTimePeriod(string date)
		{
			try
			{
				List<OrderModel> allOrders = await readOrderList();
				List<OrderModel> filteredOrder = new List<OrderModel>();
				OrderRevenueModel orders = new OrderRevenueModel(); 
				ReportController reportController = new ReportController();
				TopOrderStats topOrderStats = new TopOrderStats();

				//filtering order according to the parameter
				DateTime today = DateTime.Today;
				switch (date.ToLower())
				{
					case "all":
						topOrderStats = reportController.generateTopProducts(allOrders);
						if(topOrderStats != null)
						{
							orders = new OrderRevenueModel
							{
								selectedDate = "All",
								mostOrderedCoffee = topOrderStats.TopCoffees,
								mostOrderdAddins = topOrderStats.TopAddins,
								totalorders = allOrders.Count,
								revenue = allOrders.Sum(o => o.net_total),
								filtorders = allOrders
							};
						}
						return orders;


					case "today":
						filteredOrder =allOrders.Where(order => order.DateCreated.Date == today).ToList();
						topOrderStats = reportController.generateTopProducts(filteredOrder);
						if (topOrderStats != null)
						{
							orders = new OrderRevenueModel
							{
								selectedDate = "Today",
								mostOrderedCoffee = topOrderStats.TopCoffees,
								mostOrderdAddins = topOrderStats.TopAddins,
								totalorders = filteredOrder.Count,
								revenue = filteredOrder.Where(or=> or.type !="complement").Sum(o => o.net_total),
								filtorders = filteredOrder
							};
						}
						return orders;

					case "week":
						filteredOrder = allOrders.Where(order => order.DateCreated >= today.AddDays(-6)).ToList();
						topOrderStats = reportController.generateTopProducts(filteredOrder);
						if (topOrderStats != null)
						{
							orders = new OrderRevenueModel
							{
								selectedDate = "This Week",
								mostOrderedCoffee = topOrderStats.TopCoffees,
								mostOrderdAddins = topOrderStats.TopAddins,
								totalorders = filteredOrder.Count,
								revenue = filteredOrder.Where(or=> or.type !="complement").Sum(o => o.net_total),
								filtorders = filteredOrder
							};
						}
						return orders;

					case "month":
						filteredOrder= allOrders.Where(order => order.DateCreated.Month == today.Month && order.DateCreated.Year == today.Year).ToList();
						topOrderStats = reportController.generateTopProducts(filteredOrder);
						if (topOrderStats != null)
						{
							orders = new OrderRevenueModel
							{
								selectedDate = "This Month",
								mostOrderedCoffee = topOrderStats.TopCoffees,
								mostOrderdAddins = topOrderStats.TopAddins,
								totalorders = filteredOrder.Count,
								revenue = filteredOrder.Where(or => or.type != "complement").Sum(o => o.net_total),
								filtorders = filteredOrder
							};
						}
						return orders;

					default:
						throw new ArgumentException("Invalid time period specified.");
				}
			}
			catch (Exception e)
			{
				return null;
			}
		}

		//filtering order according to the custom date range
		public async Task<OrderRevenueModel> getOrdersByMonth(DateTime from, DateTime to)
		{
			List<OrderModel> allOrders = await readOrderList();
            OrderRevenueModel orders = new OrderRevenueModel();
            ReportController reportController = new ReportController();
            TopOrderStats topOrderStats = new TopOrderStats();
            List<OrderModel> filteredOrder = allOrders.Where(order => order.DateCreated >= from && order.DateCreated <= to).ToList();
			topOrderStats = reportController.generateTopProducts(filteredOrder);
            if (topOrderStats != null)
            {
                orders = new OrderRevenueModel
                {
                    selectedDate = from.Date.ToString()+ " to " + to.Date.ToString(),
                    mostOrderedCoffee = topOrderStats.TopCoffees,
                    mostOrderdAddins = topOrderStats.TopAddins,
                    totalorders = filteredOrder.Count,
                    revenue = filteredOrder.Where(or => or.type != "complement").Sum(o => o.net_total),
                    filtorders = filteredOrder
                };
            }
            return orders;
        }

		//customer summary
		public async Task<CustomerSummary> findMostActiveCustomer()
		{
			try
			{
				List<OrderModel> Orders = await readOrderList();
				if (Orders != null)
				{
					var customerSummary = Orders
						.GroupBy(order => order.customerId)
						.Select(group => new CustomerSummary
						{
							CustomerId = group.Key,
							CustomerName = new CustomerControllers().getCustomerById(group.Key), // Assuming CustomerName is a property in OrderModel
							TotalOrders = group.Count(),
							TotalCashPaid = group.Sum(order => order.net_total)
						})
						.OrderByDescending(summary => summary.TotalOrders)
						.FirstOrDefault();

					return customerSummary;
				}
				else
				{
					return null;
				}
			}
			catch (Exception ex)
			{
				return new CustomerSummary();
			}
		}

		//check if the customer has purchased daily and is eligible for 10% discount.
		public async Task<bool> isEligibleForDiscount(Guid customerId)
		{
			DateTime currentDate = DateTime.Now;
			DateTime firstDayOfCurrentMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
			DateTime lastMonthEnd = firstDayOfCurrentMonth.AddDays(-1);
			DateTime lastMonthStart = new DateTime(lastMonthEnd.Year, lastMonthEnd.Month, 1);

			List<OrderModel> totalOrders = await new OrderController().readOrderList();
			//the weekend is sunday here.
			List<DateTime> filteredOrders = totalOrders.Where(o => o.customerId == customerId && o.DateCreated >= lastMonthStart && o.DateCreated <= lastMonthEnd && o.DateCreated.DayOfWeek != DayOfWeek.Sunday).Select(i => i.DateCreated.Date).ToList();

			List<DateTime> allDates = Enumerable.Range(0, (int)(lastMonthEnd - lastMonthStart).TotalDays + 1)
												.Select(i => lastMonthStart.AddDays(i)).Where(date => date.DayOfWeek != DayOfWeek.Sunday).ToList();

			List<DateTime> missingDates = allDates.Except(filteredOrders).ToList();

			return missingDates.Count>0 ? false : true;
		}

		//check if the total orders count is 10 and provide free complemrntary coffee

		public async Task<List<OrderModel>> readOrderList()
		{
			if (!Directory.Exists(appDataDirectoryPath))
			{
				Directory.CreateDirectory(appDataDirectoryPath);
			}
			if (!File.Exists(appOrdersFilePath))
			{
				return new List<OrderModel>();
			}
			
			string jsonContent = await File.ReadAllTextAsync(appOrdersFilePath);
			
			return System.Text.Json.JsonSerializer.Deserialize<List<OrderModel>>(jsonContent);
			
		}
	}

}
