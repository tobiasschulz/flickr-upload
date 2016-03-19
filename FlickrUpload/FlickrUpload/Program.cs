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

			foreach (var p in indexLocal.Photos) {
				if (!p.FullPath.Contains ("Francisco"))
					continue;
				if (indexRemote.ByDesc.ContainsKey (p.Description))
					continue;
				
				FlickrUploader.Upload (p);

				// sleep depending on the time of day
				var hours = DateTime.Now.TimeOfDay.TotalHours;
				if (hours >= 9 && hours <= 23) {
					var sleep = r.Next (20, 60);
					Console.WriteLine ("It's " + (int)hours + " o’clock, so we wait " + sleep + " seconds");
					System.Threading.Thread.Sleep (sleep * 1000);
				}


				//break;
			}
		}
	}
}
