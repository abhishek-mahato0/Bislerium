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
	public class UserController
		
	{
		
        private string appDataDirectoryPath = FilePaths.GetAppDirectoryPath();
        private string appUsersFilePath = FilePaths.GetJSONFilePath("users.json");

		//login function
		public async Task<CustomUserMessage>  login(string username, string password)
		{
			try
			{
              
				List<UserModel> userList = await readUserList();
				PasswordUtils psUtils = new PasswordUtils();

				if ( userList == null || userList.Count==0)
				{
                    UserModel userfound = new UserModel();
                    if (username != "admin" || password != "admin")
					{
						return new CustomUserMessage
						{
							message = "The initial login credential is admin and admin.",
							user = null,
							success = false,
							isFirstUser = true,
						};
					}
					else
					{
						userfound = new UserModel
							{
								Name = "admin",
								Username = username,
								Password = "admin",
								role = "admin",
								PhoneNumber="9800000000", 
							};
							CustomMessage message = await addUsers(userfound);
							if (message.success)
							{
								return new CustomUserMessage
								{
									message = message.message,
									user = userfound,
									success = true,
									isFirstUser = true,
								};
							}
							else
							{
								return new CustomUserMessage
								{
									message = message.message,
									user = null,
									success = true,
									isFirstUser = true,
								};
							}
						
					}
				}
				else
				{ 
					UserModel userfound = userList.FirstOrDefault(c => c.Username == username);
					if (userfound == null)
					{
						return new CustomUserMessage
						{
							message = "No user found",
							user = null,
							success = false,
							isFirstUser = false,
						};
					}
					bool isPasswordCorrect = new PasswordUtils().VerifyHash(password, userfound.Password);

					if (!isPasswordCorrect)
					{
						return new CustomUserMessage
						{
							message = "Password Incorrect",
							user = null,
							success = false,
							isFirstUser = false,
						};
					}
					else
					{
						return new CustomUserMessage
						{
							message = "Login successfull.",
							user = userfound,
							success = true,
							isFirstUser = false,
						};
					}
				}
			}catch(Exception ex)
			{
				return new CustomUserMessage
				{
					message = ex.Message,
					user = null,
					success = false,
					isFirstUser = false,
				};
			}

		}

	//verify password while deleting any product or making changes
	public async Task<bool> verifyPassword(Guid Id, string password)
		{
			try
			{
				List<UserModel> userList = await readUserList();
				UserModel user = userList.FirstOrDefault(c => c.Id == Id && c.role == "admin");
				if (user == null)
				{
					return false;
				}
				else
				{
					if (new PasswordUtils().VerifyHash(password, user.Password))
					{
						return true;
					}
					return false;
				}
			}catch (Exception ex)
			{
				return false;
			}
        }
		//adding new users only by admin
	public async Task<CustomMessage> addUsers(UserModel user)
	{
		try
		{
			CustomMessage custommessage = new CustomMessage();

			List<UserModel> userList =await readUserList();
			UserModel userModel = userList.FirstOrDefault(c => c.Username == user.Username);
				if(userModel != null)
				{
					return new CustomMessage
					{
						message = "User already exists.",
						success = false
					};
				}
                user.Password =new PasswordUtils().HashSecret(user.Password);
                userList.Add(user);
			string updatedJson = System.Text.Json.JsonSerializer.Serialize(userList);
			await File.WriteAllTextAsync(appUsersFilePath, updatedJson);
			return new CustomMessage
            {
                message = "User created successfully.",
                success = true
            };
            }
		catch (Exception e)
		{
			return new CustomMessage
            {
                message = e.Message,
                success = false
            }; ;
		}

	}

	public async Task<CustomMessage> editUser(UserModel updatedUser)
	{
		try
		{
			List<UserModel> userList =await readUserList();
			UserModel userToUpdate = userList.Find(c => c.Id == updatedUser.Id);

			if (userToUpdate != null)
			{
				userToUpdate.Name = updatedUser.Name;
				userToUpdate.Username = updatedUser.Username;
				userToUpdate.Password = updatedUser.Password;
				userToUpdate.PhoneNumber = updatedUser.PhoneNumber;
				userToUpdate.role = updatedUser.role;

				string updatedJson = JsonSerializer.Serialize(userList);
				await File.WriteAllTextAsync(appUsersFilePath, updatedJson);
				return new CustomMessage
                {
                    message = "User edited successfully.",
                    success = true
                }; ;
			}
			else
			{
				return new CustomMessage
                {
                    message = "User does not exist.",
                    success = false
                }; ; //user with the specified ID not found
			}
		}
		catch (Exception e)
		{
			
			return new CustomMessage
            {
                message = e.Message,
                success = false
            }; 
            }
	}
		public CustomMessage changePassword(Guid id, string currentPassword, string newPassword)
		{

			var json = File.ReadAllText(appUsersFilePath);
			List<UserModel> userList = JsonSerializer.Deserialize<List<UserModel>>(json); ;
			UserModel user = userList.FirstOrDefault(x => x.Id == id);

			if (user == null)
			{
				return new CustomMessage
				{
					success = false,
					message = "No user found."
				};
			}
			if(currentPassword == newPassword)
			{
				return new CustomMessage
				{
					success = false,
					message = "New password and current password cannot be same."
				};
			}

			PasswordUtils psUtils = new PasswordUtils();
			bool passwordIsValid = psUtils.VerifyHash(currentPassword, user.Password);  //verifying password hash

			if (!passwordIsValid)
			{
				return new CustomMessage
				{
					success = false,
					message = "Incorrect current password."
				};
			}
			else
			{
				user.Password =	psUtils.HashSecret(newPassword);
				string updatedJson = JsonSerializer.Serialize(userList);
				File.WriteAllText(appUsersFilePath, updatedJson);
				return new CustomMessage
				{
					success = true,
					message = "Password changed successfuly."
				};
			}
		}
		public string getUserById(Guid userId)
		{
			try
			{
				var json = File.ReadAllText(appUsersFilePath);
				List<UserModel> userList = JsonSerializer.Deserialize<List<UserModel>>(json);
				UserModel userToshow = userList.FirstOrDefault(c => c.Id == userId);

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
		public async Task<CustomMessage> deleteUser(Guid userId)
	{
		try
		{
			List<UserModel> userList =await readUserList();
			UserModel userToDelete = userList.Find(c => c.Id == userId);

			if (userToDelete != null)
			{
				userList.Remove(userToDelete);
				string updatedJson = JsonSerializer.Serialize(userList);
				await File.WriteAllTextAsync(appUsersFilePath, updatedJson);
				return new CustomMessage
                {
                    message = "User edited successfully.",
                    success = true
                }; ;
                }
			else
			{
                    return new CustomMessage
                    {
                        message = "User does not exist.",
                        success = false
                    }; ;// Coffee with the specified ID not found
                }
		}
		catch (Exception e)
		{
			return new CustomMessage
            {
                message = e.Message,
                success = false
            };
            }
	}
        public List<UserModel> getUser(string parameter)
        {
            try
            {
                var json = File.ReadAllText(appUsersFilePath);
                List<UserModel> userList = JsonSerializer.Deserialize<List<UserModel>>(json); ;
				List<UserModel> users = userList.FindAll(c =>
			c.Name.Contains(parameter, StringComparison.OrdinalIgnoreCase) ||
			c.PhoneNumber.Contains(parameter, StringComparison.OrdinalIgnoreCase)
	   );
                if (users.Count > 0)
                {
                    return users;
                }
                else
                {
                    return null; // user with the specified ID not found
                }
            }
            catch (Exception e)
            {
                return null;
            }
        }

		//reading json file
        public async Task<List<UserModel>> readUserList()
	{
            if (!Directory.Exists(appDataDirectoryPath))
            {
                Directory.CreateDirectory(appDataDirectoryPath);
            }
            if (!File.Exists(appUsersFilePath))
            {
                return new List<UserModel>();
            }
            string jsonContent =await File.ReadAllTextAsync(appUsersFilePath);
			return JsonSerializer.Deserialize<List<UserModel>>(jsonContent);
	}
}
}
