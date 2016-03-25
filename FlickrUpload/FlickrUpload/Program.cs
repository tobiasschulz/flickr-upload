using System;
using System.Linq;
using FlickrNet;
using System.Threading.Tasks;

namespace FlickrUpload
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
			var do_full = args.Contains ("--full");
			Console.WriteLine ("Parameters:");
			Console.WriteLine ("  full: " + (do_full ? "true" : "false"));

			Console.WriteLine ("");
			Console.WriteLine ("Set this:\n" +
			"  http://www.flickr.com/account/geo/privacy/?from=privacy\n" +
			"to 'Anyone', and tick this one:\n" +
			"  http://www.flickr.com/account/geo/exif/?from=privacy\n" +
			"These affect only SUBSEQUENT uploads.\n");
			Console.WriteLine ("");

			FlickrConfig.Load ();
			LocalDatabase.Load ();

			FlickrAuth.Auth ();
			var indexLocal = new LocalFileIndex ();
			var photosToUpload = new LocalFile[0];

			if (do_full) {
				await indexLocal.Build ();

				var indexRemote = new RemoteFileIndex ();
				await indexRemote.Build ();

				photosToUpload = indexLocal.Photos
					.Where (p => !indexRemote.ByDesc.ContainsKey (p.Description))
					.OrderByDescending (p => p.FullPath)
					.ToArray ();
			} else {
				await indexLocal.Build ();

				photosToUpload = indexLocal.Photos
					.Where (p => !LocalDatabase.Instance.RemoteFiles.ContainsKey (p.Description))
					.OrderByDescending (p => p.FullPath)
					.ToArray ();
			}

			// geo tagging
			var remoteMissingGeoPhotos = indexLocal.Photos.Where (p => p.GeoLocation != null).Select (p => {
				RemoteFile rf;
				if (LocalDatabase.Instance.RemoteFiles.TryGetValue (p.Description, out rf)) {
					if (rf.GeoLocation == null) {
						return new { local = p, remote = rf };
					}
				}
				return null;
			}).Where (o => o != null).ToArray ();
			Console.WriteLine ("Local photos with geo location that are missing on flickr: " + remoteMissingGeoPhotos.Length);
			foreach (var o in remoteMissingGeoPhotos) {
				FlickrUploader.FixGeoLocation (o.local, o.remote);
			}

			// upload
			photosToUpload = photosToUpload.Where (p => p.FullPath.ContainsAny ("Francisco", "York", "Reisen", "Tante", "Familie", "Thomas", "Schule", "arty", "Ausfluege", "Maxi", "Tobias")).ToArray ();

			var r = new Random ();
			int i = 0;
			foreach (var p in photosToUpload) {
				FlickrUploader.Upload (p, i, photosToUpload.Length);

				// sleep depending on the time of day
				var timeofday = DateTime.Now.TimeOfDay;
				var dayofweek = (int)DateTime.Now.DayOfWeek;
				var hours = timeofday.TotalHours;
				bool isDayTime = false;
				if ((dayofweek == 0 || dayofweek >= 5)) { // weekend
					if (hours >= 9.5 || hours <= 1.5) {
						isDayTime = true;
					}
				} else {
					if (hours >= 9.5 || hours <= 0.5) {
						isDayTime = true;
					}
				}
				if (isDayTime) {
					var sleep = r.Next (20, 70);
					Console.WriteLine ("It's " + ((int)hours).ToString ().PadLeft (2, '0') + ":" + timeofday.Minutes.ToString ().PadLeft (2, '0') + ", so we wait " + sleep + " seconds");
					await Task.Delay (sleep * 1000);
				}

				i++;
			}
		}
	}
}
