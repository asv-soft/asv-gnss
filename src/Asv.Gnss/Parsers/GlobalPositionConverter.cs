using System;
using Geodesy;
using Newtonsoft.Json;

namespace Asv.Gnss
{
    /// <summary>
    /// The GlobalPositionConverter class is a custom JSON converter for serializing and deserializing GlobalPosition objects. </summary>
    /// /
    public class GlobalPositionConverter : JsonConverter<GlobalPosition>
    {
        /// <summary>
        /// Represents the default instance of the <see cref="GlobalPositionConverter"/> class.
        /// This instance can be used for converting global positions.
        /// </summary>
        public static readonly GlobalPositionConverter Default = new();

        /// <summary>
        /// Writes the JSON representation of a GlobalPosition object to a JsonWriter.
        /// </summary>
        /// <param name="writer">The JsonWriter to write the JSON.</param>
        /// <param name="value">The GlobalPosition object to serialize.</param>
        /// <param name="serializer">The JsonSerializer object used for serialization.</param>
        public override void WriteJson(
            JsonWriter writer,
            GlobalPosition value,
            JsonSerializer serializer
        )
        {
            writer.WriteStartObject();
            writer.WritePropertyName("Lat");
            writer.WriteValue(value.Latitude.Degrees);
            writer.WritePropertyName("Lon");
            writer.WriteValue(value.Longitude.Degrees);
            writer.WritePropertyName("Alt");
            writer.WriteValue(value.Elevation);
            writer.WriteEndObject();
        }

        /// <summary>
        /// Reads JSON data and converts it to a <see cref="GlobalPosition"/> object.
        /// </summary>
        /// <param name="reader">The <see cref="JsonReader"/> object to read JSON data from.</param>
        /// <param name="objectType">The type of the object being deserialized.</param>
        /// <param name="existingValue">The existing value of the object being deserialized.</param>
        /// <param name="hasExistingValue">A flag indicating whether an existing value is present.</param>
        /// <param name="serializer">The <see cref="JsonSerializer"/> object being used for deserialization.</param>
        /// <returns>A new <see cref="GlobalPosition"/> object populated with the deserialized data.</returns>
        public override GlobalPosition ReadJson(
            JsonReader reader,
            Type objectType,
            GlobalPosition existingValue,
            bool hasExistingValue,
            JsonSerializer serializer
        )
        {
            if (reader.TokenType != JsonToken.StartObject)
            {
                ThrowError();
            }

            if (!reader.Read())
            {
                ThrowError();
            }

            if (reader.TokenType != JsonToken.PropertyName)
            {
                ThrowError();
            }

            if (reader.Value?.Equals("Lat") != true)
            {
                ThrowError();
            }

            var lat = reader.ReadAsDouble();
            if (lat == null)
            {
                ThrowError();
            }

            if (!reader.Read())
            {
                ThrowError();
            }

            if (reader.TokenType != JsonToken.PropertyName)
            {
                ThrowError();
            }

            if (reader.Value?.Equals("Lon") != true)
            {
                ThrowError();
            }

            var lon = reader.ReadAsDouble();
            if (lon == null)
            {
                ThrowError();
            }

            if (!reader.Read())
            {
                ThrowError();
            }

            if (reader.TokenType != JsonToken.PropertyName)
            {
                ThrowError();
            }

            if (reader.Value?.Equals("Alt") != true)
            {
                ThrowError();
            }

            var alt = reader.ReadAsDouble();
            if (alt == null)
            {
                ThrowError();
            }

            if (!reader.Read())
            {
                ThrowError();
            }

            if (reader.TokenType != JsonToken.EndObject)
            {
                ThrowError();
            }

            return new GlobalPosition(new GlobalCoordinates(lat.Value, lon.Value), alt.Value);
        }

        /// <summary>
        /// Throws an exception with a specific error message.
        /// </summary>
        private void ThrowError()
        {
            throw new Exception($"Error to deserialize {nameof(GlobalPositionConverter)}");
        }
    }

    /// <summary>
    /// Converts a <see cref="GlobalPosition"/> object to its nullable equivalent when serializing and deserializing JSON.
    /// </summary>
    public class GlobalPositionNullableConverter : JsonConverter<GlobalPosition?>
    {
        /// <summary>
        /// Represents the default instance of the <see cref="GlobalPositionNullableConverter"/> class.
        /// </summary>
        public static readonly GlobalPositionNullableConverter Default = new();

        /// <summary>
        /// Writes the specified <paramref name="value"/> as a JSON string representation to the specified <paramref name="writer"/> using the specified <paramref name="serializer"/>.
        /// </summary>
        /// <param name="writer">The <see cref="JsonWriter"/> to write the JSON string representation to.</param>
        /// <param name="value">The <see cref="GlobalPosition"/> value to write.</param>
        /// <param name="serializer">The <see cref="JsonSerializer"/> to use for serialization.</param>
        public override void WriteJson(
            JsonWriter writer,
            GlobalPosition? value,
            JsonSerializer serializer
        )
        {
            if (value.HasValue)
            {
                GlobalPositionConverter.Default.WriteJson(writer, value, serializer);
            }
            else
            {
                writer.WriteNull();
            }
        }

        /// <summary>
        /// Reads a JSON representation of a GlobalPosition object and converts it to a GlobalPosition? object. </summary> <param name="reader">The JsonReader object to read the JSON from.</param> <param name="objectType">The Type of the object being deserialized.</param> <param name="existingValue">The existing GlobalPosition? object to populate, or null.</param> <param name="hasExistingValue">A flag indicating whether an existing value is present.</param> <param name="serializer">The JsonSerializer object being used.</param> <returns>
        /// The deserialized GlobalPosition? object. If the reader.TokenType is JsonToken.Null, it returns null. Otherwise,
        /// it returns the deserialized GlobalPosition object converted to a GlobalPosition? object. </returns>
        /// /
        public override GlobalPosition? ReadJson(
            JsonReader reader,
            Type objectType,
            GlobalPosition? existingValue,
            bool hasExistingValue,
            JsonSerializer serializer
        )
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return null;
            }

            return GlobalPositionConverter.Default.ReadJson(
                reader,
                objectType,
                existingValue ?? default(GlobalPosition),
                hasExistingValue,
                serializer
            );
        }
    }
}
