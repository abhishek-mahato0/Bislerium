using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using System.Text.Json;
using CoffeeBucks.models;
using CoffeeBucks.Utils;

namespace CoffeeBucks.Controllers
{
    public class CoffeeController
    {

		private string appDataDirectoryPath = FilePaths.GetAppDirectoryPath();
		private string jsonFilePath = FilePaths.GetJSONFilePath("coffee.json");
		public async Task<CustomMessage> addCoffee( CoffeeModel coffee)
        {
            try
            {

                List<CoffeeModel> coffeeList = await readCoffeeList();
                //searching if the coffee exists
                CoffeeModel exists = coffeeList.FirstOrDefault(x=> x.Name.Contains(coffee.Name, StringComparison.OrdinalIgnoreCase));
                if (exists == null)
                {
					coffeeList.Add(coffee);
					string updatedJson = System.Text.Json.JsonSerializer.Serialize(coffeeList);
					await File.WriteAllTextAsync(jsonFilePath, updatedJson);
					return new CustomMessage
                    {
                        message="Coffee Added.",
                        success=true,
                    };
                }
                else
                {
					return new CustomMessage
					{
						message = "Coffee already exists.",
						success = false,
					};
				}
               
            } catch ( Exception e)
            {
                return new CustomMessage
				{
					message = e.Message,
					success = false,
				};
			}

        }

        public async Task<bool> editCoffee(CoffeeModel updatedCoffee)
        {
            try
            {
                List<CoffeeModel> coffeeList = await readCoffeeList();
                CoffeeModel coffeeToUpdate = coffeeList.Find(c => c.Id == updatedCoffee.Id);

                if (coffeeToUpdate != null)
                {
                    coffeeToUpdate.Name = updatedCoffee.Name;
                    coffeeToUpdate.Price = updatedCoffee.Price;

                    string updatedJson = JsonSerializer.Serialize(coffeeList);
                    await File.WriteAllTextAsync(jsonFilePath, updatedJson);
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

        public async Task<bool> deleteCoffee(Guid coffeeId)
        {
            try
            {
                List<CoffeeModel> coffeeList = await readCoffeeList();
                CoffeeModel coffeeToDelete = coffeeList.Find(c => c.Id == coffeeId);

                if (coffeeToDelete != null)
                {
                    coffeeList.Remove(coffeeToDelete);
                    string updatedJson = JsonSerializer.Serialize(coffeeList);
                    await File.WriteAllTextAsync(jsonFilePath, updatedJson);
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

        //searching coffee by name
		public async Task<List<CoffeeModel>> getCoffee(string searchParameter)
		{
			try
			{
				List<CoffeeModel> coffeeList = await readCoffeeList();
				List<CoffeeModel> coffees = coffeeList.Where(c => c.Name.Contains(searchParameter, StringComparison.OrdinalIgnoreCase))
			.ToList();


				if (coffees.Count > 0)
				{
					return coffees;
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

        //reading the json file
		public async Task<List<CoffeeModel>> readCoffeeList()
        {
			if (!Directory.Exists(appDataDirectoryPath))
			{
				Directory.CreateDirectory(appDataDirectoryPath);
			}
			if (!File.Exists(jsonFilePath))
			{
				return new List<CoffeeModel>();
			}
			string jsonContent = await File.ReadAllTextAsync(jsonFilePath);
            return JsonSerializer.Deserialize<List<CoffeeModel>>(jsonContent);
        }

    }
}
