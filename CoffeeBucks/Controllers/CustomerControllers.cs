using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CoffeeBucks.models;
using CoffeeBucks.Utils;

namespace CoffeeBucks.Controllers
{
    public class CustomerControllers {
		private string appDataDirectoryPath = FilePaths.GetAppDirectoryPath();
		private string appFilePath = FilePaths.GetJSONFilePath("customers.json");
		public async Task<CustomMessage> addCustomers(CustomerModel customer)
    {
        try
        {
            List<CustomerModel> customerList = await readCustomersList();
            List<CustomerModel> customers = customerList.FindAll(c =>
           c.PhoneNumber.Contains(customer.PhoneNumber, StringComparison.OrdinalIgnoreCase));
                if(customers.Count !=0) {
                    return new CustomMessage {
                        message = "User already exists.",
                        success = false
                    };
            }else
            {

				customerList.Add(customer);
				string updatedJson = System.Text.Json.JsonSerializer.Serialize(customerList);
				await File.WriteAllTextAsync(appFilePath, updatedJson);
					return new CustomMessage
					{
						message = "User added successfully.",
						success = true
					};
				}

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

    public async Task<bool> editCustomer(CustomerModel updatedUser)
    {
        try
        {
            List<CustomerModel> userList = await readCustomersList();
            CustomerModel userToUpdate = userList.Find(c => c.Id == updatedUser.Id);

            if (userToUpdate != null)
            {
                userToUpdate.Name = updatedUser.Name;
                userToUpdate.PhoneNumber = updatedUser.PhoneNumber;

                string updatedJson = System.Text.Json.JsonSerializer.Serialize(userList);
                await File.WriteAllTextAsync(appFilePath, updatedJson);
                return true;
            }
            else
            {
                return false; // Coffee with the specified ID not found
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }
        //adding order id to the custoerm order list
        public async Task<CustomMessage> addOrders(Guid CustomerId, Guid orderId)
        {
            try
            {
                List<CustomerModel> customerList = await readCustomersList();
                CustomerModel orderToUpdate = customerList.Find(c => c.Id == CustomerId);

                if (orderToUpdate != null)
                {
                    orderToUpdate.orders.Add(orderId); //adding order id

                    string updatedJson = System.Text.Json.JsonSerializer.Serialize(customerList);
                    await File.WriteAllTextAsync(appFilePath, updatedJson);
                    return new CustomMessage { message = "Order place successfully.", success = true };
                }
                else
                {
                    return new CustomMessage
                    {
                        success = false,
                        message="No customer found"
                    }; // Customer with the specified ID not found
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
				return new CustomMessage
				{
					success = false,
					message = e.Message
				};
			}
        }

        //increase the order_before_com o fthe customer for providing the free coffee after 10 orders
		public async Task<CustomMessage> increaseCompOrders(Guid CustomerId)
		{
			try
			{
				List<CustomerModel> customerList = await readCustomersList();
				CustomerModel orderToUpdate = customerList.Find(c => c.Id == CustomerId);

				if (orderToUpdate != null)
				{
                    if (orderToUpdate.orders_before_complementary > 10)
                    {
                        orderToUpdate.orders_before_complementary = orderToUpdate.orders_before_complementary - 9; //reseting the order count
                    }
                    else
                    {
                        orderToUpdate.orders_before_complementary = orderToUpdate.orders_before_complementary + 1;
                    }

					string updatedJson = System.Text.Json.JsonSerializer.Serialize(customerList);
					await File.WriteAllTextAsync(appFilePath, updatedJson);
					return new CustomMessage { message = "Order increased.", success = true };
				}
				else
				{
					return new CustomMessage
					{
						success = false,
						message = "No customer found"
					}; // Coffee with the specified ID not found
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				return new CustomMessage
				{
					success = false,
					message = e.Message
				};
			}
		}

		public string getCustomerById(Guid userId)
		{
			try
			{
				var json = File.ReadAllText(appFilePath);
                List<CustomerModel> userList = JsonSerializer.Deserialize<List<CustomerModel>>(json); 
				CustomerModel userToshow = userList.Find(c => c.Id == userId);

				if (userToshow != null)
				{
					return userToshow.Name;
				}
				else
				{
					return null; // Coffee with the specified ID not found
				}
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				return null;
			}
		}

		public async Task<bool> deleteCustomer(Guid userId)
    {
        try
        {
            List<CustomerModel> userList = await readCustomersList();
            CustomerModel userToDelete = userList.Find(c => c.Id == userId);

            if (userToDelete != null)
            {
                userList.Remove(userToDelete);
                string updatedJson = System.Text.Json.JsonSerializer.Serialize(userList);
                await File.WriteAllTextAsync(appFilePath, updatedJson);
                return true;
            }
            else
            {
                return false; // Coffee with the specified ID not found
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return false;
        }
    }
        public List<CustomerModel> getCustomer(string parameter)
        {
            try
            {
				var json = File.ReadAllText(appFilePath);
				List<CustomerModel> customerList = JsonSerializer.Deserialize<List<CustomerModel>>(json); ;
				List<CustomerModel> customers = customerList.FindAll(c =>
           c.Name.Contains(parameter, StringComparison.OrdinalIgnoreCase) || // Case-insensitive name search
           c.PhoneNumber.Contains(parameter, StringComparison.OrdinalIgnoreCase) // Case-insensitive phone search
       );


                if (customers.Count >0)
                {
                    return customers;
                }
                else
                {
                    return null; // Coffee with the specified ID not found
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }

		public async Task<bool> isEligibleForComplimentary(Guid customerId)
		{
			List<CustomerModel> customers = await readCustomersList();
            CustomerModel customer = customers.FirstOrDefault(c => c.Id == customerId);
			return (customer.orders_before_complementary == 10); // There are 6 days in a week excluding Saturday
		}

		public async Task<List<CustomerModel>> readCustomersList()
    {
			if (!Directory.Exists(appDataDirectoryPath))
			{
				Directory.CreateDirectory(appDataDirectoryPath);
			}
			if (!File.Exists(appFilePath))
			{
				return new List<CustomerModel>();
			}
			string jsonContent = await File.ReadAllTextAsync(appFilePath);
        return System.Text.Json.JsonSerializer.Deserialize<List<CustomerModel>>(jsonContent);
    }
}
}
