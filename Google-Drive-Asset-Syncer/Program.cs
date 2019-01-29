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
			Program.config = new ConfigurationBuilder<Configuration>()
				.UseIniFile("config.ini")
				.Build();
		}

		static void Main(string[] args)
		{
			Setup();
			Drive drive = new Google_Drive.Drive();
			CLI cli = new CLI();
			Console.Read();

		}
	}
}