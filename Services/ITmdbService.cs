using TmdbAppApi.Dtos;

namespace TmdbAppApi.Services;

public interface ITmdbService
{
    Task<MovieDto?> GetMovieByTitle(string title);
}