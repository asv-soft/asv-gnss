using System;

namespace Asv.Gnss
{
    /// <summary>
    /// Indicates that serialization is not supported for the marked element.
    /// </summary>
    /// <remarks>
    /// This attribute can be applied to classes, structs, interfaces, enumerations, fields, properties, events, delegates, methods, constructors, and parameters.
    /// </remarks>
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public class SerializationNotSupportedAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the SerializationNotSupportedAttribute class.
        /// </summary>
        public SerializationNotSupportedAttribute()
        {

        }
    }
}