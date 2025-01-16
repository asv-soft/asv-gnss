using System;
using System.Collections.Generic;
using Asv.Common;
using Geodesy;
using Angle = Geodesy.Angle;

namespace Asv.Gnss
{
    /// <summary>
    /// Converts degrees to radians.
    /// </summary>
    /// <returns>The angle in radians.</returns>
    public static class GeoMath
    {
        /// <summary>
        /// Represents a geodetic calculator for performing calculations on ellipsoids.
        /// </summary>
        private static readonly GeodeticCalculator Calculator = new(Ellipsoid.WGS84);

        /// <summary>
        /// Calculates the initial azimuth (the angle measured clockwise from true north) at a point from that point to a second point.
        /// </summary>
        /// <param name="latitude1">The latitude of the first point.</param>
        /// <param name="longitude1">The longitude of the first point.</param>
        /// <param name="latitude2">The latitude of the second point.</param>
        /// <param name="longitude2">The longitude of the second point.</param>
        /// <returns>The initial azimuth of the first point to the second point.</returns>
        /// <example>
        /// <code>
        /// double azimuth = Azimuth(0, 0, 1, 0); // azimuth = 0
        /// double azimuth = Azimuth(0, 0, 0, 1); // azimuth = 90
        /// </code>
        /// The azimuth from 0,0 to 1,0 is 0 degrees. From 0,0 to 0,1 is 90 degrees (due east). The range of the result is [-180, 180].
        /// </example>
        public static double Azimuth(
            double latitude1,
            double longitude1,
            double latitude2,
            double longitude2
        )
        {
            var measurement = Calculator.CalculateGeodeticMeasurement(
                new GlobalPosition(
                    new GlobalCoordinates(new Angle(latitude1), new Angle(longitude1))
                ),
                new GlobalPosition(
                    new GlobalCoordinates(new Angle(latitude2), new Angle(longitude2))
                )
            );

            return measurement.Azimuth.Degrees;
        }

        /// <summary>
        /// Calculates the great circle distance in meters between two points.
        /// </summary>
        /// <param name="point1">The location of the first point.</param>
        /// <param name="point2">The location of the second point.</param>
        /// <returns>The great circle distance in meters.</returns>
        /// <remarks>The antemeridian is not considered.</remarks>
        /// <exception cref="ArgumentNullException">Thrown when point1 or point2 is null.</exception>
        public static double Distance(GeoPoint point1, GeoPoint point2)
        {
            return Distance(
                point1.Latitude,
                point1.Longitude,
                point1.Altitude,
                point2.Latitude,
                point2.Longitude,
                point2.Altitude
            );
        }

        /// <summary>
        /// Calculates the great circle distance in meters between two points on the Earth's surface.
        /// </summary>
        /// <param name="latitude1">The latitude of the first point.</param>
        /// <param name="longitude1">The longitude of the first point.</param>
        /// <param name="latitude2">The latitude of the second point.</param>
        /// <param name="longitude2">The longitude of the second point.</param>
        /// <returns>The great circle distance in meters.</returns>
        /// <remarks>The antemeridian is not considered.</remarks>
        public static double Distance(
            double latitude1,
            double longitude1,
            double latitude2,
            double longitude2
        )
        {
            var measurement = Calculator.CalculateGeodeticMeasurement(
                new GlobalPosition(
                    new GlobalCoordinates(new Angle(latitude1), new Angle(longitude1))
                ),
                new GlobalPosition(
                    new GlobalCoordinates(new Angle(latitude2), new Angle(longitude2))
                )
            );

            return measurement.EllipsoidalDistance;
        }

