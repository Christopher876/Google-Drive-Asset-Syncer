using Config.Net;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Drive.v3.Data;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Google_Drive;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ExtensionMethods;

namespace Google_Drive
{
	class Program
	{
		public static Configuration config;
		static void Setup()
		{
			config = new ConfigurationBuilder<Configuration>()
				.UseIniFile("config.ini")
				.Build();			
			
			//Check for folders and create the ones that does not exist
			List<string> folders = new List<string>();
			folders.Add("Models");
			folders.Add("Music");
			folders.Add("Audio");
			folders.Add("Textures");

			foreach(string folder in folders)
			{
				if (!Directory.Exists("Assets/" + folder))
				{
					Directory.CreateDirectory(folder);
					Console.WriteLine("Created " + folder + " folder");
				}
			}
		}

		static void Main(string[] args)
		{
			Notifications notifications = new Notifications();

			//Setup();
			//Drive drive = new Drive();
			//CLI cli = new CLI();
			Console.Read();
		}
	}
}