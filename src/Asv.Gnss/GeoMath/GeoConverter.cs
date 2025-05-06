using System;
using Asv.Common;
#pragma warning disable SA1407, SA1300, SA1313
namespace Asv.Gnss
{
    // The GeoConverter class provides methods for converting coordinates between different coordinate systems.
    // /
    public static class GeoConverter
    {
        /// <summary>
        /// Represents the mathematical constant Pi. </summary>
        /// /
        private const double Pi = 3.14159265358979; // Число Пи
        private const double Ro = 206264.8062; // Число угловых секунд в радиане

        // Эллипсоид Красовского

        /// <summary>
        /// Represents the major axis of an ellipse used in geodetic calculations.
        /// </summary>
        private const double AP = 6378136; // Большая полуось

        /// <summary>
        /// The flattening value of the ellipsoid used for geodetic calculations. </summary>
        /// <remarks>
        /// The flattening value indicates the amount of compression of the ellipsoid along its polar axis.
        /// It is used in geodetic calculations to account for the shape of the Earth.
        /// This value is defined as the inverse of the ellipticity. </remarks>
        /// /
        private const double AlP = 1 / 298.257839303; // Сжатие

        /// <summary>
        /// The square of the eccentricity.
        /// </summary>
        private static readonly double E2P = (2 * AlP) - Math.Pow(AlP, 2); // Квадрат эксцентриситета

        // Элипсоид WGS84 (GRS80, эти два эллипсоида сходны по большинству параметров)
        // Represents the length of the semi-major axis of the Earth's ellipsoid in meters.
        // </summary>
        private const double AW = 6378137; // Большая полуось

        /// <summary>
        /// Constant variable representing the compression of the Earth ellipsoid.
        /// </summary>
        private const double AlW = 1 / 298.257223563; // Сжатие

        /// <summary>
        /// Represents the square eccentricity (Квадрат эксцентриситета).
        /// </summary>
        private static readonly double E2W = (2 * AlW) - Math.Pow(AlW, 2); // Квадрат эксцентриситета

        // Вспомогательные значения для преобразования эллипсоидов

        /// <summary>
        /// Represents the average value of two variables: aP and aW.
        /// </summary>
        private const double A = (AP + AW) / 2;

        /// <summary>
        /// The e2 variable represents the average value of e2P and e2W.
        /// </summary>
        /// <remarks>
        /// e2P and e2W are two separate variables and their values are used to compute the average value stored in e2.
        /// </remarks>
        private static readonly double E2 = (E2P + E2W) / 2;

        /// <summary>
        /// This variable represents the difference between the value of 'aW' and 'aP'.
        /// </summary>
        private const double Da = AW - AP;

        /// <summary>
        /// Represents the subtraction of two double variables.
        /// </summary>
        private static readonly double De2 = E2W - E2P;

        // Линейные элементы трансформирования, в метрах

        /// <summary>
        /// The value of the variable dx_42_84 is 28.
        /// </summary>
        private const double Dx_42_84 = 28;

        /// <summary>
        /// Represents the value of dy_42_84.
        /// </summary>
        private const double Dy_42_84 = -130;

        /// <summary>
        /// Represents the value of dz_42_84.
        /// </summary>
        private const double Dz_42_84 = -95;

        /// <summary>
        /// Represents the value of dx_90_84.
        /// </summary>
        private const double Dx_90_84 = -1.08;

        /// <summary>
        /// Represents the value of the variable dy_90_84.
        /// </summary>
        private const double Dy_90_84 = -0.27;

        /// <summary>
        /// This variable represents the value of dz_90_84.
        /// </summary>
        private const double Dz_90_84 = -0.9;

        // Угловые элементы трансформирования, в секундах

        /// <summary>
        /// Represents the value of wx.
        /// </summary>
        private const double Wx = 0;

        /// <summary>
        /// Represents the value of variable wy.
        /// </summary>
        private const double Wy = 0;

        /// <summary>
        /// Represents the value of wz, which is a private static double variable.
        /// </summary>
        private const double Wz = 0;

        // Дифференциальное различие масштабов

        /// <summary>
        /// Stores the value of milliseconds.
        /// </summary>
        private const double Ms = 0;

