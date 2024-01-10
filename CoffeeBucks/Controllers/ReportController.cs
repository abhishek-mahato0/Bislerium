using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoffeeBucks.models;

namespace CoffeeBucks.Controllers
{
	public class ReportController
	{
		CustomerControllers customerControllers = new CustomerControllers();
		OrderController orderControllers = new OrderController();
		CoffeeController coffeeController = new CoffeeController();
		AddInsController addInsController = new AddInsController();
		UserController userControllers = new UserController();

		//this function generates top 5 most ordred coffee and addins
		public TopOrderStats generateTopProducts(List<OrderModel> orders)
		{
			try
			{
				if (orders != null || orders.Count > 0)
				{
					var coffeeStats = new Dictionary<string, CoffeeStats>();
					var addinStats = new Dictionary<string, AddinStats>();
					foreach (var order in orders)
					{
						foreach (var product in order.product.prod)
						{
							//updating quantity if exists
							if (coffeeStats.ContainsKey(product.Name))
							{
								coffeeStats[product.Name].Quantity += product.qty;
								coffeeStats[product.Name].TotalRevenue += product.price * product.qty;
							}
							else
							{
								coffeeStats.Add(product.Name, new CoffeeStats
								{
									Name = product.Name,
									Quantity = product.qty,
									TotalRevenue = product.price * product.qty
								});
							}

							foreach (var addin in product.topings)
							{
								if (addinStats.ContainsKey(addin.Name))
								{
									addinStats[addin.Name].Quantity += addin.qty;
									addinStats[addin.Name].TotalRevenue += addin.price * addin.qty;
								}
								else
								{
									addinStats.Add(addin.Name, new AddinStats
									{
										Name = addin.Name,
										Quantity = addin.qty,
										TotalRevenue = addin.price * addin.qty
									});
								}
							}

						}
					}
					var topCoffees = coffeeStats.OrderByDescending(kv => kv.Value.Quantity).Take(5).Select(kv => kv.Value).ToList();
					var topAddins = addinStats.OrderByDescending(kv => kv.Value.Quantity).Take(5).Select(kv => kv.Value).ToList();

					return new TopOrderStats
					{
						TopCoffees = topCoffees,
						TopAddins = topAddins
					};
				}
				return new TopOrderStats();
			}catch(Exception ex)
			{
				return new TopOrderStats();
			}
			

		}

		public async Task<int> getTotalUsers()
		{
			try
			{
				List<UserModel> users = await userControllers.readUserList();
				if (users != null)
				{
					return users.Count;
				}
				else
				{
					return 0;
				}
			}catch (Exception ex)
			{
				return 0;
			}
			
		}
		//total customers count
		public async Task<int> getTotalCustomer()
		{
			try
			{
				List<CustomerModel> users = await customerControllers.readCustomersList();
				if (users != null)
				{
					return users.Count;
				}
				else
				{
					return 0;
				}
			}
			catch (Exception ex)
			{
				return 0;
			}

		}
		public async Task<int> getTotalCoffee()
		{
			try
			{
				List<CoffeeModel> coffee = await coffeeController.readCoffeeList();
				if (coffee != null)
				{
					return coffee.Count;
				}
				else
				{
					return 0;
				}
			}
			catch (Exception ex)
			{
				return 0;
			}

		}
		public async Task<int> getTotalAddins()
		{
			try
			{
				List<AddInsModel> addins = await addInsController.readAddinsList();
				if (addins != null)
				{
					return addins.Count;
				}
				else
				{
					return 0;
				}
			}
			catch (Exception ex)
			{
				return 0;
			}

		}
		public async Task<decimal> getTodaysSales()
		{
			try
			{

				OrderRevenueModel revenue = await orderControllers.getOrderByTimePeriod("all");
				if (revenue != null)
				{
					return revenue.revenue;
				}
				return 0;
			}
			catch (Exception ex)
			{
				return 0;
			}
		}
		public async Task<int> getTodaysOrderCount()
		{
			try
			{
				OrderRevenueModel revenue = await orderControllers.getOrderByTimePeriod("all");
				if (revenue != null)
				{
					return revenue.totalorders;
				}
				return 0;
			}
			catch (Exception ex)
			{
				return 0;
			}
		}
		public async Task<TopOrderStats> getTodayTopOrders()
		{
			try
			{
				OrderRevenueModel revenue = await orderControllers.getOrderByTimePeriod("all");
				if (revenue != null)
				{
					new TopOrderStats
					{
						TopCoffees = revenue.mostOrderedCoffee,
						TopAddins = revenue.mostOrderdAddins
					};
				}
				return new TopOrderStats();
			}
			catch (Exception ex)
			{
				return new TopOrderStats();
			}
		}

	}
}
