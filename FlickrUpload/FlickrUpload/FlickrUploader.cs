using System;
using System.Linq;

namespace FlickrUpload
{
	public class FlickrUploader
	{
		public static int CountFailures = 0;

		public static void Upload (LocalFile file, int i, int n)
		{
			try {
				Console.Write ("Upload local file " + i + " of " + n + ": " + file + " => ");
				var f = FlickrManager.GetAuthInstance ();
				f.OnUploadProgress += F_OnUploadProgress;
				string photoId = f.UploadPicture (file.FullPath, file.Title, file.Description, string.Join (",", new [] {
					FlickrManager.MasterTag,
					file.PathTag
				}.Union (file.RelativePathNormalized.Split ('/'))), false, false, false);
				Console.WriteLine ("photoId: " + photoId);
			} catch (Exception ex) {
				CountFailures++;
				if (CountFailures > 10) {
					throw;
				}
				Console.WriteLine ("Exception during upload:");
				Console.WriteLine (ex);
				int sec = 10 * Math.Min (5, CountFailures);
				Console.WriteLine ("Wait for " + sec + " seconds");
				System.Threading.Thread.Sleep (1000 * sec);
			}
		}

		static void F_OnUploadProgress (object sender, FlickrNet.UploadProgressEventArgs e)
		{
			
		}
	}
}

