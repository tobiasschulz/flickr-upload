using System;
using ExifLib;

namespace FlickrUpload
{
	public static class ExifHelper
	{
		public static GeoLocation GetLatlngFromImage (string imagePath)
		{
			GeoLocation result = null;

			using (var stream = System.IO.File.OpenRead (imagePath)) {
				JpegInfo info;
				try {
					info = ExifReader.ReadJpeg (stream);
				} catch (Exception) {
					Console.WriteLine ("Warning: unable to get jpeg info from: " + imagePath);
					return null;
				}

				// EXIF lat/lng tags stored as [Degree, Minute, Second]
				double[] latitudeComponents = info.GpsLatitude;
				double[] longitudeComponents = info.GpsLongitude;

				var latitudeRef = info.GpsLatitudeRef; // "N" or "S" ("S" will be negative latitude)
				var longitudeRef = info.GpsLongitudeRef; // "E" or "W" ("W" will be a negative longitude)

				if (latitudeComponents != null && longitudeComponents != null) {
					var latitude = ConvertDegreeAngleToDouble (latitudeComponents [0], latitudeComponents [1], latitudeComponents [2], latitudeRef);
					var longitude = ConvertDegreeAngleToDouble (longitudeComponents [0], longitudeComponents [1], longitudeComponents [2], longitudeRef);
					if (Math.Abs (latitude) > 0.1 && Math.Abs (longitude) > 0.1) {
						result = new GeoLocation { Lat = latitude, Lng = longitude };
					}
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
		public double Lat { get; set; }

		public double Lng { get; set; }
	}
}

