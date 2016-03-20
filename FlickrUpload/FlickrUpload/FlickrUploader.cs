using System;
using System.Linq;

namespace FlickrUpload
{
	public class FlickrUploader
	{
		public static void Upload (LocalFile file, int i, int n)
		{
			Console.Write ("Upload local file " + i + " of " + n + ": " + file + " => ");
			var f = FlickrManager.GetAuthInstance ();
			f.OnUploadProgress += F_OnUploadProgress;
			string photoId = f.UploadPicture (file.FullPath, file.Title, file.Description, string.Join (",", new [] {
				FlickrManager.MasterTag,
				file.PathTag
			}.Union (file.RelativePathNormalized.Split ('/'))), false, false, false);
			Console.WriteLine ("photoId: " + photoId);
		}

		static void F_OnUploadProgress (object sender, FlickrNet.UploadProgressEventArgs e)
		{
			
		}
	}
}

