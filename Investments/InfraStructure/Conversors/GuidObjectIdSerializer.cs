//using MongoDB.Bson;
//using MongoDB.Bson.Serialization;
//using MongoDB.Bson.Serialization.Serializers;
//using System;

//namespace Infrastructure.Conversors
//{

//    public class GuidObjectIdSerializer : SerializerBase<Guid>
//    {
//        public override Guid Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
//        {
//            var bsonReader = context.Reader;

//            // Verifica se o valor é um ObjectId
//            if (bsonReader.CurrentBsonType == BsonType.ObjectId)
//            {
//                ObjectId objectId = bsonReader.ReadObjectId();
//                return new Guid(objectId.ToString());
//            }

//            // Se não for um ObjectId, tenta ler como string (no caso de Guid serializado como string)
//            return Guid.Parse(bsonReader.ReadString());
//        }

//        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, Guid value)
//        {
//            context.Writer.WriteObjectId(new ObjectId(value.ToString()));
//        }
//    }
//}
