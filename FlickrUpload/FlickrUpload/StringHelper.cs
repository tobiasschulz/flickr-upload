using System;
using System.Text.RegularExpressions;

namespace FlickrUpload
{
	public static class StringHelper
	{
		static Regex regexTagNotAllowed = new Regex ("[^a-zA-Z0-9]+");
		static Regex regexDescriptionNotAllowed = new Regex ("[^a-zA-Z0-9 _./-]+");

		public static string FormatPathTag (this string value)
		{
			value = value.NormalizePath ();
			if (value.StartsWith ("Bilder/")) {
				value = value.Substring (7);
			}
			value = regexTagNotAllowed.Replace (value, "");
			return "path-" + value;
		}

		public static bool IsPathTag (this string value)
		{
			return value.StartsWith ("path-");
		}

		public static string FormatDescription (this string value)
		{
			value = value.NormalizePath ();
			if (value.StartsWith ("Bilder/")) {
				value = value.Substring (7);
			}
			value = regexDescriptionNotAllowed.Replace (value, string.Empty);
			return "Path: " + value;
		}

		public static bool IsDescription (this string value)
		{
			return value.StartsWith ("Path: ");
		}

		public static string FormatTitle (this string value)
		{
			return System.IO.Path.GetFileName (regexDescriptionNotAllowed.Replace (value, string.Empty));
		}

		public static string NormalizePath (this string path)
		{
			return path.Replace ("\\", "/").Trim ('/');
		}
	}
}

