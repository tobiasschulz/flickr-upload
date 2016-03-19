using System;
using FlickrNet;
using System.IO;
using Newtonsoft.Json;

namespace FlickrUpload
{
	public class FlickrConfig
	{
		public static string ConfigDirectory = Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.ApplicationData), "FlickrUpload");
		public static string ConfigFile = Path.Combine (ConfigDirectory, "config.json");
		static FlickrConfig _instance;

		public static FlickrConfig Instance {
			get {
				if (_instance == null) {
					Load ();
				}
				return _instance;
			}
		}

		public static void Load ()
		{
			Directory.CreateDirectory (Path.GetDirectoryName (ConfigFile));
			Console.WriteLine ("Load config from: " + ConfigFile);
			if (File.Exists (ConfigFile)) {
				_instance = File.ReadAllText (ConfigFile).FromJson<FlickrConfig> () ?? new FlickrConfig ();
			} else {
				_instance = new FlickrConfig ();
			}
		}

		public static void Save ()
		{
			Directory.CreateDirectory (Path.GetDirectoryName (ConfigFile));
			Console.WriteLine ("Save config to: " + ConfigFile);
			File.WriteAllText (ConfigFile, Instance.ToJson ());
		}

		[JsonProperty ("oauth_access_token")]
		public OAuthAccessToken OAuthToken { get; set; }

		[JsonProperty ("base_directory")]
		public string BaseDirectory { get; set; }
	}
}
