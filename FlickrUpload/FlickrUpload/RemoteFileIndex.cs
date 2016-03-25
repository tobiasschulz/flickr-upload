using System;
using FlickrNet;
using System.Threading.Tasks;

namespace FlickrUpload
{
	public class RemoteFileIndex
	{
		public Map<string, RemoteFile> ByDesc = new DefaultMap<string, RemoteFile> ();

		public RemoteFileIndex ()
		{
		}

		public async Task Build ()
		{
			PhotoSearchOptions o = new PhotoSearchOptions ();
			o.Extras = PhotoSearchExtras.AllUrls | PhotoSearchExtras.Description | PhotoSearchExtras.OriginalUrl | PhotoSearchExtras.Tags | PhotoSearchExtras.Geo;
			o.SortOrder = PhotoSearchSortOrder.DatePostedDescending; // DateTakenDescending
			//o.Tags = FlickrManager.MasterTag;
			//o.TagMode = TagMode.AllTags;
			o.UserId = FlickrManager.OAuthToken.UserId;
			//o.Text = "Path";
			o.PerPage = 500;
			o.Page = 1;

			var f = FlickrManager.GetAuthInstance ();
			

			int i = 0;
			var results = f.PhotosSearch (o);
			do {
				Console.WriteLine ("Result count (page " + results.Page + " of " + results.Pages + "): " + results.Count);
				foreach (var p in results) {
					// Console.WriteLine (p.ToJson ());
					var file = new RemoteFile (p);
					LocalDatabase.RunLocked (() => LocalDatabase.Instance.RemoteFiles [file.Description] = file);

					ByDesc [file.Description] = file;

					Console.WriteLine ("Remote file " + i + ": " + file);

					i++;
				}

				LocalDatabase.RunLocked (() => LocalDatabase.Save ());
				await Task.Delay (1000);

				o.Page++;
				results = f.PhotosSearch (o);
			} while (results != null && results.Count >= 0 && results.Page <= results.Pages);
		}
	}
}

