using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Week3.Dto;
using Week3.MongoModels;
using Week3.Repository.Interface;
using Week3.Data;

namespace Week3.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReviewController(
    AppDbContext context,
    IReviewRepository reviewRepository,
    IRedisService cacheService,
    IMapper mapper
) : ControllerBase
{
    private readonly AppDbContext _context = context;
    private readonly IReviewRepository _reviewRepository = reviewRepository;

    [HttpPost]
    public async Task<IActionResult> CreateReview([FromBody] ReviewDto reviewDto)
    {
        var book = await _context.Books.FindAsync(reviewDto.BookId);
        var reviewer = await _context.Reviewers.FindAsync(reviewDto.ReviewerId);

        if (book == null || reviewer == null)
            return NotFound("Book hoặc Reviewer không tồn tại");

        using var transaction = await _context.Database.BeginTransactionAsync();

        try
        {
            var review = mapper.Map<Review>(reviewDto);
            review.Book = mapper.Map<BookDto>(book);
            review.Reviewer = mapper.Map<ReviewerDto>(reviewer);

            var reviews = await _reviewRepository.GetByBookIdAsync(book.Id);
            book.Rate = (float)((reviews.Sum(r => r.Rate) + review.Rate) / (reviews.Count + 1));

            _context.Books.Update(book);
            await _context.SaveChangesAsync();

            await _reviewRepository.AddAsync(review);
            await transaction.CommitAsync();

            // Clear cache
            await cacheService.RemoveAsync($"reviews_book_{review.BookId}");
            await cacheService.RemoveAsync($"reviews_reviewer_{review.ReviewerId}");

            return CreatedAtAction(nameof(GetByBookId), new { bookId = review.BookId }, mapper.Map<ReviewDto>(review));
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return StatusCode(500, $"Đã xảy ra lỗi: {ex.Message}");
        }
    }

    [HttpGet("book/{bookId}")]
    public async Task<IActionResult> GetByBookId(int bookId)
    {
        var cacheKey = $"reviews_book_{bookId}";
        var cached = await cacheService.GetAsync<IEnumerable<ReviewDto>>(cacheKey);
        if (cached != null)
            return Ok(cached);

        var reviews = await _reviewRepository.GetByBookIdAsync(bookId);
        var reviewDtos = mapper.Map<IEnumerable<ReviewDto>>(reviews);

        await cacheService.SetAsync(cacheKey, reviewDtos, TimeSpan.FromMinutes(1));
        return Ok(reviewDtos);
    }

    [HttpGet("reviewer/{reviewerId}")]
    public async Task<IActionResult> GetByReviewerId(int reviewerId)
    {
        var cacheKey = $"reviews_reviewer_{reviewerId}";
        var cached = await cacheService.GetAsync<IEnumerable<ReviewDto>>(cacheKey);
        if (cached != null)
            return Ok(cached);

        var reviews = await _reviewRepository.GetByReviewerIdAsync(reviewerId);
        var reviewDtos = mapper.Map<IEnumerable<ReviewDto>>(reviews);

        await cacheService.SetAsync(cacheKey, reviewDtos, TimeSpan.FromMinutes(10));
        return Ok(reviewDtos);
    }
}
