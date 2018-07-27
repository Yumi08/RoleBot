using System.IO;
using Newtonsoft.Json;

namespace RoleBot
{
	class Config
	{
		private const string ConfigFolder = "Resources";
		private const string ConfigFile = "config.json";
		public static BotConfig Bot;

		static Config()
		{
			if (!Directory.Exists(ConfigFolder)) Directory.CreateDirectory(ConfigFolder);

			if (!File.Exists(ConfigFolder + "/" + ConfigFile))
			{
				Bot = new BotConfig();
				string json = JsonConvert.SerializeObject(Bot, Formatting.Indented);
				File.WriteAllText(ConfigFolder + "/" + ConfigFile, json);
			}
			else
			{
				string json = File.ReadAllText(ConfigFolder + "/" + ConfigFile);
				Bot = JsonConvert.DeserializeObject<BotConfig>(json);
			}
		}
	}

	public struct BotConfig
	{
		public string token;
		public char cmdPrefix;
		public string botName;
		public string botId;
	}
}