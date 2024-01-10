using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CoffeeBucks.models;

namespace CoffeeBucks.Utils
{
	public class CustomUserMessage
	{
		public string message {  get; set; }
		public bool success { get; set; }
		public UserModel user { get; set; }
		public bool isFirstUser { get; set; } = false;
	}
}
