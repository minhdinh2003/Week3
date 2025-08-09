using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using Week3.Models;
using Week3.MongoModels;
using Week3.Repository.Interface;
using Week3.Data;

namespace Week3.Repository;

public class ReviewerRepository(AppDbContext context, IMongoCollection<Review> mongoReviewCollection)
    : GenericRepository<Reviewer>(context), IReviewerRepository
{
    private readonly AppDbContext _context = context;
    private readonly IMongoCollection<Review> _mongoReviewCollection = mongoReviewCollection;

    public async Task<Reviewer?> GetReviewsAsync(int id)
    {
        var reviewer = await _context.Reviewers.FindAsync(id);
        if (reviewer == null)
            return null;

        var reviews = await _mongoReviewCollection
            .Find(r => r.ReviewerId == id)
            .ToListAsync();

        reviewer.Reviews = reviews;
        return reviewer;
    }
}
