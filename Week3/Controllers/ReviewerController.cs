using Microsoft.AspNetCore.Mvc;
using Week3.Models;
using Week3.Repository.Interface;
using StackExchange.Redis;
using System.Text.Json;
using AutoMapper;
using Week3.Dto;

namespace Week3.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReviewerController(IUnitOfWork unitOfWork, IConnectionMultiplexer redis, IMapper mapper) : ControllerBase
{
    private readonly IDatabase _cache = redis.GetDatabase();
    private const string CacheKey = "reviewers";

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var cachedData = await _cache.StringGetAsync(CacheKey);
        if (cachedData.HasValue)
        {
            var reviewers = JsonSerializer.Deserialize<IEnumerable<Reviewer>>(cachedData!);
            return Ok(reviewers);
        }

        var reviewersFromDb = await unitOfWork.Reviewers.GetAllAsync();
        var mapped = mapper.Map<IEnumerable<Reviewer>>(reviewersFromDb);

        var serialized = JsonSerializer.Serialize(mapped);
        await _cache.StringSetAsync(CacheKey, serialized, TimeSpan.FromMinutes(5));
        return Ok(mapped);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        string cacheKey = $"reviewer:{id}";

        var cachedData = await _cache.StringGetAsync(cacheKey);
        if (cachedData.HasValue)
        {
            var reviewer = JsonSerializer.Deserialize<Reviewer>(cachedData!);
            return Ok(reviewer);
        }

        var reviewerFromDb = await unitOfWork.Reviewers.GetByIdAsync(id);
        if (reviewerFromDb == null) return NotFound();

        var mapped = mapper.Map<Reviewer>(reviewerFromDb);

        var serialized = JsonSerializer.Serialize(mapped);
        await _cache.StringSetAsync(cacheKey, serialized, TimeSpan.FromMinutes(5));

        return Ok(mapped);
    }

    [HttpPost]
    public async Task<IActionResult> Create(ReviewerDto reviewerDto)
    {
        var mappedEntity = mapper.Map<Reviewer>(reviewerDto);
        await unitOfWork.Reviewers.AddAsync(mappedEntity);
        await unitOfWork.SaveChangesAsync();
        await _cache.KeyDeleteAsync(CacheKey);
        return CreatedAtAction(nameof(GetById), new { id = mappedEntity.Id }, mappedEntity);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, ReviewerDto reviewerDto)
    {
        if (id != reviewerDto.Id) return BadRequest();

        var entity = await unitOfWork.Reviewers.GetByIdAsync(id);
        if (entity == null) return NotFound();

        mapper.Map(reviewerDto, entity);
        unitOfWork.Reviewers.Update(entity);
        await unitOfWork.SaveChangesAsync();
        await _cache.KeyDeleteAsync(CacheKey);
        await _cache.KeyDeleteAsync($"reviewer:{id}");

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var reviewer = await unitOfWork.Reviewers.GetByIdAsync(id);
        if (reviewer == null) return NotFound();

        unitOfWork.Reviewers.Delete(reviewer);
        await unitOfWork.SaveChangesAsync();
        await _cache.KeyDeleteAsync(CacheKey);
        await _cache.KeyDeleteAsync($"reviewer:{id}");

        return NoContent();
    }
}
