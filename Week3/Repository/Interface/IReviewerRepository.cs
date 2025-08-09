using Week3.Models;

namespace Week3.Repository.Interface;

public interface IReviewerRepository : IGenericRepository<Reviewer>
{
    Task<Reviewer?> GetReviewsAsync(int id);
}
