using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using CoffeeBucks.models;
using CoffeeBucks.Utils;

namespace CoffeeBucks.Controllers
{
	public class AddInsController
	{


		private string appDataDirectoryPath = FilePaths.GetAppDirectoryPath();
		private string jsonFilePath = FilePaths.GetJSONFilePath("addins.json");
		public async Task<CustomMessage> addAddins(AddInsModel addin)
		{
			try
			{

				List<AddInsModel> addinsList = await readAddinsList();
				//checking if the addin already exists
				AddInsModel exists = addinsList.FirstOrDefault(x=> x.Name.Contains(addin.Name, StringComparison.OrdinalIgnoreCase)); 
				if (exists == null)
				{
					addinsList.Add(addin);
					string updatedJson = System.Text.Json.JsonSerializer.Serialize(addinsList);
					await File.WriteAllTextAsync(jsonFilePath, updatedJson);
					return new CustomMessage
					{
						success = true,
						message = "Addins added."
					};
				}
				else
				{
					return new CustomMessage
					{
						success = false,
						message = "Addins already exists."
					};
				}
				
			}
			catch (Exception e)
			{
				return new CustomMessage
				{
					success = false,
					message = e.Message
				};
			}

		}
		public async Task<List<AddInsModel>> getAddin(string searchParameter)
		{
			try
			{
				List<AddInsModel> addinList = await readAddinsList();
				List<AddInsModel> addin = addinList.Where(c => c.Name.Contains(searchParameter, StringComparison.OrdinalIgnoreCase))
			.ToList();


				if (addin.Count > 0)
				{
					return addin;
				}
				else
				{
					return null; 
				}
			}
			catch (Exception e)
			{
				return null;
			}
		}

		public async Task<CustomMessage> editAddins(AddInsModel updatedAddIns)
		{
			try
			{
				List<AddInsModel> addinsList = await readAddinsList();
				AddInsModel coffeeToUpdate = addinsList.Find(c => c.Id == updatedAddIns.Id);

				if (coffeeToUpdate != null)
				{
					coffeeToUpdate.Name = updatedAddIns.Name;
					coffeeToUpdate.Price = updatedAddIns.Price;

					string updatedJson = JsonSerializer.Serialize(addinsList);
					await File.WriteAllTextAsync(jsonFilePath, updatedJson);
					return new CustomMessage
					{
						success = true,
						message = "Addins updated."
					};
				}
				else
				{
					return new CustomMessage
					{
						success = false,
						message = "No coffee found"
					};// Coffee with the specified ID not found
				}
			}
			catch (Exception e)
			{
				return new CustomMessage
				{
					success = false,
					message = e.Message
				};
			}
		}

		public async Task<CustomMessage> deleteAddin(Guid addinsId)
		{
			try
			{
				List<AddInsModel> addinsList = await readAddinsList();
				AddInsModel addInstodelete = addinsList.Find(c => c.Id == addinsId);

				if (addInstodelete != null)
				{
					addinsList.Remove(addInstodelete);  //removing the addin
					string updatedJson = JsonSerializer.Serialize(addinsList);
					await File.WriteAllTextAsync(jsonFilePath, updatedJson);
					return new CustomMessage {
						success = true,
						message="Addins deleted."
					};
				}
				else
				{
					return new CustomMessage
					{
						success = false,
						message = "No coffee found"
					};
				}
			}
			catch (Exception e)
			{
				return new CustomMessage
				{
					success = false,
					message = e.Message
				};
			}
		}

		public async Task<List<AddInsModel>> readAddinsList()
		{
			if (!Directory.Exists(appDataDirectoryPath))
			{
				Directory.CreateDirectory(appDataDirectoryPath);
			}
			if (!File.Exists(jsonFilePath))
			{
				return new List<AddInsModel>();
			}
			string jsonContent = await File.ReadAllTextAsync(jsonFilePath);
			return JsonSerializer.Deserialize<List<AddInsModel>>(jsonContent);
		}
	}
}