        /// <summary>
        /// Calculates the great circle distance in meters between two points in all three dimensions.
        /// </summary>
        /// <param name="latitude1">The latitude of the first point.</param>
        /// <param name="longitude1">The longitude of the first point.</param>
        /// <param name="altitude1">The altitude of the first point.</param>
        /// <param name="latitude2">The latitude of the second point.</param>
        /// <param name="longitude2">The longitude of the second point.</param>
        /// <param name="altitude2">The altitude of the second point.</param>
        /// <returns>The great circle distance in meters.</returns>
        /// <remarks>The antemeridian is not considered.</remarks>
        public static double Distance(
            double latitude1,
            double longitude1,
            double altitude1,
            double latitude2,
            double longitude2,
            double altitude2
        )
        {
            var measurement = Calculator.CalculateGeodeticMeasurement(
                new GlobalPosition(
                    new GlobalCoordinates(new Angle(latitude1), new Angle(longitude1)),
                    altitude1
                ),
                new GlobalPosition(
                    new GlobalCoordinates(new Angle(latitude2), new Angle(longitude2)),
                    altitude2
                )
            );

            return measurement.PointToPointDistance;
        }

        /// <summary>
        /// Calculates the elevation difference between two geographic points.
        /// </summary>
        /// <param name="p1">The first geographic point.</param>
        /// <param name="p2">The second geographic point.</param>
        /// <returns>The elevation difference between the two geographic points.</returns>
        public static double Elevation(GeoPoint p1, GeoPoint p2)
        {
            return Elevation(
                p1.Latitude,
                p1.Longitude,
                p1.Altitude,
                p2.Latitude,
                p2.Longitude,
                p2.Altitude
            );
        }

        /// <summary>
        /// Calculates the angle from the horizontal plane between the two altitudes.
        /// </summary>
        /// <param name="latitude1">The latitude of the first point.</param>
        /// <param name="longitude1">The longitude of the first point.</param>
        /// <param name="altitude1">The altitude of the first point.</param>
        /// <param name="latitude2">The latitude of the second point.</param>
        /// <param name="longitude2">The longitude of the second point.</param>
        /// <param name="altitude2">The altitude of the second point.</param>
        /// <returns>The angle from the horizontal plane in degrees.</returns>
        /// <remarks>
        /// This is a naive implementation accurate only over short distances
        /// and does not account for surface curvature. To use this as the value
        /// of KML's tilt, add 90 degrees (since in KML a tilt of 0 is vertical).
        /// </remarks>
        /// <example>
        /// The returned angle from (37.00, -121.98, 600) to a point about 1778
        /// meters west, 400 meters below at (37.00, -122.00, 200) is -12.7 degrees.
        /// </example>
        public static double Elevation(
            double latitude1,
            double longitude1,
            double altitude1,
            double latitude2,
            double longitude2,
            double altitude2
        )
        {
            double surfaceDistance = Distance(latitude1, longitude1, latitude2, longitude2);
            return RadiansToDegrees(Math.Atan2(altitude2 - altitude1, surfaceDistance));
        }

        /// <summary>
        /// Calculates the absolute distance between the ground point and the point directly under the end of the specified vector.
        /// </summary>
        /// <param name="range">The distance in meters.</param>
        /// <param name="elevation">The angle in degrees from the horizontal plane.</param>
        /// <returns>The absolute ground distance in meters.</returns>
        public static double GroundDistance(double range, double elevation)
        {
            return Math.Abs(Math.Cos(DegreesToRadians(elevation)) * range);
        }

        /// <summary>
        /// Calculates the absolute height of a point given the range and elevation angle.
        /// </summary>
        /// <param name="range">The distance in meters.</param>
        /// <param name="elevation">
        /// The angle in degrees from the horizontal plane.
        /// </param>
        /// <returns>The absolute height in meters.</returns>
        public static double Height(double range, double elevation)
        {
            return Math.Abs(Math.Sin(DegreesToRadians(elevation)) * range);
        }

