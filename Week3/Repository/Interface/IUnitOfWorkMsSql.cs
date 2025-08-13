using System;
using Week3.Models;
using Week3.Services.Interface;

namespace Week3.Repository.Interface;
public interface IUnitOfWorkMsSql : IDisposable
{
    IBookService Books{ get; }
    IReviewerRepository Reviewers { get; }
    IAuthorRepository Authors { get; }
    IQueryable<Book> BooksQueryable { get; }
    IQueryable<Reviewer> ReviewersQueryable { get; }
    IQueryable<Author> AuthorsQueryable { get; }

    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitAsync();
    Task RollbackAsync();
}