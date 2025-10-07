namespace TmdbAppApi.Dtos
{
    public class MovieDto
    {
        public string Title { get; set; } = "";
        public string OriginalTitle { get; set; } = "";
        public double VoteAverage { get; set; }
        public string ReleaseDate { get; set; } = "";
        public string Overview { get; set; } = "";
        public string SimilarMovies { get; set; } = "";
    }
}
