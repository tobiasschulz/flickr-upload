using System;
using FlickrNet;
using System.IO;
using Newtonsoft.Json;

namespace FlickrUpload
{
	public class FlickrManager
	{
		public const string ApiKey = "c2b13f09ff17e0c12428d4ea67bbfa1e";
		public const string SharedSecret = "d7f277b558c6450c";

		public const string MasterTag = "tobiasschulz";

		public static Flickr GetInstance ()
		{
			return new Flickr (ApiKey, SharedSecret) { InstanceCacheDisabled = true };
		}

		public static Flickr GetAuthInstance ()
		{
			var f = new Flickr (ApiKey, SharedSecret) { InstanceCacheDisabled = true };
			f.OAuthAccessToken = OAuthToken.Token;
			f.OAuthAccessTokenSecret = OAuthToken.TokenSecret;
			return f;
		}

		public static OAuthAccessToken OAuthToken {
			get {
				return FlickrConfig.Instance.OAuthToken;
			}
			set {
				FlickrConfig.Instance.OAuthToken = value;
				FlickrConfig.Save ();
			}
		}

	}
}

