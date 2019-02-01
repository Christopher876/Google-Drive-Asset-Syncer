using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using ExtensionMethods;

using System.Security.Cryptography.X509Certificates;
using Google.Apis.Download;

namespace Google_Drive
{
    class Drive
	{
		private string[] scopes = { DriveService.Scope.Drive }; //Read and write everything
		private string applicationName = "Drive Project Sync";

		public DriveService service;

		public string[] Scopes
		{
			get { return scopes; }
		}

		public string ApplicationName { get; }
		
		//Initialize the Google Drive service
		public Drive()
		{
			//Authentication
			//credentials.json & token.json are used for the login
			//This is the service account that can Read/Write to the directory

			var certificate = new X509Certificate2(@"key.p12", "notasecret", X509KeyStorageFlags.Exportable);

			ServiceAccountCredential credential = new ServiceAccountCredential(
				new ServiceAccountCredential.Initializer(Program.config.Server_Account)
				{
					Scopes = scopes
				}.FromCertificate(certificate));

			// Create Drive API service.
			service = new DriveService(new BaseClientService.Initializer()
			{
				HttpClientInitializer = credential,
				ApplicationName = ApplicationName,
			});
		}

		public string CreateFolder(string folderName)
		{
			var fileMetadata = new Google.Apis.Drive.v3.Data.File()
			{
				Name = folderName,
				MimeType = "application/vnd.google-apps.folder"
			};
			var request = service.Files.Create(fileMetadata);
			request.Fields = "id";
			var file = request.Execute();
			return file.Id;
		}

		public GoogleFile[] ListFiles(int c)
		{
			// Define parameters of request.
			FilesResource.ListRequest listRequest = service.Files.List();
			listRequest.PageSize = 1000; //Max:1000
			listRequest.Fields = "nextPageToken, files(id, name, parents)";

			// List files.
			IList<Google.Apis.Drive.v3.Data.File> files = listRequest.Execute()
				.Files;

			GoogleFile[] googleFile = new GoogleFile[files.Count];
			int i = 0;
			if (files != null && files.Count > 0)
			{
				foreach (var file in files)
				{
					googleFile[i] = new GoogleFile();
					googleFile[i].filename = file.Name;
					googleFile[i].fileID = file.Id;
					googleFile[i].parent = file.Parents;

					if (c == 1)
					{
						if(file.Parents != null) Console.WriteLine("{0} ({1}) Parent={2}", file.Name, file.Id, file.Parents[0]);
						else Console.WriteLine("{0} ({1})", file.Name, file.Id);
					}
					i++;
				}
			}
			else
			{
				Console.WriteLine("No files found.");
			}
			return googleFile;
		}

		public void AutoDownload(GoogleFile[] googleFiles)
		{
			//Holds the names of the files within each directory
			List<string> modelFilenames = new List<string>();
			List<string> audioFilenames = new List<string>();
			List<string> musicFilenames = new List<string>();
			
			//Not implemented to get info from the config file yet
			List<string> validModelsFileExtensions = new List<string>();
			List<string> validAudioFileExtensions = new List<string>();
			List<string> validMusicFileExtensions = new List<string>();

			//DIrectory Info
			DirectoryInfo modelDirectory = new DirectoryInfo(@"Assets/Models");
			DirectoryInfo audioDirectory = new DirectoryInfo(@"Assets/Audio");
			DirectoryInfo musicDirectory = new DirectoryInfo(@"Assets/Music");

			//Collects all the file names and adds them to the list			
			foreach(FileInfo file in modelDirectory.GetFiles())
			{
				modelFilenames.Add(file.Name);
			}
			foreach (FileInfo file in audioDirectory.GetFiles())
			{
				audioFilenames.Add(file.Name);
			}
			foreach (FileInfo file in musicDirectory.GetFiles())
			{
				musicFilenames.Add(file.Name);
			}

			#region Loops that check the extensions and check if the file exists and then downloads it if it does not exist
			//Check the parent to make sure right thing
			Console.WriteLine("Downloading Model Files...");
			for (int i = 0; i < googleFiles.Length; i++)
			{
				try
				{
					if (!modelFilenames.Contains(googleFiles[i].filename) && googleFiles[i].filename.CompareExtension(".fbx") && googleFiles[i].parent[0] == Program.config.Current_Project_Models_ID)
					{
						DownloadFile(googleFiles[i].fileID, @"Assets/Models/" + googleFiles[i].filename);
					}
				}
				catch(Exception e)
				{
					continue;
				}
			}

			//Check the parent for the audio and music files
			Console.WriteLine("Downloading Audio Files...");
			for (int i = 0; i < googleFiles.Length; i++)
			{
				try
				{
					if (!audioFilenames.Contains(googleFiles[i].filename) && googleFiles[i].filename.CompareExtension(".wav") && googleFiles[i].parent[0] == Program.config.Current_Project_Audio_ID)
					{
						DownloadFile(googleFiles[i].fileID, @"Assets/Audio/" + googleFiles[i].filename);
					}
				}
				catch(Exception e)
				{
					continue;
				}
			}

			Console.WriteLine("Downloading Music Files...");
			for (int i = 0; i < googleFiles.Length; i++)
			{
				try
				{
					if (!musicFilenames.Contains(googleFiles[i].filename) && googleFiles[i].parent[0] == Program.config.Current_Project_Music_ID && (googleFiles[i].filename.CompareExtension(".wav") || googleFiles[i].filename.CompareExtension(".flac")))
					{
						DownloadFile(googleFiles[i].fileID, @"Assets/Music/" + googleFiles[i].filename);
					}
				}
				catch(Exception e)
				{
					continue;
				}
			}
			#endregion
		}

