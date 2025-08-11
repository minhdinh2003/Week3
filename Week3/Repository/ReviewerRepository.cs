using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using Week3.Models;
using Week3.MongoModels;
using Week3.Repository.Interface;
using Week3.Data;

namespace Week3.Repository;

public class ReviewerRepository(AppDbContext context)
    : GenericRepository<Reviewer>(context), IReviewerRepository{}