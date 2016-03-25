using System;
using FlickrNet;
using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace FlickrUpload
{
	public class LocalDatabase
	{
		public static string ConfigDirectory = Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.ApplicationData), "FlickrUpload");
		public static string ConfigFile = Path.Combine (ConfigDirectory, "database.json");
		static LocalDatabase _instance;
		static object _lock = new object ();

		public static LocalDatabase Instance {
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
			Console.WriteLine ("Load database from: " + ConfigFile);
			if (File.Exists (ConfigFile)) {
				_instance = JsonConvert.DeserializeObject<LocalDatabase> (File.ReadAllText (ConfigFile)) ?? new LocalDatabase ();
			} else {
				_instance = new LocalDatabase ();
			}
		}

		public static void Save ()
		{
			Directory.CreateDirectory (Path.GetDirectoryName (ConfigFile));
			Console.WriteLine ("Save database to: " + ConfigFile);
			File.WriteAllText (ConfigFile, Instance.ToJson (ignoreNull: false));
		}

		public static void RunLocked (Action action)
		{
			lock (_lock) {
				action ();
			}
		}

		[JsonProperty ("geo_locations")]
		public Dictionary<string, string> GeoLocations { get; set; } = new Dictionary<string, string>();

		[JsonProperty ("remote_files")]
		public Dictionary<string, RemoteFile> RemoteFiles { get; set; } = new Dictionary<string, RemoteFile>();
	}
}
