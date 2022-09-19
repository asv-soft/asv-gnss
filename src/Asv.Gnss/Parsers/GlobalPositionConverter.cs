using System;
using Geodesy;
using Newtonsoft.Json;

namespace Asv.Gnss
{
    public class GlobalPositionConverter : JsonConverter<GlobalPosition>
    {
        public static readonly GlobalPositionConverter Default = new();

        public override void WriteJson(JsonWriter writer, GlobalPosition value, JsonSerializer serializer)
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

        public override GlobalPosition ReadJson(JsonReader reader, Type objectType, GlobalPosition existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType != JsonToken.StartObject) ThrowError();
            if (reader.Read() == false) ThrowError();
            if (reader.TokenType != JsonToken.PropertyName) ThrowError();
            if (reader.Value == null || reader.Value.Equals("Lat") == false) ThrowError();
            var lat = reader.ReadAsDouble();
            if (lat == null) ThrowError();

            if (reader.Read() == false) ThrowError();
            if (reader.TokenType != JsonToken.PropertyName) ThrowError();
            if (reader.Value == null || reader.Value.Equals("Lon") == false) ThrowError();
            var lon = reader.ReadAsDouble();
            if (lon == null) ThrowError();

            if (reader.Read() == false) ThrowError();
            if (reader.TokenType != JsonToken.PropertyName) ThrowError();
            if (reader.Value == null || reader.Value.Equals("Alt") == false) ThrowError();
            var alt = reader.ReadAsDouble();
            if (alt == null) ThrowError();

            if (reader.Read() == false) ThrowError();
            if (reader.TokenType != JsonToken.EndObject) ThrowError();

            return new GlobalPosition(new GlobalCoordinates(lat.Value, lon.Value), alt.Value);
        }

        private void ThrowError()
        {
            throw new Exception($"Error to deserialize {nameof(GlobalPositionConverter)}");
        }
    }

    public class GlobalPositionNullableConverter : JsonConverter<GlobalPosition?>
    {
        public static readonly GlobalPositionNullableConverter Default = new();

        public override void WriteJson(JsonWriter writer, GlobalPosition? value, JsonSerializer serializer)
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

        public override GlobalPosition? ReadJson(JsonReader reader, Type objectType, GlobalPosition? existingValue, bool hasExistingValue,
            JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            return GlobalPositionConverter.Default.ReadJson(reader, objectType, existingValue ?? new GlobalPosition(), hasExistingValue, serializer);
        }

        private void ThrowError()
        {
            throw new Exception($"Error to deserialize {nameof(GlobalPositionConverter)}");
        }
    }
}