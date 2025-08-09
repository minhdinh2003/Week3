using Week3.Models;

namespace Week3.Repository.Interface;

public interface IBookRepository : IGenericRepository<Book>
{
    Task<Book?> GetBookWithAuthorAsync(int id);
    Task<Book?> GetBookWithReviewsAsync(int id);
    Task<bool> DeleteBookAndReviewsAsync(int id);
}
