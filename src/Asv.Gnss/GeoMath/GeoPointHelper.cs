using System;
using Asv.Common;

namespace Asv.Gnss
{
    /// <summary>
    /// Provides helper methods for performing various calculations related to geographical points.
    /// </summary>
    public static class GeoPointHelper
    {
        /// <summary>
        /// Calculates the distance between two geographic points.
        /// </summary>
        /// <param name="a">The first geographic point.</param>
        /// <param name="b">The second geographic point.</param>
        /// <returns>The distance between the two geographic points.</returns>
        public static double DistanceTo(this GeoPoint a, GeoPoint b)
        {
            return GeoMath.Distance(a, b);
        }

        /// <summary>
        /// Calculates the azimuth (bearing) from point A to point B.
        /// </summary>
        /// <param name="a">The starting GeoPoint.</param>
        /// <param name="b">The ending GeoPoint.</param>
        /// <returns>The azimuth, in degrees, from point A to point B.</returns>
        public static double Azimuth(this GeoPoint a, GeoPoint b)
        {
            return GeoMath.Azimuth(a.Latitude, a.Longitude, b.Latitude, b.Longitude);
        }

        /// <summary>
        /// Calculates the geographical point obtained by moving a specific distance along a radial direction from a given point.
        /// </summary>
        /// <param name="point">The starting geographical point.</param>
        /// <param name="distance">The distance to move along the radial direction.</param>
        /// <param name="radialDeg">The radial direction in degrees.</param>
        /// <returns>The geographical point obtained by moving the specified distance along the radial direction from the starting point.</returns>
        public static GeoPoint RadialPoint(this GeoPoint point, double distance, double radialDeg)
        {
            return GeoMath.RadialPoint(
                point.Latitude,
                point.Longitude,
                point.Altitude,
                distance,
                radialDeg
            );
        }

        /// <summary>
        /// Calculate the angle between two GeoPoints.
        /// </summary>
        /// <param name="a">The first GeoPoint.</param>
        /// <param name="b">The second GeoPoint.</param>
        /// <returns>The angle between the two GeoPoints.</returns>
        public static double AngleBetween(this GeoPoint a, GeoPoint b)
        {
            return a.Azimuth(b);
        }

        /// <summary>
        /// Calculates the planar distance between two geographic points.
        /// </summary>
        /// <param name="a">The first GeoPoint.</param>
        /// <param name="b">The second GeoPoint.</param>
        /// <returns>The planar distance between the two GeoPoints.</returns>
        public static double PlanarDistance(this GeoPoint a, GeoPoint b)
        {
            double dx = a.Latitude - b.Latitude;
            double dy = a.Longitude - b.Longitude;
            return (double)Math.Sqrt((dx * dx) + (dy * dy));
        }

        /// <summary>
        /// Calculates the squared planar distance between two GeoPoints.
        /// </summary>
        /// <param name="a">The first GeoPoint.</param>
        /// <param name="b">The second GeoPoint.</param>
        /// <returns>The squared planar distance between the two GeoPoints.</returns>
        public static double PlanarDistanceSquared(this GeoPoint a, GeoPoint b)
        {
            double dx = a.Latitude - b.Latitude;
            double dy = a.Longitude - b.Longitude;

            return (dx * dx) + (dy * dy);
        }

        /// <summary>
        /// Adds the specified altitude to a <see cref="GeoPoint"/>.
        /// </summary>
        /// <param name="point">The <see cref="GeoPoint"/> to add altitude to.</param>
        /// <param name="alt">The altitude to add.</param>
        /// <returns>A new <see cref="GeoPoint"/> with the added altitude.</returns>
        public static GeoPoint AddAltitude(this GeoPoint point, double alt)
        {
            return new GeoPoint(point.Latitude, point.Longitude, point.Altitude + alt);
        }

        /// <summary>
        /// Sets the altitude of a GeoPoint object.
        /// </summary>
        /// <param name="point">The original GeoPoint object.</param>
        /// <param name="alt">The new altitude value.</param>
        /// <returns>A new GeoPoint object with the updated altitude.</returns>
        public static GeoPoint SetAltitude(this GeoPoint point, double alt)
        {
            return new GeoPoint(point.Latitude, point.Longitude, alt);
        }
    }
}