        /// <summary>
        /// Calculates the height of a point given the ground range and the elevation angle.
        /// </summary>
        /// <param name="groundRange">The distance in meters on the ground.</param>
        /// <param name="elevation">The angle in degrees from the horizontal plane.</param>
        /// <returns>The absolute height in meters.</returns>
        public static double HeightFromGroundRange(double groundRange, double elevation)
        {
            return Math.Abs(Math.Tan(DegreesToRadians(elevation)) * groundRange);
        }

        /// <summary>
        /// Calculate the ground range in meters from the given height and elevation angle.
        /// </summary>
        /// <param name="height">The height of the object in meters.</param>
        /// <param name="elevation">The angle in degrees from the horizontal plane.</param>
        /// <returns>The ground range in meters.</returns>
        public static double GroundRangeFromHeight(double height, double elevation)
        {
            return Math.Abs(height / Math.Tan(DegreesToRadians(elevation)));
        }

        /// <summary>
        /// Finds the point on the given line that is the intersection of the perpendicular dropped from the given point to the line (height is not considered).
        /// </summary>
        /// <param name="lineX">The first point on the line.</param>
        /// <param name="lineY">The second point on the line.</param>
        /// <param name="p">The point from which to drop the perpendicular.</param>
        /// <returns>The intersection point of the line and the perpendicular from the given point.</returns>
        public static GeoPoint IntersectionLineAndPerpendicularFromPoint(
            GeoPoint lineX,
            GeoPoint lineY,
            GeoPoint p
        )
        {
            if (lineX.Equals(lineY))
            {
                return p; // если прямая задана одинаковыми точками
            }

            var azimuth = DegreesToRadians(lineX.Azimuth(lineY) - lineX.Azimuth(p));
            var d = Distance(lineX, p);
            var h = Math.Abs(d * Math.Cos(azimuth));
            return lineX.RadialPoint(h, lineX.Azimuth(lineY));
        }

        /// <summary>
        /// Finds the point that is the intersection of the perpendicular dropped from a given point to a line passing at a given angle (height is not taken into account).
        /// </summary>
        /// <param name="lineX">The starting point of the line.</param>
        /// <param name="lineY">The ending point of the line.</param>
        /// <param name="alpha">The angle at which the line is oriented.</param>
        /// <returns>The point that is the intersection of the perpendicular and the line.</returns>
        public static GeoPoint IntersectionLineAndPerpendicularFromPoint(
            GeoPoint lineX,
            GeoPoint lineY,
            double alpha
        )
        {
            var azimuth = lineX.Azimuth(lineY) + alpha;
            var b = Distance(lineX, lineY);
            var c = Math.Abs(b / Math.Cos(DegreesToRadians(alpha)));
            return lineX.RadialPoint(c, azimuth);
        }

        /// <summary>
        /// Finds the shortest distance (perpendicular length) between a point and a given line (height not taken into account).
        /// </summary>
        /// <param name="lineX">The starting point of the line.</param>
        /// <param name="lineY">The ending point of the line.</param>
        /// <param name="p">The point to calculate the perpendicular length from.</param>
        /// <returns>The perpendicular length between the point and the line.</returns>
        public static double PerpendicularLength(GeoPoint lineX, GeoPoint lineY, GeoPoint p)
        {
            var azimuth = lineX.Azimuth(lineY) - lineX.Azimuth(p);
            var d = Distance(lineX, p);
            return Math.Abs(d * Math.Sin(azimuth * Math.PI / 180));
        }

        /// <summary>
        /// Calculates a point at the specified distance along a radial from a center point.
        /// </summary>
        /// <param name="latitude">The latitude of the center point.</param>
        /// <param name="longitude">The longitude of the center point.</param>
        /// <param name="distance">The distance in meters.</param>
        /// <param name="radialDeg">The radial in degrees, measures clockwise from north.</param>
        /// <returns>A GeoPoint containing the Latitude and Longitude of the calculated point.</returns>
        /// <remarks>The antemeridian is not considered.</remarks>
        public static GeoPoint RadialPoint(
            double latitude,
            double longitude,
            double altitude,
            double distance,
            double radialDeg
        )
        {
            radialDeg = !double.IsNaN(radialDeg) ? radialDeg : 0;
            var coordinates = Calculator.CalculateEndingGlobalCoordinates(
                new GlobalCoordinates(new Angle(latitude), new Angle(longitude)),
                new Angle(radialDeg),
                distance
            );

            return new GeoPoint(
                coordinates.Latitude.Degrees,
                coordinates.Longitude.Degrees,
                altitude
            );
        }

