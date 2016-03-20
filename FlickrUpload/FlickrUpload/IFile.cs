using System;
using System.Linq;

namespace FlickrUpload
{
	public interface IFile
	{
		string PathTag { get; set; }

		string Description { get; set; }

		string Title { get; set; }
	}

	public class LocalFile : IFile
	{
		public string PathTag { get; set; }

		public string Description { get; set; }

		public string Title { get; set; }

		public string FullPath { get; set; }

		public string RelativePathNormalized { get; set; }

		public LocalFile (string fullPath, string relativePathNormalized)
		{
			FullPath = fullPath;
			RelativePathNormalized = relativePathNormalized;

			PathTag = relativePathNormalized.FormatPathTag ();
			Description = relativePathNormalized.FormatDescription ();
			Title = relativePathNormalized.FormatTitle ();
		}

		public override string ToString ()
		{
			// return string.Format ("[LocalFile: PathTag={0}, Description={1}]", PathTag, Description);
			return string.Format ("[LocalFile: Description={1}]", Description);
		}
	}

	public class RemoteFile : IFile
	{
		public string PathTag { get; set; }

		public string Description { get; set; }

		public string Title { get; set; }

		public RemoteFile (FlickrNet.Photo p)
		{
			PathTag = p.Tags.FirstOrDefault (t => t.IsPathTag ());
			Description = p.Description;
			Title = p.Title;
		}

		public override string ToString ()
		{
			// return string.Format ("[RemoteFile: PathTag={0}, Description={1}]", PathTag, Description);
			return string.Format ("[RemoteFile: Description={1}]", Description);
		}
	}
}

