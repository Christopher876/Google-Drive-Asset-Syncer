using Config.Net;

namespace Google_Drive
{
    public interface Configuration
	{
		string Server_Account { get; }
		string Current_Project { get; }
		string Current_Project_ID { get; }
		string Current_Project_Models_ID { get; }
		string Current_Project_Audio_ID { get; }
		string Current_Project_Music_ID { get; }
		string Unity_Directory { get; }

		[Option(DefaultValue = false)]
		bool Auto_Launch_Unity { get; }
		[Option(DefaultValue = false)]
		bool Auto_Launch { get; }

		string Test { get; set; }
	}
}
