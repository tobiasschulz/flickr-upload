using System;
using System.Linq;

namespace FlickrUpload
{
	public class FlickrUploader
	{
		public static void Upload (LocalFile file)
		{
			Console.WriteLine ("Upload local file: " + file);
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