		public void GenerateParents(GoogleFile[] googleFile)
		{
			Dictionary<string,string> parents = new Dictionary<string, string>();

			//Get the number of directories
			for(int i = 0; i < googleFile.Length; i++)
			{
				//Search for the file that contains the file Id for the parent file
				if (googleFile[i].parent != null && !parents.ContainsValue(googleFile[i].parent[0]))
				{
					for(int j = 0; j < googleFile.Length; j++)
					{
						try
						{
							if (googleFile[j].fileID == googleFile[i].parent[0])
							{
								parents.Add(googleFile[j].filename, googleFile[j].fileID);
							}
						}
						catch (Exception e)
						{
							Console.ForegroundColor = ConsoleColor.Red;
							Console.WriteLine(e.ToString());
							Console.ResetColor();
							continue;
						}
					}
				}
			}

			//Pair the parent name with the file
			foreach(KeyValuePair<string,string> entry in parents)
			{
				for (int i = 0; i < googleFile.Length; i++)
				{
					try
					{
						if (googleFile[i].parent[0] == entry.Value)
						{
							googleFile[i].parentName = entry.Key;
						}
					}
					catch (Exception e)
					{
						Console.ForegroundColor = ConsoleColor.Red;
						Console.WriteLine(e.ToString());
						Console.ResetColor();
					}
				}
			}

			foreach(var value in googleFile)
			{
				if(value.parentName != "")
					Console.WriteLine(value.filename + "=" + value.parentName);
			}
		}

		public void DownloadFile(string id, string filename)
		{
			string fileId = id;
			var request = service.Files.Get(fileId);
			var stream = new MemoryStream();

			//Event for the downloader
			request.MediaDownloader.ProgressChanged +=
			(IDownloadProgress progress) =>
			{
				switch (progress.Status)
				{
					case DownloadStatus.Downloading:
						{
							Console.WriteLine(progress.BytesDownloaded/1024);
							break;
						}
					case DownloadStatus.Completed:
						{
							Console.WriteLine("Download complete.");
							break;
						}
					case DownloadStatus.Failed:
						{
							Console.WriteLine("Download failed.");
							break;
						}
				}
			};
			request.Download(stream);

			using (FileStream file = new System.IO.FileStream(filename, System.IO.FileMode.Create, System.IO.FileAccess.Write))
			{
				stream.WriteTo(file);
			}
		}

		public void UploadModel(string toUpload)
		{
			string folderId = Program.config.Current_Project_Models_ID; //Folder ID for the Project Models Folder CAN BE EDITTED IN CONFIG
			var fileMetadata = new Google.Apis.Drive.v3.Data.File()
			{
				Name = Path.GetFileName(toUpload),
				Parents = new List<string>
				{
					folderId
				}
			};
			FilesResource.CreateMediaUpload request;
			using (var stream = new System.IO.FileStream(toUpload,
									System.IO.FileMode.Open))
			{
				request = service.Files.Create(
					fileMetadata, stream, "application/octet-stream"); //FBX content type
				request.Fields = "id";
				request.Upload();
			}
			var file = request.ResponseBody;
			Console.WriteLine("File ID: " + file.Id);
		}

		public void UploadMP3(string toUpload)
		{
			string folderId = Program.config.Current_Project_Music_ID; //Folder ID for the Project Music Folder CAN BE EDITTED IN CONFIG

			if (!File.Exists(toUpload)) return;

			var fileMetadata = new Google.Apis.Drive.v3.Data.File()
			{
				Name = Path.GetFileName(toUpload),
				Parents = new List<string>
				{
					folderId
				}
			};
			FilesResource.CreateMediaUpload request;
			using (var stream = new System.IO.FileStream(toUpload,
									System.IO.FileMode.Open))
			{
				request = service.Files.Create(
					fileMetadata, stream, "audio/mpeg"); //MP3 content type
				request.Fields = "id";
				request.Upload();
			}
			var file = request.ResponseBody;
			Console.WriteLine("File ID: " + file.Id);
		}

		public void UploadFileToFolder()
		{

		}
	}
}
