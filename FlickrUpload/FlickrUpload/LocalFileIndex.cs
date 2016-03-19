using System;
using System.IO;
using System.Collections.Generic;

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
		}
	}
}

