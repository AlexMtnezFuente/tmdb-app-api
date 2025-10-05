# TMDB App API

A minimal REST API (ASP.NET Core) that:
- Retrieves movie information by title (first result from TMDB search)
- Returns up to 5 similar movies formatted as `Title (YYYY)`

## Requirements
- .NET 8 SDK
- Docker (optional, for containerized run)
- **A `.env` file with your TMDB API key** (see below)

## Environment Variables (.env)
Create a `.env` file in the project root:

```env
TMDB__ApiKey=REPLACE_WITH_YOUR_TMDB_API_KEY