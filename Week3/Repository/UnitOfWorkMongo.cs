using MongoDB.Driver;
using Week3.MongoModels;
using Week3.Repository.Interface;

namespace Week3.Repository
{
    public class UnitOfWorkMongo(IMongoDatabase database) : IUnitOfWorkMongo 
    {
        public IMongoCollection<Review> Reviews { get; } = database.GetCollection<Review>("Reviews");
    }
}
