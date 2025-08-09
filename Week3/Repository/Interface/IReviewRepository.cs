using System;
using Week3.Models;
using Week3.MongoModels;

namespace Week3.Repository.Interface;

public interface IReviewRepository
{
    Task<List<Review>> GetByBookIdAsync(int bookId);
    Task<List<Review>> GetByReviewerIdAsync(int reviewerId);
    Task AddAsync(Review review);
}
