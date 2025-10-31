using TmdbAppApi.Dtos;
using TmdbAppApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace TmdbAppApi.Controllers;

[ApiController]
[Route("[controller]")]
public class MoviesController(ITmdbService tmdbService) : ControllerBase
{
    private readonly ITmdbService _tmdbService = tmdbService;

    [HttpGet("by-title")]
    public async Task<ActionResult<MovieDto>> GetByTitle([FromQuery] string title)
    {
        var movie = await _tmdbService.GetMovieByTitle(title);
        return movie is null ? NotFound("Movie not found") : Ok(movie);
    }
}