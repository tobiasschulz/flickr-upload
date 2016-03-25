using System;
using ExifLib;
using Newtonsoft.Json;
using System.Globalization;

namespace FlickrUpload
{
	public static class ExifHelper
	{
		public static GeoLocation GetLatlngFromImage (string imagePath)
		{
			GeoLocation result = GeoLocation.Zero;

			using (var stream = System.IO.File.OpenRead (imagePath)) {
				JpegInfo info;
				try {
					info = ExifReader.ReadJpeg (stream);
				} catch (Exception) {
					Console.WriteLine ("Warning: unable to get jpeg info from: " + imagePath);
					return GeoLocation.Zero;
				}

				// EXIF lat/lng tags stored as [Degree, Minute, Second]
				double[] latitudeComponents = info.GpsLatitude;
				double[] longitudeComponents = info.GpsLongitude;

				var latitudeRef = info.GpsLatitudeRef; // "N" or "S" ("S" will be negative latitude)
				var longitudeRef = info.GpsLongitudeRef; // "E" or "W" ("W" will be a negative longitude)

				if (latitudeComponents != null && longitudeComponents != null) {
					var latitude = ConvertDegreeAngleToDouble (latitudeComponents [0], latitudeComponents [1], latitudeComponents [2], latitudeRef);
					var longitude = ConvertDegreeAngleToDouble (longitudeComponents [0], longitudeComponents [1], longitudeComponents [2], longitudeRef);
					result = new GeoLocation (latitude, longitude);
				}
			}
			return result;
		}

		public static double ConvertDegreeAngleToDouble (double degrees, double minutes, double seconds, ExifGpsLatitudeRef latRef)
		{
			double result = ConvertDegreeAngleToDouble (degrees, minutes, seconds);
			if (latRef == ExifGpsLatitudeRef.South) {
				result *= -1;
			}
			return result;
		}

		public static double ConvertDegreeAngleToDouble (double degrees, double minutes, double seconds, ExifGpsLongitudeRef lngRef)
		{
			double result = ConvertDegreeAngleToDouble (degrees, minutes, seconds);
			if (lngRef == ExifGpsLongitudeRef.West) {
				result *= -1;
			}
			return result;
		}

		public static double ConvertDegreeAngleToDouble (double degrees, double minutes, double seconds)
		{
			return degrees + (minutes / 60) + (seconds / 3600);
		}
	}

	public class GeoLocation
	{
		[JsonProperty ("lat")]
		public double Lat { get; set; }

		[JsonProperty ("lng")]
		public double Lng { get; set; }

		public GeoLocation ()
		{
		}

		public GeoLocation (double lat, double lng)
		{
			Lat = lat;
			Lng = lng;
		}

		public static GeoLocation Zero { get; } = new GeoLocation(0,0);

		[JsonIgnore]
		public bool IsNonZero { get { return Lat != 0 && Lng != 0; } }

		[JsonIgnore]
		public bool IsZero { get { return Lat == 0 || Lng == 0; } }

		public string Serialize ()
		{
			if (IsZero) {
				return "null";
			} else {
				return string.Format ("{0},{1}", Lat.ToString (CultureInfo.InvariantCulture), Lng.ToString (CultureInfo.InvariantCulture));
			}
		}

		public static GeoLocation Deserialize (string s)
		{
			if (!string.IsNullOrWhiteSpace (s) && s != "null" && s != "NaN,NaN") {
				var p = s.Split (',');
				double d1, d2;
				if (p.Length == 2 && double.TryParse (p [0], NumberStyles.Float, CultureInfo.InvariantCulture, out d1) && double.TryParse (p [1], NumberStyles.Float, CultureInfo.InvariantCulture, out d2)) {
					return new GeoLocation (d1, d2);
				}
				if (p.Length == 4 && double.TryParse (p [0] + "." + p [1], NumberStyles.Float, CultureInfo.InvariantCulture, out d1) && double.TryParse (p [2] + "." + p [3], NumberStyles.Float, CultureInfo.InvariantCulture, out d2)) {
					return new GeoLocation (d1, d2);
				}
			}
			return GeoLocation.Zero;
		}

		public override string ToString ()
		{
			return Serialize ();
		}
	}
}

