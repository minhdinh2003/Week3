using Microsoft.EntityFrameworkCore;
using Week3.Models;
using Week3.Repository.Interface;
using Week3.Data;

namespace Week3.Repository;

public class AuthorRepository(AppDbContext context) : GenericRepository<Author>(context), IAuthorRepository
{
    private readonly AppDbContext _context = context;

    public async Task<Author?> GetAuthorWithBooksAsync(int id)
    {
        return await _context.Authors
            .Include(a => a.Books)
            .FirstOrDefaultAsync(a => a.Id == id);
    }
}
