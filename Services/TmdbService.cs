using Microsoft.Extensions.Caching.Memory;
using System.Text.Json.Serialization;
using TmdbAppApi.Dtos;

namespace TmdbAppApi.Services;

public class TmdbService : ITmdbService
{
    private readonly HttpClient _http;
    private readonly string _apiKey;
    private readonly IMemoryCache _cache;

    public TmdbService(HttpClient http, IConfiguration config, IMemoryCache cache)
    {
        _http = http;
        _cache = cache;
        _apiKey = config["TMDB:ApiKey"]
                  ?? throw new InvalidOperationException("TMDB:ApiKey no configurada.");
    }

    public async Task<MovieDto?> GetMovieByTitle(string title)
    {
        var cacheKey = $"movie:{title.ToLower()}";

        var cachedMovie = _cache.Get<MovieDto>(cacheKey);
        if (cachedMovie is not null)
            return cachedMovie;

        string url = $"search/movie?api_key={_apiKey}&query={Uri.EscapeDataString(title)}&page=1";
        var response = await _http.GetFromJsonAsync<TmdbPagedResponse<TmdbMovie>>(url)
          ?? new TmdbPagedResponse<TmdbMovie>();
        var first = response?.Results?.FirstOrDefault();
        if (first == null)
        {
            return null;
        }

        var dto = MapToDto(first);
        dto.SimilarMovies = await GetSimilarMovies(first.Id);

        _cache.Set(cacheKey, dto, TimeSpan.FromSeconds(10));

        return dto;
    }

    private async Task<List<string>> GetSimilarMovies(int movieId, int maxResults = 5)
    {
        var url = $"movie/{movieId}/similar?api_key={_apiKey}&page=1";
        var result = await _http.GetFromJsonAsync<TmdbPagedResponse<TmdbMovie>>(url);

        if (result?.Results == null)
        {
            return new();
        }

        return result.Results
            .Take(maxResults)
            .Select(movie =>
            {
                var year = !string.IsNullOrWhiteSpace(movie.ReleaseDate) && movie.ReleaseDate.Length >= 4
                    ? movie.ReleaseDate[..4]
                    : "N/A";
                return $"{movie.Title} ({year})";
            })
            .ToList();
    }

    private static MovieDto MapToDto(TmdbMovie tmdbMovie) => new()
    {
        Title = tmdbMovie.Title ?? "",
        OriginalTitle = tmdbMovie.OriginalTitle ?? "",
        VoteAverage = tmdbMovie.VoteAverage,
        ReleaseDate = tmdbMovie.ReleaseDate,
        Overview = tmdbMovie.Overview ?? ""
    };

    private sealed class TmdbPagedResponse<T>
    {
        public int Page { get; set; }
        [JsonPropertyName("results")] public List<T> Results { get; set; } = new();
    }

    private sealed class TmdbMovie
    {
        [JsonPropertyName("id")] public int Id { get; set; }
        [JsonPropertyName("title")] public string? Title { get; set; }
        [JsonPropertyName("original_title")] public string? OriginalTitle { get; set; }
        [JsonPropertyName("vote_average")] public double VoteAverage { get; set; }
        [JsonPropertyName("release_date")] public string? ReleaseDate { get; set; }
        [JsonPropertyName("overview")] public string? Overview { get; set; }
    }
}
