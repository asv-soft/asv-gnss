using System;

namespace Asv.Gnss
{
    public static class GeoPointHelper
    {
        public static double DistanceTo(this GeoPoint a, GeoPoint b)
        {
            return GeoMath.Distance(a, b);
        }

        public static double Azimuth(this GeoPoint a, GeoPoint b)
        {
            return GeoMath.Azimuth(a.Latitude, a.Longitude, b.Latitude, b.Longitude);
        }

        public static GeoPoint RadialPoint(this GeoPoint point, double distance, double radialDeg)
        {
            return GeoMath.RadialPoint(point.Latitude, point.Longitude, point.Altitude, distance, radialDeg);
        }

        public static double AngleBetween(this GeoPoint a, GeoPoint b)
        {
            return a.Azimuth(b);
        }

        public static double PlanarDistance(this GeoPoint a, GeoPoint b)
        {
            double dx = a.Latitude - b.Latitude;
            double dy = a.Longitude - b.Longitude;
            return (double)Math.Sqrt((dx * dx) + (dy * dy));
        }
        public static double PlanarDistanceSquared(this GeoPoint a, GeoPoint b)
        {
            double dx = a.Latitude - b.Latitude;
            double dy = a.Longitude - b.Longitude;

            return (dx * dx) + (dy * dy);
        }

        public static GeoPoint AddAltitude(this GeoPoint point, double alt)
        {
            return new GeoPoint(point.Latitude, point.Longitude, (point.Altitude) + alt);
        }

        public static GeoPoint SetAltitude(this GeoPoint point, double alt)
        {
            return new GeoPoint(point.Latitude, point.Longitude, alt);
        }
    }
}
