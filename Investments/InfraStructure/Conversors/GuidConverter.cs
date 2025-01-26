
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Infrastructure.Conversors
{

    public class GuidConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Guid);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.String)
            {
                var value = (string)reader.Value;
                return Guid.TryParse(value, out var result) ? result : Guid.Empty;
            }
            return Guid.Empty;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(((Guid)value).ToString());
        }
    }

}
