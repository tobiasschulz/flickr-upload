using System;
using System.IO;
using System.Threading.Tasks;
using System.Collections.Generic;
using FlickrUpload;
using Newtonsoft.Json;

namespace FlickrMap
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			Task.Run (async () => {
				await Run (args);
			}).Wait ();
		}

		static async Task Run (string[] args)
		{
			var jsonPhotos = new List<JsonPhoto> ();
			foreach (var rf in LocalDatabase.Instance.RemoteFiles.Values) {
				if (rf.GeoLocation.IsNonZero && rf.LinkThumbnail != null) {
					jsonPhotos.Add (new JsonPhoto {
						GeoLocation = rf.GeoLocation,
						LinkOriginal = rf.LinkOriginal,
						LinkThumbnail = rf.LinkThumbnail,
					});
				}
			}

			if (Directory.GetCurrentDirectory ().Trim ('\\', '/').EndsWith ("Release")) {
				Directory.SetCurrentDirectory (Path.GetDirectoryName (Directory.GetCurrentDirectory ()));
			}
			if (Directory.GetCurrentDirectory ().Trim ('\\', '/').EndsWith ("bin")) {
				Directory.SetCurrentDirectory (Path.GetDirectoryName (Directory.GetCurrentDirectory ()));
			}
			Console.WriteLine ("path: " + Directory.GetCurrentDirectory ());

			File.WriteAllText ("data.json", jsonPhotos.ToJson (inline: true));
		}
	}

	public class JsonPhoto
	{
		[JsonProperty ("geo_location")]
		public GeoLocation GeoLocation { get; set; }

		[JsonProperty ("link_original")]
		public RemoteUrl LinkOriginal { get; set; }

		[JsonProperty ("link_thumbnail")]
		public RemoteUrl LinkThumbnail { get; set; }
	}
}