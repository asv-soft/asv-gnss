namespace Asv.Gnss
{
    public enum ReasonForException
    {
        /// <summary>
        /// No exception reason.
        /// </summary>
        NoExceptionReason,

        /// <summary>
        /// Not enough ephemeris available.
        /// </summary>
        NotEphemeris,

        /// <summary>
        /// Excluded by RAIM algorithm.
        /// </summary>
        Raim,

        /// <summary>
        /// Satellite health.
        /// </summary>
        Health,

        /// <summary>
        /// SQM.
        /// </summary>
        Sqm,
    }
}
