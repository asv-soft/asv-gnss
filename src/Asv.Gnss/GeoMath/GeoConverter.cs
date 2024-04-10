using System;
using Asv.Common;

namespace Asv.Gnss
{
    /// The GeoConverter class provides methods for converting coordinates between different coordinate systems.
    /// /
    public static class GeoConverter
    {
        /// <summary>
        /// Represents the mathematical constant Pi. </summary>
        /// /
        private static double Pi = 3.14159265358979;             // Число Пи
        private static double ro = 206264.8062;                  // Число угловых секунд в радиане

        // Эллипсоид Красовского
        /// <summary>
        /// Represents the major axis of an ellipse used in geodetic calculations.
        /// </summary>
        private static double aP  = 6378136;                     // Большая полуось

        /// <summary>
        /// The flattening value of the ellipsoid used for geodetic calculations. </summary>
        /// <remarks>
        /// The flattening value indicates the amount of compression of the ellipsoid along its polar axis.
        /// It is used in geodetic calculations to account for the shape of the Earth.
        /// This value is defined as the inverse of the ellipticity. </remarks>
        /// /
        private static double alP = 1 / 298.257839303;           // Сжатие

        /// <summary>
        /// The square of the eccentricity.
        /// </summary>
        private static double e2P = 2 * alP - Math.Pow(alP,2);   // Квадрат эксцентриситета

        // Элипсоид WGS84 (GRS80, эти два эллипсоида сходны по большинству параметров)
        /// Represents the length of the semi-major axis of the Earth's ellipsoid in meters.
        /// </summary>
        private static double aW  = 6378137;                     // Большая полуось

        /// <summary>
        /// Constant variable representing the compression of the Earth ellipsoid.
        /// </summary>
        private static double alW = 1 / 298.257223563;           // Сжатие

        /// <summary>
        /// Represents the square eccentricity (Квадрат эксцентриситета).
        /// </summary>
        private static double e2W = 2 * alW - Math.Pow(alW,2);    // Квадрат эксцентриситета

        // Вспомогательные значения для преобразования эллипсоидов
        /// <summary>
        /// Represents the average value of two variables: aP and aW.
        /// </summary>
        private static double a   = (aP + aW) / 2;

        /// <summary>
        /// The e2 variable represents the average value of e2P and e2W.
        /// </summary>
        /// <remarks>
        /// e2P and e2W are two separate variables and their values are used to compute the average value stored in e2.
        /// </remarks>
        private static double e2  = (e2P + e2W) / 2;

        /// <summary>
        /// This variable represents the difference between the value of 'aW' and 'aP'.
        /// </summary>
        private static double da  = aW - aP;

        /// <summary>
        /// Represents the subtraction of two double variables.
        /// </summary>
        private static double de2 = e2W - e2P;

        // Линейные элементы трансформирования, в метрах
        /// <summary>
        /// The value of the variable dx_42_84 is 28.
        /// </summary>
        private static double dx_42_84 = 28;

        /// <summary>
        /// Represents the value of dy_42_84.
        /// </summary>
        private static double dy_42_84 = -130;

        /// <summary>
        /// Represents the value of dz_42_84.
        /// </summary>
        private static double dz_42_84 = -95;

        /// <summary>
        /// The value of dx_42_90.
        /// </summary>
        private static double dx_42_90 = 23.92;

        /// <summary>
        /// Represents the value of dy_42_90.
        /// </summary>
        private static double dy_42_90 = -141.27;

        /// <summary>
        /// The value of dz_42_90 variable.
        /// </summary>
        private static double dz_42_90 = -80.9;

        /// <summary>
        /// Represents the value of dx_90_84.
        /// </summary>
        private static double dx_90_84 = -1.08;

        /// <summary>
        /// Represents the value of the variable dy_90_84.
        /// </summary>
        private static double dy_90_84 = -0.27;

        /// <summary>
        /// This variable represents the value of dz_90_84.
        /// </summary>
        private static double dz_90_84 = -0.9;

        // Угловые элементы трансформирования, в секундах
        /// <summary>
        /// Represents the value of wx.
        /// </summary>
        private static double wx = 0;

        /// <summary>
        /// Represents the value of variable wy.
        /// </summary>
        private static double wy = 0;

