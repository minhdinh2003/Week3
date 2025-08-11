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
    public class UnitOfWorkMsSql : IUnitOfWorkMsSql
    {
        private readonly AppDbContext _context;
        private IDbContextTransaction? _transaction;

        public IAuthorRepository Authors { get; }
        public IBookRepository Books { get; }
        public IReviewerRepository Reviewers { get; }

        IBookRepository IUnitOfWorkMsSql.Books => Books;
        IReviewerRepository IUnitOfWorkMsSql.Reviewers => Reviewers;
        IAuthorRepository IUnitOfWorkMsSql.Authors => Authors;

        public IQueryable<Book> BooksQueryable => throw new NotImplementedException();
        public IQueryable<Reviewer> ReviewersQueryable => throw new NotImplementedException();
        public IQueryable<Author> AuthorsQueryable => throw new NotImplementedException();

        public UnitOfWorkMsSql(
            AppDbContext context)
        {
            _context = context;
            Authors = new AuthorRepository(_context);
            Books = new BookRepository(_context);
            Reviewers = new ReviewerRepository(_context);
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