using System;
using System.Linq;
using Newtonsoft.Json;

namespace FlickrUpload
{
	public interface IFile
	{
		string PathTag { get; set; }

		string Description { get; set; }

		string Title { get; set; }

		GeoLocation GeoLocation { get; set; }
	}

	public class LocalFile : IFile
	{
		[JsonProperty ("path_tag")]
		public string PathTag { get; set; }

		[JsonProperty ("description")]
		public string Description { get; set; }

		[JsonProperty ("title")]
		public string Title { get; set; }

		[JsonProperty ("full_path")]
		public string FullPath { get; set; }

		[JsonProperty ("relative_path_normalized")]
		public string RelativePathNormalized { get; set; }

		[JsonProperty ("geo_location")]
		public GeoLocation GeoLocation { get; set; }

		public LocalFile ()
		{
		}

		public LocalFile (string fullPath, string relativePathNormalized)
		{
			GeoLocation = GeoLocation.Zero;
			FullPath = fullPath;
			RelativePathNormalized = relativePathNormalized;

			PathTag = relativePathNormalized.FormatPathTag ();
			Description = relativePathNormalized.FormatDescription ();
			Title = relativePathNormalized.FormatTitle ();
		}

		public override string ToString ()
		{
			// return string.Format ("[LocalFile: PathTag=\"{0}\", Description=\"{1}\"]", PathTag, Description);
			return string.Format ("[LocalFile: Description=\"{0}\"]", Description);
		}
	}

	public class RemoteFile : IFile
	{
		[JsonProperty ("photo_id")]
		public string PhotoId { get; set; }

		[JsonProperty ("path_tag")]
		public string PathTag { get; set; }

		[JsonProperty ("description")]
		public string Description { get; set; }

		[JsonProperty ("title")]
		public string Title { get; set; }

		[JsonProperty ("geo_location")]
		public GeoLocation GeoLocation { get; set; }

		public RemoteFile ()
		{
		}

		public RemoteFile (FlickrNet.Photo p)
		{
			GeoLocation = GeoLocation.Zero;
			PhotoId = p.PhotoId;
			PathTag = p.Tags.FirstOrDefault (t => t.IsPathTag ());
			Description = p.Description;
			Title = p.Title;

			if (Math.Abs (p.Latitude) > 0.1 && Math.Abs (p.Longitude) > 0.1) {
				GeoLocation = new GeoLocation (p.Latitude, p.Longitude);
			}
		}

		public override string ToString ()
		{
			// return string.Format ("[RemoteFile: PathTag=\"{0}\", Description=\"{1}\"]", PathTag, Description);
			return string.Format ("[RemoteFile: Description=\"{0}\"]", Description);
		}
	}
}

