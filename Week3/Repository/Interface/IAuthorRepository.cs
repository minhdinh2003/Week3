using Week3.Models;

namespace Week3.Repository.Interface;

public interface IAuthorRepository : IGenericRepository<Author>
{
    Task<Author?> GetAuthorWithBooksAsync(int id);
}
