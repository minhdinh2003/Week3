using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Week3.Dto;
using Week3.Models;
using Week3.Repository.Interface;

namespace Week3.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthorController(IUnitOfWork unitOfWork, IRedisService cacheService, IMapper mapper) : ControllerBase
{
    private readonly string _authorListCacheKey = "author_list";

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            var cachedAuthors = await cacheService.GetAsync<IEnumerable<AuthorDto>>(_authorListCacheKey);
            if (cachedAuthors != null)
                return Ok(cachedAuthors);

            var authors = await unitOfWork.Authors.GetAllAsync();
            var authorDtos = mapper.Map<IEnumerable<AuthorDto>>(authors);

            await cacheService.SetAsync(_authorListCacheKey, authorDtos, TimeSpan.FromMinutes(1));
            return Ok(authorDtos);
        } catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        try{
        var cacheKey = $"author_{id}";
        var cachedAuthor = await cacheService.GetAsync<AuthorDto>(cacheKey);
        if (cachedAuthor != null)
            return Ok(cachedAuthor);

        var author = await unitOfWork.Authors.GetByIdAsync(id);
        if (author == null) return NotFound();

        var authorDto = mapper.Map<AuthorDto>(author);
        await cacheService.SetAsync(cacheKey, authorDto, TimeSpan.FromMinutes(1));

        return Ok(authorDto);
        } catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpGet("with-books/{id}")]
    public async Task<IActionResult> GetAuthorWithBooks(int id)
    {
        try{
        var cacheKey = $"author_with_books_{id}";
        var cached = await cacheService.GetAsync<AuthorDto>(cacheKey);
        if (cached != null)
            return Ok(cached);

        var author = await unitOfWork.Authors.GetAuthorWithBooksAsync(id);
        if (author == null) return NotFound();

        var authorDto = mapper.Map<AuthorDto>(author);
        await cacheService.SetAsync(cacheKey, authorDto, TimeSpan.FromMinutes(1));

        return Ok(authorDto);
        } catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpPost]
    public async Task<IActionResult> Create(AuthorDto authorDto)
    {
        try
        {
            var author = mapper.Map<Author>(authorDto);
            await unitOfWork.Authors.AddAsync(author);
            await unitOfWork.SaveChangesAsync();

            await cacheService.RemoveAsync(_authorListCacheKey);

            return CreatedAtAction(nameof(GetById), new { id = author.Id }, mapper.Map<AuthorDto>(author));
        } catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, AuthorDto authorDto)
    {
        try{
        if (id != authorDto.Id) return BadRequest();

        var author = mapper.Map<Author>(authorDto);
        unitOfWork.Authors.Update(author);
        await unitOfWork.SaveChangesAsync();

        await cacheService.RemoveAsync(_authorListCacheKey);
        await cacheService.RemoveAsync($"author_{id}");
        await cacheService.RemoveAsync($"author_with_books_{id}");

        return NoContent();
        }  catch (Exception ex)
        {
            return StatusCode(500, $"Internal server error: {ex.Message}");
        }
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var author = await unitOfWork.Authors.GetByIdAsync(id);
        if (author == null) return NotFound();

        unitOfWork.Authors.Delete(author);
        await unitOfWork.SaveChangesAsync();

        await cacheService.RemoveAsync(_authorListCacheKey);
        await cacheService.RemoveAsync($"author_{id}");
        await cacheService.RemoveAsync($"author_with_books_{id}");

        return NoContent();
    }
}
