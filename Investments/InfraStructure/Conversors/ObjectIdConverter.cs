using MongoDB.Bson;
using Newtonsoft.Json;
namespace Infrastructure.Conversors
{

    public class ObjectIdConverter : JsonConverter<ObjectId>
    {
        public override ObjectId ReadJson(JsonReader reader, Type objectType, ObjectId existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.String)
            {
                return new ObjectId(reader.Value.ToString());
            }
            throw new JsonSerializationException("Expected string for ObjectId.");
        }

        public override void WriteJson(JsonWriter writer, ObjectId value, JsonSerializer serializer)
        {
            writer.WriteValue(value.ToString());
        }
    }
}