        /// <summary>Converts the specified value in radians to degrees.</summary>
        /// <param name="radians">The angle in radians.</param>
        /// <returns>The specified angle converted to degrees.</returns>
        public static double RadiansToDegrees(double radians)
        {
            return radians * 180.0 / Math.PI;
        }

        /// <summary>
        /// Converts the specified value in degrees to radians.
        /// </summary>
        /// <param name="degrees">The angle in degrees.</param>
        /// <returns>The specified angle converted to radians.</returns>
        public static double DegreesToRadians(double degrees)
        {
            return degrees * Math.PI / 180.0;
        }

        /// <summary>
        /// Generate wagging path points
        /// [2]     [4]
        /// [START]--/   \   /   \--[STOP]
        /// [3].
        /// </summary>
        /// <param name="start">The starting point of the path.</param>
        /// <param name="stop">The ending point of the path.</param>
        /// <param name="convergencePoint">The point of convergence for the wagging path.</param>
        /// <param name="waggingCountsValue">The number of sub-points to generate between the start and stop points.</param>
        /// <param name="deviationFromCenterLineInMeters">The maximum deviation from the center line of the wagging path in meters.</param>
        /// <param name="minAltitudeInMeters">The minimum altitude value for the generated points.</param>
        /// <returns>An enumerable collection of GeoPoint representing the wagging path points.</returns>
        public static IEnumerable<GeoPoint> GenerateWaggingLatLonPoints(
            GeoPoint start,
            GeoPoint stop,
            GeoPoint convergencePoint,
            int waggingCountsValue,
            double deviationFromCenterLineInMeters,
            double minAltitudeInMeters
        )
        {
            var distanceToStart = convergencePoint.DistanceTo(start);
            var distanceToStop = convergencePoint.DistanceTo(stop);
            var factor =
                distanceToStart > distanceToStop
                    ? deviationFromCenterLineInMeters / distanceToStart
                    : deviationFromCenterLineInMeters / distanceToStop;

            if (waggingCountsValue <= 0 || deviationFromCenterLineInMeters <= 0)
            {
                // this is simple path from start to stop without sub points
                yield return start;
                yield return stop;
            }
            else
            {
                var altDiff = stop.Altitude - start.Altitude;
                var azimuth = start.Azimuth(stop);
                var distance = start.DistanceTo(stop);
                var incDistance = distance / (waggingCountsValue + 1);
                var incAlt = altDiff / (waggingCountsValue + 1);
                var m = distanceToStart > distanceToStop ? -1 : 1;

                yield return start;
                var currentDist = 0.0;
                var currentAlt = 0.0;
                for (var i = 0; i < waggingCountsValue; i++)
                {
                    currentDist += incDistance;
                    currentAlt += incAlt;
                    var alt = start.Altitude + currentAlt;
                    alt = alt < minAltitudeInMeters ? minAltitudeInMeters : alt;
                    yield return start
                        .RadialPoint(currentDist, azimuth)
                        .RadialPoint(
                            (distanceToStart + (m * currentDist)) * factor,
                            azimuth + (i % 2 == 0 ? -90 : 90)
                        )
                        .SetAltitude(alt);
                }

                yield return stop;
            }
        }

