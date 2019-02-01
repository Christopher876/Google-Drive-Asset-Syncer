using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text;

namespace Google_Drive
{
    class CLI
	{
		Drive drive;
		GoogleFile[] googleFile;

		public CLI()
		{
			drive = new Drive();
			googleFile = drive.ListFiles(0);
			//if auto launch is on 
			//downloads the new assets and then launches the unity hub/unity editor
			if (Program.config.Auto_Launch)
			{
				drive.AutoDownload(googleFile);
				if(Program.config.Auto_Launch_Unity) Process.Start(Program.config.Unity_Directory); //Starts up the Unity Hub
				Environment.Exit(0);
			}
			while (true)
			{
				Console.Write("> ");
				ExecuteCommands(Console.ReadLine());
			}
		}

		private void ExecuteCommands(string command)
		{
			string[] commands = command.Split();
			switch (commands[0])
			{
				case "help":
					Help();
					break;

				case "exit":
					Environment.Exit(0);
					break;
				case "ls":
					drive.ListFiles(1);
					break;
				case "clear":
					Console.Clear();
					break;
				case "download":
					if(commands.Length > 2)
					{
						StringBuilder builder = new StringBuilder();
						for(int i = 1; i < commands.Length; i++)
						{
							builder.Append(commands[i] + " ");
						}
						commands[1] = builder.ToString().TrimEnd(' ');
					}

					string id = "";
					string filename = "";
					for(int i = 0; i < googleFile.Length; i++)
					{
						if (commands[1] == googleFile[i].filename)
						{
							Console.WriteLine("Found a file to download");
							id = googleFile[i].fileID;
							filename = googleFile[i].filename;
						}
					}

					if(id != "") drive.DownloadFile(id,filename);
					break;
				case "umodel":
					drive.UploadModel(commands[1]);
					break;

				case "ump3":
					drive.UploadMP3(commands[1]);
					break;

				case "gen":
					drive.GenerateParents(googleFile);
					break;

				case "res":
					var app = Assembly.GetExecutingAssembly().Location;
					System.Diagnostics.Process.Start(app);
					Environment.Exit(0);
					break;

				case "settings":
					Console.WriteLine(Program.config.Current_Project);
					Console.WriteLine(Program.config.Server_Account);
					Console.WriteLine(Program.config.Current_Project_Audio_ID);
					Console.WriteLine(Program.config.Current_Project_Music_ID);
					Console.WriteLine(Program.config.Current_Project_Models_ID);
					break;

				case "auto":
					drive.AutoDownload(googleFile);
					break;

				default:
					StringBuilder toPrint = new StringBuilder();

					for(int i = 0; i < commands.Length; i++)
					{
						toPrint.Append(commands[i] + " ");
					}

					Console.WriteLine("No command exists for: " + toPrint.ToString());
					toPrint.Clear();
					break;				
			}
		}

		private void Help()
		{
			using(StreamReader reader = new StreamReader("help.txt"))
			{
				while (!reader.EndOfStream)
				{
					Console.WriteLine(reader.ReadLine());
				}
			}
		}
	}
}
