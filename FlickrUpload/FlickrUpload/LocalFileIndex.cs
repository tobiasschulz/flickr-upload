using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FlickrUpload
{
	public class LocalFileIndex
	{
		public LocalFile[] Photos;
		public LocalFile[] Videos;
		public LocalFile[] Others;
		public Map<string, LocalFile> ByDesc;

		public LocalFileIndex ()
		{
		}

		public async Task Build ()
		{
			while (FlickrConfig.Instance.BaseDirectory == null) {
				Console.Write ("Base Directory: ");
				var dir = Console.ReadLine ().Trim ();
				if (Directory.Exists (dir)) {
					FlickrConfig.Instance.BaseDirectory = dir;
					FlickrConfig.Save ();
				}
			}

			var photos = new List<LocalFile> ();
			var videos = new List<LocalFile> ();
			var others = new List<LocalFile> ();
			var byDesc = new DefaultMap<string, LocalFile> ();

			var baseDir = FlickrConfig.Instance.BaseDirectory;
			var baseDirNormalized = baseDir.NormalizePath ();

			foreach (string fullPath in Directory.EnumerateFiles (baseDir, "*.*", SearchOption.AllDirectories)) {
				var fullPathNormalized = fullPath.NormalizePath ();
				var relativePathNormalized = fullPathNormalized.Replace (baseDirNormalized, string.Empty).NormalizePath ();
				var file = new LocalFile (fullPath: fullPath, relativePathNormalized: relativePathNormalized);

				var ext = Path.GetExtension (fullPath);
				if (ext.IsAny (".jpg", ".png" /*, ".gif"*/)) {
					photos.Add (file);
				} else if (ext.IsAny (".mp4", ".mkv")) {
					videos.Add (file);
				} else {
					others.Add (file);
					//Console.WriteLine (file);
				}
				byDesc [file.Description] = file;
			}

			Photos = photos.ToArray ();
			Videos = videos.ToArray ();
			Others = others.ToArray ();
			ByDesc = byDesc;

			// geo location?
			var photosJpeg = photos.Where (p => p.FullPath.EndsWithAny ("jpg", "jpeg")).ToArray ();
			var photosJpegWithoutGeo = new List<LocalFile> ();
			foreach (var file in photosJpeg) {
				string geoStr;
				if (LocalDatabase.Instance.GeoLocations.TryGetValue (file.Description, out geoStr)) {
					file.GeoLocation = GeoLocation.Deserialize (geoStr);
				} else {
					photosJpegWithoutGeo.Add (file);
				}
			}
			Console.WriteLine ("Local photos without geo location: " + photosJpegWithoutGeo.Count + " of " + photosJpeg.Length);
			var parallelism = 4;
			var i = 0;
			var tasks = new Task[parallelism];
			var timeStart = DateTime.Now;
			for (int m = 0; m < parallelism; m++) {
				tasks [m] = parallel (photosJpegWithoutGeo, parallelism, m, (file) => {
					var geo = ExifHelper.GetLatlngFromImage (file.FullPath);
					file.GeoLocation = geo;
					LocalDatabase.RunLocked (() => {
						i++;
						LocalDatabase.Instance.GeoLocations [file.Description] = geo.Serialize ();
					});
					if (i % 200 == 0) {
						LocalDatabase.RunLocked (() => {
							var timeNow = DateTime.Now;
							var speed = (timeNow - timeStart).TotalMilliseconds / Math.Max (i, 1);
							var timeRemaining = TimeSpan.FromMilliseconds (speed * (photosJpegWithoutGeo.Count - i));
							LocalDatabase.Save ();
							Console.WriteLine ("  " + i + " of " + photosJpegWithoutGeo.Count + " (" + Math.Round (10000.0 / (double)photosJpegWithoutGeo.Count * (double)i) / 100 + "%, remaining: " + new DateTime (timeRemaining.Ticks).ToString ("HH:mm:ss") + ")");
						});
					}
				});
			}
			await Task.WhenAll (tasks);

			LocalDatabase.Save ();
		}

		static Task parallel<T> (List<T> list, int parallelism, int modulo, Action<T> call)
		{
			return Task.Run (() => {
				for (int i = 0; i < list.Count; i++) {
					if (i % parallelism == modulo) {
						// Console.WriteLine (modulo + ": " + i);
						call (list [i]);
					}
				}
			});
		}
	}
}

