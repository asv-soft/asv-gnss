using System;

namespace Asv.Gnss
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = true)]
    public class SerializationNotSupportedAttribute : Attribute
    {
        public SerializationNotSupportedAttribute()
        {

        }
    }
}