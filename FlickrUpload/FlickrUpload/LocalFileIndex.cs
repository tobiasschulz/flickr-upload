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
				if (ext.IsAny (".jpg", ".png", ".gif")) {
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
				GeoLocation geo;
				if (LocalDatabase.Instance.GeoLocations.TryGetValue (file.Description, out geo)) {
					file.GeoLocation = geo;
				} else {
					photosJpegWithoutGeo.Add (file);
				}
			}
			Console.WriteLine ("Local photos without geo location: " + photosJpegWithoutGeo.Count + " of " + photosJpeg.Length);
			int parallelism = 8;
			int i = 0;
			Task[] tasks = new Task[parallelism];
			for (int m = 0; m < parallelism; m++) {
				tasks [m] = Task.Run (async () => await parallel (photosJpegWithoutGeo, m, (file) => {
					var geo = ExifHelper.GetLatlngFromImage (file.FullPath);
					file.GeoLocation = geo;
					LocalDatabase.RunLocked (() => LocalDatabase.Instance.GeoLocations [file.Description] = geo);
					i++;
					if (i % 100 == 0) {
						LocalDatabase.RunLocked (() => {
							LocalDatabase.Save ();
							Console.WriteLine ("  " + i + " of " + photosJpegWithoutGeo.Count + " (" + Math.Round (10000.0 / (double)photosJpegWithoutGeo.Count * (double)i) / 100 + "%)");
						});
					}
				}));
			}
			await Task.WhenAll (tasks);

			LocalDatabase.Save ();
		}

		static Task parallel<T> (List<T> list, int modulo, Action<T> call)
		{
			for (int i = 0; i < list.Count; i++) {
				if (i % modulo == 0) {
					call (list [i]);
				}
			}
			return Tasks.Completed;
		}
	}
}