        /// <summary>
        /// Represents the value of wz, which is a private static double variable.
        /// </summary>
        private static double wz = 0;
        // Дифференциальное различие масштабов
        /// <summary>
        /// Stores the value of milliseconds.
        /// </summary>
        private static double ms = 0;

        /// <summary>
        /// Converts a point from the PZ-90 coordinate system to the WGS84 coordinate system.
        /// </summary>
        /// <param name="point">The point to convert.</param>
        /// <returns>A new GeoPoint object representing the converted point.</returns>
        public static GeoPoint PZ90_WGS84(this GeoPoint point)
        {
            var lat = PZ90_WGS84_Lat(point.Latitude, point.Longitude, point.Altitude);
            var lon = PZ90_WGS84_Long(point.Latitude, point.Longitude, point.Altitude);
            var alt = WGS84Alt(point.Latitude, point.Longitude, point.Altitude, dx_90_84, dy_90_84, dz_90_84);
            return new GeoPoint(lat,lon,alt);
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
            var alt = WGS84Alt(point.Latitude, point.Longitude, point.Altitude, dx_90_84, dy_90_84, dz_90_84);
            return new GeoPoint(lat, lon, alt);
        }


        /// <summary>
        /// Calculates the dB value based on the given inputs. </summary> <param name="Bd">The value of Bd</param> <param name="Ld">The value of Ld</param> <param name="H">The value of H</param> <param name="dx">The value of dx</param> <param name="dy">The value of dy</param> <param name="dz">The value of dz</param> <returns>
        /// The calculated dB value. </returns>
        /// /
        private static double dB(double Bd, double Ld, double H, double dx, double dy, double dz)
        {
            double B, L, M, N;
            B = Bd * Pi / 180;
            L = Ld * Pi / 180;
            M = a * (1 - e2) / Math.Pow(1 - e2 * Math.Pow(Math.Sin(B),2), 1.5);
            N = a * Math.Pow(1 - e2 * Math.Pow(Math.Sin(B),2), -0.5);

            return ro / (M + H) * (N / a * e2 * Math.Sin(B) * Math.Cos(B) * da +
                                   (Math.Pow(N,2) / Math.Pow(a,2) + 1) * N * Math.Sin(B) * Math.Cos(B) * de2 / 2
                                   - (dx * Math.Cos(L) + dy * Math.Sin(L)) * Math.Sin(B) + dz * Math.Cos(B))
                   - wx * Math.Sin(L) * (1 + e2 * Math.Cos(2 * B)) + wy * Math.Cos(L) * (1 + e2 * Math.Cos(2 * B))
                   - ro * ms * e2 * Math.Sin(B) * Math.Cos(B);
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
            double B, L, N;
            B = Bd * Pi / 180;
            L = Ld * Pi / 180;
            N = a * Math.Pow(1 - e2 * Math.Pow(Math.Sin(B),2), -0.5);
            return ro / ((N + H) * Math.Cos(B)) * (-dx * Math.Sin(L) + dy * Math.Cos(L))
                   + Math.Tan(B) * (1 - e2) * (wx * Math.Cos(L) + wy * Math.Sin(L)) - wz;
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
        private static double WGS84Alt(double Bd, double Ld, double H, double dx, double dy, double dz)
        {
            double B, L, N, dH;
            B = Bd * Pi / 180;
            L = Ld * Pi / 180;
            N = a * Math.Pow(1 - e2 * Math.Pow(Math.Sin(B),2), -0.5);
            dH = -a / N * da + N * Math.Pow(Math.Sin(B),2) * de2 / 2
                             + (dx * Math.Cos(L) + dy * Math.Sin(L)) * Math.Cos(B) + dz * Math.Sin(B)
                 - N * e2 * Math.Sin(B) * Math.Cos(B) * (wx / ro * Math.Sin(L) - wy / ro * Math.Cos(L))
                 + (Math.Pow(a,2) / N + H) * ms;
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
            return Bd + dB(Bd, Ld, H, dx_90_84, dy_90_84, dz_90_84) / 3600;
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
            return Ld + dL(Bd, Ld, H, dx_90_84, dy_90_84, dz_90_84) / 3600;
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
            return Bd - dB(Bd, Ld, H, dx_90_84, dy_90_84, dz_90_84) / 3600;
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
            return Ld - dL(Bd, Ld, H, dx_90_84, dy_90_84, dz_90_84) / 3600;
        }

        //
        /// <summary>
        /// Converts geographic latitude in the CK42 coordinate system to PZ-90 coordinate system.
        /// </summary>
        /// <param name="Bd">Geographic latitude in the CK42 coordinate system, in degrees.</param>
        /// <param name="Ld">Geographic longitude in the CK42 coordinate system, in degrees.</param>
        /// <param name="H">Geographic altitude in meters.</param>
        /// <returns>
        /// Geographic latitude in the PZ-90 coordinate system, in degrees.
        /// </returns>
        private static double CK42_PZ90_Lat(double Bd, double Ld, double H)
        {
            return Bd + dB(Bd, Ld, H, dx_42_90, dy_42_90, dz_42_90) / 3600;
        }

        /// <summary>
        /// Calculates the longitude in the PZ-90 coordinate system using the CK-42 geodetic data.
        /// </summary>
        /// <param name="Bd">The latitude in decimal degrees.</param>
        /// <param name="Ld">The longitude in decimal degrees.</param>
        /// <param name="H">The height in meters.</param>
        /// <returns>
        /// The calculated longitude in the PZ-90 coordinate system.
        /// </returns>
        private static double CK42_PZ90_Long(double Bd, double Ld, double H)
        {
            return Ld + dL(Bd, Ld, H, dx_42_90, dy_42_90, dz_42_90) / 3600;
        }

        /// <summary>
        /// Calculate the latitude in the CK-42 coordinate system based on the PZ-90 geodetic system.
        /// </summary>
        /// <param name="Bd">The geodetic latitude in degrees.</param>
        /// <param name="Ld">The geodetic longitude in degrees.</param>
        /// <param name="H">The geodetic height in meters.</param>
        /// <returns>
        /// The latitude in the CK-42 coordinate system calculated based on the PZ-90 geodetic system.
        /// </returns>
        private static double PZ90_CK42_Lat(double Bd, double Ld, double H)
        {
            return Bd - dB(Bd, Ld, H, dx_42_90, dy_42_90, dz_42_90) / 3600;
        }

        private static double PZ90_CK42_Long(double Bd, double Ld, double H)
        {
            return Ld - dL(Bd, Ld, H, dx_42_90, dy_42_90, dz_42_90) / 3600;
        }

        //
        /// <summary>
        /// Converts coordinates from CK42 datum to WGS84 datum latitude.
        /// </summary>
        /// <param name="Bd">CK42 latitude in decimal degrees.</param>
        /// <param name="Ld">CK42 longitude in decimal degrees.</param>
        /// <param name="H">Height in meters.</param>
        /// <returns>WGS84 latitude in decimal degrees.</returns>
        private static double CK42_WGS84_Lat(double Bd, double Ld, double H)
        {
            return Bd + dB(Bd, Ld, H, dx_42_84, dy_42_84, dz_42_84) / 3600;
        }

        /// <summary>
        /// Calculates the WGS84 longitude based on CK42 coordinates. </summary> <param name="Bd">The latitude in CK42 coordinates.</param> <param name="Ld">The longitude in CK42 coordinates.</param> <param name="H">The height in CK42 coordinates.</param> <returns>
        /// The WGS84 longitude corresponding to the given CK42 coordinates.
        /// This value is calculated by adding the result of the dL function (based on the given CK42 coordinates and the dx_42_84, dy_42_84, dz_42_84 parameters) to the Ld parameter and dividing
        /// the result by 3600. </returns>
        /// /
        private static double CK42_WGS84_Long(double Bd, double Ld, double H)
        {
            return Ld + dL(Bd, Ld, H, dx_42_84, dy_42_84, dz_42_84) / 3600;
        }

        /// <summary>
        /// Calculates the WGS84 CK42 latitude based on the provided parameters.
        /// </summary>
        /// <param name="Bd">The latitude in degrees.</param>
        /// <param name="Ld">The longitude in degrees.</param>
        /// <param name="H">The height above the sea level in meters.</param>
        /// <returns>The WGS84 CK42 latitude calculated based on the provided parameters.</returns>
        private static double WGS84_CK42_Lat(double Bd, double Ld, double H)
        {
            return Bd - dB(Bd, Ld, H, dx_42_84, dy_42_84, dz_42_84) / 3600;
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
            return Ld - dL(Bd, Ld, H, dx_42_84, dy_42_84, dz_42_84) / 3600;
        }
    }
}
