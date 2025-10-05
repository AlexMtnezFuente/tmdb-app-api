using TmdbAppApi.Services;
using DotNetEnv;

namespace TmdbAppApi;

public class Program
{
    public static void Main(string[] args)
    {
        Env.Load();
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddMemoryCache();

        builder.Services.AddHttpClient<ITmdbService, TmdbService>(c =>
        {
            c.BaseAddress = new Uri("https://api.themoviedb.org/3/");
        });

        var app = builder.Build();

        // app.UseHttpsRedirection();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
