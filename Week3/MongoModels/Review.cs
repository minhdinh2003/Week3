using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Week3.Dto;

namespace Week3.MongoModels
{
    public class Review
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; } = null!;

        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int Rate { get; set; }
        public int BookId { get; set; }
        public int ReviewerId { get; set; }

        public BookDto Book { get; set; } = null!;
        public ReviewerDto Reviewer { get; set; } = null!;
    }
}
