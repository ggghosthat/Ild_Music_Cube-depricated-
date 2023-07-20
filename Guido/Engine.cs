using ShareInstances.Instances;
using Cube.Mapper.Entities;
using System.Data.SQLite;
using Dapper;
namespace Cube.Storage;
public class Engine
{
    private string path;
    private ReadOnlyMemory<char> _connectionString;

    public Engine(string path)
    {
        this.path = path;
        this._connectionString = $"Data Source = {this.path}".AsMemory();
    }

    public void StartEngine()
    {
        try
        {
            if(!File.Exists(path))
            {
                SQLiteConnection.CreateFile(path);
            }
            using (var connection = new SQLiteConnection(_connectionString.ToString()))
            {
                connection.Execute("create table if not exists artists(Id integer primary key, AID text, Name varchar, Description varchar, Avatar blob)");
                connection.Execute("create table if not exists playlists(Id integer primary key, PID text, Name varchar, Description varchar, Avatar blob)");
                connection.Execute("create table if not exists tracks(Id integer primary key, TID text, Path varchar, Name varchar, Description varchar, Avatar blob, Valid integer, Duration integer)");
                connection.Execute("create table if not exists artists_playlists(Id integer primary key, AID text, PID text)");
                connection.Execute("create table if not exists artists_tracks(Id integer primary key, AID text, TID text)");
                connection.Execute("create table if not exists playlists_tracks(Id integer primary key, PID text, TID text)");
            }
        }
        catch(Exception ex)
        {
            throw ex;
        }
    }

    public void Add<T>(T entity)
    {
        if (entity is ArtistMap artist)
        {
            using (var connection = new SQLiteConnection(_connectionString.ToString()))
            {                
                connection.Execute("insert into artists(AID, Name, Description, Avatar) values (@Buid, @Name, @Description, @Avatar)", artist);     
            }
        }
        else if (entity is PlaylistMap playlist)
        {
            using (var connection = new SQLiteConnection(_connectionString.ToString()))
            {
                connection.Execute("insert into playlist(PID, Name, Description, Avatar) values (@Buid, @Name, @Description, @Avatar)", playlist);
            }
        }
        else if (entity is TrackMap track)
        {
            using (var connection = new SQLiteConnection(_connectionString.ToString()))
            {
                connection.Execute("insert into artists(TID, Name, Description, Avatar, Valid, Duration) values (@Buid, @Name, @Description, @Avatar, @IsValid, @Duration)");
            }
        }
    }

    public void Delete<T>(T entity)
    {}

    public void Edit<T>(T entity)
    {}
}