        /// <summary>
        /// Converts a point from the PZ-90 coordinate system to the WGS84 coordinate system.
        /// </summary>
        /// <param name="point">The point to convert.</param>
        /// <returns>A new GeoPoint object representing the converted point.</returns>
        public static GeoPoint PZ90_WGS84(this GeoPoint point)
        {
            var lat = PZ90_WGS84_Lat(point.Latitude, point.Longitude, point.Altitude);
            var lon = PZ90_WGS84_Long(point.Latitude, point.Longitude, point.Altitude);
            var alt = WGS84Alt(
                point.Latitude,
                point.Longitude,
                point.Altitude,
                Dx_90_84,
                Dy_90_84,
                Dz_90_84
            );
            return new GeoPoint(lat, lon, alt);
        }

        /// <summary>
        /// Converts a geographic point from WGS84 to PZ90 coordinate system.
        /// </summary>
        /// <param name="point">The geographic point in WGS84 coordinate system.</param>
        /// <returns>The geographic point in PZ90 coordinate system.</returns>
        public static GeoPoint WGS84_PZ90(this GeoPoint point)
        {
            var lat = WGS84_PZ90_Lat(point.Latitude, point.Longitude, point.Altitude);
            var lon = WGS84_PZ90_Long(point.Latitude, point.Longitude, point.Altitude);
            var alt = WGS84Alt(
                point.Latitude,
                point.Longitude,
                point.Altitude,
                Dx_90_84,
                Dy_90_84,
                Dz_90_84
            );
            return new GeoPoint(lat, lon, alt);
        }

        /// <summary>
        /// Calculates the dB value based on the given inputs. </summary> <param name="Bd">The value of Bd.</param> <param name="Ld">The value of Ld.</param> <param name="H">The value of H.</param> <param name="dx">The value of dx.</param> <param name="dy">The value of dy.</param> <param name="dz">The value of dz.</param> <returns>
        /// The calculated dB value. </returns>
        /// /
        private static double dB(double Bd, double Ld, double H, double dx, double dy, double dz)
        {
            double b,
                l,
                m,
                n;
            b = Bd * Pi / 180;
            l = Ld * Pi / 180;
            m = A * (1 - E2) / Math.Pow(1 - (E2 * Math.Pow(Math.Sin(b), 2)), 1.5);
            n = A * Math.Pow(1 - (E2 * Math.Pow(Math.Sin(b), 2)), -0.5);

            return (
                    Ro
                    / (m + H)
                    * (
                        (n / A * E2 * Math.Sin(b) * Math.Cos(b) * Da)
                        + (
                            ((Math.Pow(n, 2) / Math.Pow(A, 2)) + 1)
                            * n
                            * Math.Sin(b)
                            * Math.Cos(b)
                            * De2
                            / 2
                        )
                        - (((dx * Math.Cos(l)) + (dy * Math.Sin(l))) * Math.Sin(b))
                        + (dz * Math.Cos(b))
                    )
                )
                - (Wx * Math.Sin(l) * (1 + (E2 * Math.Cos(2 * b))))
                + (Wy * Math.Cos(l) * (1 + (E2 * Math.Cos(2 * b))))
                - (Ro * Ms * E2 * Math.Sin(b) * Math.Cos(b));
        }

        /// <summary>
        /// Calculate the dL value using the given parameters.
        /// </summary>
        /// <param name="Bd">The latitude in degrees.</param>
        /// <param name="Ld">The longitude in degrees.</param>
        /// <param name="H">The height.</param>
        /// <param name="dx">The change in x coordinate.</param>
        /// <param name="dy">The change in y coordinate.</param>
        /// <param name="dz">The change in z coordinate.</param>
        /// <returns>The calculated dL value.</returns>
        private static double dL(double Bd, double Ld, double H, double dx, double dy, double dz)
        {
            double b,
                l,
                n;
            b = Bd * Pi / 180;
            l = Ld * Pi / 180;
            n = A * Math.Pow(1 - (E2 * Math.Pow(Math.Sin(b), 2)), -0.5);
            return (Ro / ((n + H) * Math.Cos(b)) * ((-dx * Math.Sin(l)) + (dy * Math.Cos(l))))
                + (Math.Tan(b) * (1 - E2) * ((Wx * Math.Cos(l)) + (Wy * Math.Sin(l))))
                - Wz;
        }

