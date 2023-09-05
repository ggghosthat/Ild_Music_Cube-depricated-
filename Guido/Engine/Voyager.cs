using ShareInstances.Instances;
using Cube.Mapper.Entities;

using System.Threading.Tasks;
using System.Data.SQLite;
using Dapper;
namespace Cube.Storage.Guido.Engine.Searcher;
internal class Voyager
{
    private string _connectionString;
    public Voyager(ref string connectionString)
    {
        _connectionString = connectionString;
    }

                                           
    public async Task<IEnumerable<ArtistMap>> SearchArtists(Memory<char> searchTerm)
    {
       IEnumerable<ArtistMap> artists = null;

       ReadOnlyMemory<char> dapperQuery = @"select AID, Name, Description, Avatar, Year from artists where Name like '%@term%';".AsMemory();

       using (var connection = new SQLiteConnection(_connectionString.ToString()))
       {
            await connection.OpenAsync();
            artists = await connection.QueryAsync<ArtistMap>(dapperQuery.ToString(), new {term=searchTerm.ToString()} );
       }

       return artists;
    }

    public async Task<IEnumerable<PlaylistMap>> SearchPlaylists(Memory<char> searchTerm)
    {
       IEnumerable<PlaylistMap> playlists = null;

       ReadOnlyMemory<char> dapperQuery = @"select PID, Name, Description, Avatar, Year from playlists where Name like '%@term%';".AsMemory();

       using (var connection = new SQLiteConnection(_connectionString.ToString()))
       {
            await connection.OpenAsync();
            playlists = await connection.QueryAsync<PlaylistMap>(dapperQuery.ToString(), new {term=searchTerm.ToString()} );
       }

       return playlists;
    }

    public async Task<IEnumerable<TrackMap>> SearchTracks(Memory<char> searchTerm)
    {
       IEnumerable<TrackMap> tracks = null;

       ReadOnlyMemory<char> dapperQuery = @"select TID, Path, Name, Description, Avatar, Valid, Duration, Year from tracks where Name like '%@term%';".AsMemory();

       using (var connection = new SQLiteConnection(_connectionString.ToString()))
       {
            await connection.OpenAsync();
            tracks = await connection.QueryAsync<TrackMap>(dapperQuery.ToString(), new {term=searchTerm.ToString()} );
       }

       return tracks;
    }
}
