using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CoffeeBucks.Utils
{
	public class FilePaths
	{
        public static string GetAppDirectoryPath()
        {
            return Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                "Bislerium"
            );
        }

        public static string GetJSONFilePath(string filename)
        {
            return Path.Combine(GetAppDirectoryPath(), filename);
        }
    }
}
