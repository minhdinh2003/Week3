using System;
using System.Linq;
using Microsoft.EntityFrameworkCore.Storage;
using Week3.Models;
using Week3.Repository.Interface;
using MongoDB.Driver;
using Week3.MongoModels;
using Week3.Data;

namespace Week3.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        private IDbContextTransaction? _transaction;

        public IAuthorRepository Authors { get; }
        public IBookRepository Books { get; }
        public IReviewerRepository Reviewers { get; }

        IBookRepository IUnitOfWork.Books => Books;
        IReviewerRepository IUnitOfWork.Reviewers => Reviewers;
        IAuthorRepository IUnitOfWork.Authors => Authors;

        public IQueryable<Book> BooksQueryable => throw new NotImplementedException();
        public IQueryable<Reviewer> ReviewersQueryable => throw new NotImplementedException();
        public IQueryable<Author> AuthorsQueryable => throw new NotImplementedException();

        public UnitOfWork(
            AppDbContext context,
            IMongoCollection<Review> mongoReviewCollection)
        {
            _context = context;
            Authors = new AuthorRepository(_context);
            Books = new BookRepository(_context, mongoReviewCollection);
            Reviewers = new ReviewerRepository(_context, mongoReviewCollection);
        }

        public async Task<int> SaveChangesAsync()
            => await _context.SaveChangesAsync();

        public async Task BeginTransactionAsync()
            => _transaction = await _context.Database.BeginTransactionAsync();

        public async Task CommitAsync()
            => await _transaction?.CommitAsync();

        public async Task RollbackAsync()
            => await _transaction?.RollbackAsync();

        public void Dispose()
            => _context.Dispose();
    }
}
