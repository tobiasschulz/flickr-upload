﻿using System;
using FlickrNet;

namespace FlickrUpload
{
	public class RemoteFileIndex
	{
		public Map<string, RemoteFile> ByDesc = new DefaultMap<string, RemoteFile> ();

		public RemoteFileIndex ()
		{
			PhotoSearchOptions o = new PhotoSearchOptions ();
			o.Extras = PhotoSearchExtras.AllUrls | PhotoSearchExtras.Description | PhotoSearchExtras.OriginalUrl | PhotoSearchExtras.Tags;
			o.SortOrder = PhotoSearchSortOrder.DateTakenDescending;
			//o.Tags = FlickrManager.MasterTag;
			//o.TagMode = TagMode.AllTags;
			o.UserId = FlickrManager.OAuthToken.UserId;
			//o.Text = "Path";
			o.PerPage = 500;
			o.Page = 1;

			var f = FlickrManager.GetAuthInstance ();

			var results = f.PhotosSearch (o);
			do {
				Console.WriteLine ("Result count: " + results.Total);
				foreach (var p in results) {
					// Console.WriteLine (p.ToJson ());
					var file = new RemoteFile (p);

					ByDesc [file.Description] = file;

					Console.WriteLine (file);
				}



				o.Page++;
				results = f.PhotosSearch (o);
			} while (results != null && o.Page <= results.Pages);
		}
	}
}

