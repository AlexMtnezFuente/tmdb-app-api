using System.Text.Json.Serialization;
using TmdbAppApi.Dtos;

namespace TmdbAppApi.Services;

public class TmdbService : ITmdbService
{
    private readonly HttpClient _http;
    private readonly string _apiKey;

    public TmdbService(HttpClient http, IConfiguration config)
    {
        _http = http;
        _apiKey = config["TMDB:ApiKey"]
                  ?? throw new InvalidOperationException("TMDB:ApiKey no configurada.");
    }

    public async Task<MovieDto?> GetMovieByTitle(string title)
    {
        var url = $"search/movie?api_key={_apiKey}&query={Uri.EscapeDataString(title)}&page=1";
        var response = await _http.GetFromJsonAsync<TmdbPagedResponse<TmdbMovie>>(url)
          ?? new TmdbPagedResponse<TmdbMovie>();
        var first = response?.Results?.FirstOrDefault();
        if (first == null)
        {
            return null;
        }

        return MapToDto(first);
    }

    private static MovieDto MapToDto(TmdbMovie tmdbMovie) => new()
    {
        Id = tmdbMovie.Id,
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