        /// <summary>
        /// Calculates the WGS84 altitude based on geodetic latitude, longitude, height, and coordinate differences.
        /// </summary>
        /// <param name="Bd">The geodetic latitude in degrees.</param>
        /// <param name="Ld">The geodetic longitude in degrees.</param>
        /// <param name="H">The geodetic height.</param>
        /// <param name="dx">The difference in x-coordinate.</param>
        /// <param name="dy">The difference in y-coordinate.</param>
        /// <param name="dz">The difference in z-coordinate.</param>
        /// <returns>
        /// The WGS84 altitude calculated based on the geodetic latitude, longitude, height, and coordinate differences.
        /// </returns>
        private static double WGS84Alt(
            double Bd,
            double Ld,
            double H,
            double dx,
            double dy,
            double dz
        )
        {
            double b,
                l,
                n,
                dH;
            b = Bd * Pi / 180;
            l = Ld * Pi / 180;
            n = A * Math.Pow(1 - (E2 * Math.Pow(Math.Sin(b), 2)), -0.5);
            dH =
                (-A / n * Da)
                + (n * Math.Pow(Math.Sin(b), 2) * De2 / 2)
                + (((dx * Math.Cos(l)) + (dy * Math.Sin(l))) * Math.Cos(b))
                + (dz * Math.Sin(b))
                - (
                    n
                    * E2
                    * Math.Sin(b)
                    * Math.Cos(b)
                    * ((Wx / Ro * Math.Sin(l)) - (Wy / Ro * Math.Cos(l)))
                )
                + (((Math.Pow(A, 2) / n) + H) * Ms);
            return H + dH;
        }

        /// <summary>
        /// Converts latitude from PZ90 to WGS84 coordinate system.
        /// </summary>
        /// <param name="Bd">Latitude in PZ90 coordinate system.</param>
        /// <param name="Ld">Longitude in PZ90 coordinate system.</param>
        /// <param name="H">Height in meters.</param>
        /// <returns>
        /// Latitude in WGS84 coordinate system.
        /// </returns>
        private static double PZ90_WGS84_Lat(double Bd, double Ld, double H)
        {
            return Bd + (dB(Bd, Ld, H, Dx_90_84, Dy_90_84, Dz_90_84) / 3600);
        }

        /// <summary>
        /// Converts longitude coordinates from PZ90 system to WGS84 system.
        /// </summary>
        /// <param name="Bd">The latitude coordinate in PZ90 system.</param>
        /// <param name="Ld">The longitude coordinate in PZ90 system.</param>
        /// <param name="H">The height coordinate in PZ90 system.</param>
        /// <returns>The longitude coordinate in WGS84 system.</returns>
        private static double PZ90_WGS84_Long(double Bd, double Ld, double H)
        {
            return Ld + (dL(Bd, Ld, H, Dx_90_84, Dy_90_84, Dz_90_84) / 3600);
        }

        /// <summary>
        /// Converts latitude from WGS84 to PZ90 coordinate system.
        /// </summary>
        /// <param name="Bd">Latitude in WGS84 coordinate system.</param>
        /// <param name="Ld">Longitude in WGS84 coordinate system.</param>
        /// <param name="H">Altitude in WGS84 coordinate system.</param>
        /// <returns>
        /// Latitude in PZ90 coordinate system.
        /// </returns>
        private static double WGS84_PZ90_Lat(double Bd, double Ld, double H)
        {
            return Bd - (dB(Bd, Ld, H, Dx_90_84, Dy_90_84, Dz_90_84) / 3600);
        }

        /// <summary>
        /// Converts a longitude from WGS84 to PZ90 coordinate system.
        /// </summary>
        /// <param name="Bd">The latitude value in degrees.</param>
        /// <param name="Ld">The longitude value in degrees.</param>
        /// <param name="H">The altitude value in meters.</param>
        /// <returns>The converted longitude value in PZ90 coordinate system.</returns>
        private static double WGS84_PZ90_Long(double Bd, double Ld, double H)
        {
            return Ld - (dL(Bd, Ld, H, Dx_90_84, Dy_90_84, Dz_90_84) / 3600);
        }

        /// <summary>
        /// Calculates the longitude in CK42 coordinates from WGS84 coordinates.
        /// </summary>
        /// <param name="Bd">The latitude in WGS84 coordinates.</param>
        /// <param name="Ld">The longitude in WGS84 coordinates.</param>
        /// <param name="H">The height in WGS84 coordinates.</param>
        /// <returns>The longitude in CK42 coordinates.</returns>
        public static double WGS84_CK42_Long(double Bd, double Ld, double H)
        {
            return Ld - (dL(Bd, Ld, H, Dx_42_84, Dy_42_84, Dz_42_84) / 3600);
        }
    }
}
#pragma warning disable SA1300
#pragma warning restore SA1407