        /// <summary>
        /// Generate wagging path points.
        /// </summary>
        /// <param name="start">The starting point of the path.</param>
        /// <param name="stop">The ending point of the path.</param>
        /// <param name="convergencePoint">The convergence point of the path.</param>
        /// <param name="waggingCountsValue">The number of wagging counts.</param>
        /// <param name="deviationFromCenterLineInMeters">The deviation from the center line in meters.</param>
        /// <param name="minAltitudeInMeters">The minimum altitude in meters.</param>
        /// <returns>
        /// An enumerable collection of GeoPoint objects representing the generated wagging path points.
        /// </returns>
        public static IEnumerable<GeoPoint> GenerateWaggingAltPoints(
            GeoPoint start,
            GeoPoint stop,
            GeoPoint convergencePoint,
            int waggingCountsValue,
            double deviationFromCenterLineInMeters,
            double minAltitudeInMeters
        )
        {
            if (waggingCountsValue <= 0 || deviationFromCenterLineInMeters <= 0)
            {
                // this is simple path from start to stop without sub points
                yield return start;
                yield return stop;
            }
            else
            {
                var isApproach = true;
                var pointDistance = start.DistanceTo(stop);
                if (pointDistance == 0)
                {
                    yield return start;
                    yield return stop;
                }

                var aimingToStartDistance = convergencePoint.DistanceTo(start);
                var aimingToStopDistance = convergencePoint.DistanceTo(stop);
                var maxAltDev = deviationFromCenterLineInMeters;
                var minAltDev = maxAltDev * aimingToStopDistance / aimingToStartDistance;
                if (aimingToStartDistance < aimingToStopDistance)
                {
                    isApproach = false;
                    minAltDev = maxAltDev * aimingToStartDistance / aimingToStopDistance;
                }

                var upAltDiff = isApproach
                    ? (start.Altitude + maxAltDev - (stop.Altitude + minAltDev))
                    : (stop.Altitude + maxAltDev - (start.Altitude + minAltDev));
                var downAltDiff = isApproach
                    ? (start.Altitude - maxAltDev - (stop.Altitude - minAltDev))
                    : (stop.Altitude - maxAltDev - (start.Altitude - minAltDev));

                var azimuth = start.Azimuth(stop);
                var incDistance = pointDistance / (waggingCountsValue + 1);
                var incUpAlt = upAltDiff / (waggingCountsValue + 1);
                var incDownAlt = downAltDiff / (waggingCountsValue + 1);

                yield return start;
                var currentDist = 0.0;
                var currentUpAlt = 0.0;
                var currentDownAlt = 0.0;

                if (isApproach)
                {
                    var startUpAltitude = start.Altitude + maxAltDev;
                    var startDownAltitude = start.Altitude - maxAltDev;
                    for (var i = 0; i < waggingCountsValue; i++)
                    {
                        currentDist += incDistance;
                        currentUpAlt += incUpAlt;
                        currentDownAlt += incDownAlt;
                        var alt =
                            i % 2 == 0
                                ? startDownAltitude - currentDownAlt
                                : startUpAltitude - currentUpAlt;
                        if (alt < minAltitudeInMeters)
                        {
                            alt = minAltitudeInMeters;
                        }

                        yield return start.RadialPoint(currentDist, azimuth).SetAltitude(alt);
                    }
                }
                else
                {
                    var startUpAltitude = start.Altitude + minAltDev;
                    var startDownAltitude = start.Altitude - minAltDev;
                    for (var i = 0; i < waggingCountsValue; i++)
                    {
                        currentDist += incDistance;
                        currentUpAlt += incUpAlt;
                        currentDownAlt += incDownAlt;
                        var alt =
                            i % 2 == 0
                                ? startDownAltitude + currentDownAlt
                                : startUpAltitude + currentUpAlt;
                        if (alt < minAltitudeInMeters)
                        {
                            alt = minAltitudeInMeters;
                        }

                        yield return start.RadialPoint(currentDist, azimuth).SetAltitude(alt);
                    }
                }

                yield return stop;
            }
        }
    }
}
