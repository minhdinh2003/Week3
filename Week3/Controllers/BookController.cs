using AutoMapper;
using MassTransit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Week3.Dto;
using Week3.Models;
using Week3.Repository.Interface;

namespace Week3.Controllers;

[ApiController]
[Route("api/[controller]")]

public class BookController(IUnitOfWorkMsSql unitOfWork, IRedisService cacheService, IMapper mapper, IPublishEndpoint publishEndpoint) : ControllerBase
{
    private readonly string _bookListCacheKey = "book_list";
    private readonly IPublishEndpoint _publishEndpoint = publishEndpoint;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var cachedBooks = await cacheService.GetAsync<IEnumerable<BookDto>>(_bookListCacheKey);
        if (cachedBooks != null)
            return Ok(cachedBooks);

        var books = await unitOfWork.Books.GetAllAsync();
        var bookDtos = mapper.Map<IEnumerable<BookDto>>(books);

        await cacheService.SetAsync(_bookListCacheKey, bookDtos, TimeSpan.FromMinutes(1));
        return Ok(bookDtos);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var cacheKey = $"book_{id}";
        var cachedBook = await cacheService.GetAsync<Book>(cacheKey);
        if (cachedBook != null)
            return Ok(cachedBook);

        var book = await unitOfWork.Books.GetByIdAsync(id);
        if (book == null) return NotFound();

        var bookDto = mapper.Map<Book>(book);
        await cacheService.SetAsync(cacheKey, bookDto, TimeSpan.FromMinutes(1));

        return Ok(bookDto);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Create(BookDto bookDto)
    {
        try
        {
            var book = mapper.Map<Book>(bookDto);
            await unitOfWork.Books.AddAsync(book);
            await unitOfWork.SaveChangesAsync();
            var bookEvent = new CreateBookRequest
            {
                Title = book.Title,
                Author = book.AuthorId
            };
            await _publishEndpoint.Publish(bookEvent);
            // Clear cache
            await cacheService.RemoveAsync(_bookListCacheKey);
            return CreatedAtAction(nameof(GetById), new { id = book.Id }, mapper.Map<BookDto>(book));
        }
        catch (Exception)
        {
            return StatusCode(500, "Internal server error");
        }

    }

    [HttpPut("{id}")]
    [Authorize]
    public async Task<IActionResult> Update(int id, BookDto bookDto)
    {
        try
        {
            if (id != bookDto.Id) return BadRequest();

            var book = mapper.Map<Book>(bookDto);
            unitOfWork.Books.Update(book);
            await unitOfWork.SaveChangesAsync();

            // Clear related cache
            await cacheService.RemoveAsync(_bookListCacheKey);
            await cacheService.RemoveAsync($"book_{id}");
            return NoContent();
        }
        catch (Exception)
        {
            // Log the exception (not shown here)
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpDelete("{id}")]
    [Authorize]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var book = await unitOfWork.Books.GetByIdAsync(id);
            if (book == null) return NotFound();

            await unitOfWork.Books.DeleteBookAndReviewsAsync(id);
            await unitOfWork.SaveChangesAsync();

            // Clear related cache
            await cacheService.RemoveAsync(_bookListCacheKey);
            await cacheService.RemoveAsync($"book_{id}");

            return NoContent();
        }
        catch (Exception)
        {
            // Log the exception (not shown here)
            return StatusCode(500, "Internal server error");
        }
    }
}
public class CreateBookRequest
{
    public string Title { get; set; }
    public int Author { get; set; }
}