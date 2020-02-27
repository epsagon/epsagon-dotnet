using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace MongoDBApplication
{
    public class User
    {
        [BsonId]
        public BsonObjectId _id { get; set; }

        public string Email { get; set; }

        public string PasswordHash { get; set; }
    }
}
