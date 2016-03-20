using System;
using System.Linq;
using FlickrNet;

namespace FlickrUpload
{
	class MainClass
	{
		public static void Main (string[] args)
		{
			FlickrAuth.Auth ();
			var indexLocal = new LocalFileIndex ();
			var indexRemote = new RemoteFileIndex ();

			var r = new Random ();

			var photosToUpload = indexLocal.Photos
				.Where (p => !indexRemote.ByDesc.ContainsKey (p.Description))
				.OrderByDescending (p => p.FullPath)
				.ToArray ();


			photosToUpload = photosToUpload.Where (p => p.FullPath.ContainsAny ("Francisco", "York")).ToArray ();

			int i = 0;
			foreach (var p in photosToUpload) {
				FlickrUploader.Upload (p, i, photosToUpload.Length);

				// sleep depending on the time of day
				var timeofday = DateTime.Now.TimeOfDay;
				var hours = timeofday.TotalHours;
				if (hours >= 9.5 || hours <= 1.5) {
					var sleep = r.Next (20, 70);
					Console.WriteLine ("It's " + ((int)hours).ToString ().PadLeft (2, '0') + ":" + timeofday.Minutes.ToString ().PadLeft (2, '0') + ", so we wait " + sleep + " seconds");
					System.Threading.Thread.Sleep (sleep * 1000);
				}

				i++;
			}
		}
	}
}
