using TmdbAppApi.Dtos;
using TmdbAppApi.Services;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("[controller]")]
public class MoviesController : ControllerBase
{
    private readonly ITmdbService _tmdbService;

    public MoviesController(ITmdbService tmdbService) => _tmdbService = tmdbService;

    [HttpGet("by-title")]
    public async Task<ActionResult<MovieDto>> GetByTitle([FromQuery] string title)
    {
        var movie = await _tmdbService.GetMovieByTitle(title);
        return movie is null ? NotFound() : Ok(movie);
    }
}