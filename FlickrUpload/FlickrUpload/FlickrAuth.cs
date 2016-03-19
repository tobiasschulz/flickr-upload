using System;
using FlickrNet;

namespace FlickrUpload
{
	public static class FlickrAuth
	{
		public static void Auth ()
		{
			if (FlickrManager.OAuthToken == null) {
				var f = FlickrManager.GetInstance ();
				var requestToken = f.OAuthGetRequestToken ("oob");

				string url = f.OAuthCalculateAuthorizationUrl (requestToken.Token, AuthLevel.Write);

				System.Diagnostics.Process.Start (url);

				Console.Write ("Auth token: ");
				var token = Console.ReadLine ().Trim ();
				try {
					var accessToken = f.OAuthGetAccessToken (requestToken, token);
					FlickrManager.OAuthToken = accessToken;
					Console.WriteLine ("Successfully authenticated as " + accessToken.FullName);
				} catch (FlickrApiException ex) {
					Console.WriteLine ("Failed to get access token. Error message: " + ex.Message);
				}
				FlickrConfig.Save ();
			} else {
				var accessToken = FlickrManager.OAuthToken;
				Console.WriteLine ("Successfully authenticated as " + accessToken.FullName);
			}
		}
	}
}

