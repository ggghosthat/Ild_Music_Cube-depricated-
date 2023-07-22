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


                connection.Execute("create table if not exists tags(Id integer primary key, TagID text, Name varchar)");
                connection.Execute("create table if not exists tags_artists(Id integer primary key, TagID text, AID text)");
                connection.Execute("create table if not exists tags_playlists(Id integer primary key, TagID text, PID text)");
                connection.Execute("create table if not exists tags_tracks(Id integer primary key, TagID text, TID text)");
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
                connection.Execute("insert or ignore into artists(AID, Name, Description, Avatar) values (@Buid, @Name, @Description, @Avatar)", artist);     
            }
        }
        else if (entity is PlaylistMap playlist)
        {
            using (var connection = new SQLiteConnection(_connectionString.ToString()))
            {
                connection.Execute("insert or ignore into playlist(PID, Name, Description, Avatar) values (@Buid, @Name, @Description, @Avatar)", playlist);
            }
        }
        else if (entity is TrackMap track)
        {
            using (var connection = new SQLiteConnection(_connectionString.ToString()))
            {
                connection.Execute("insert or ignore into artists(TID, Name, Description, Avatar, Valid, Duration) values (@Buid, @Name, @Description, @Avatar, @IsValid, @Duration)", track);
            }
        }
    }

    public void AddStores(ICollection<Store> stores)
    {
        foreach(Store store in stores)
        {
            InsertStore(store);
        }
    }

    private void InsertStore(Store store)
    {
        switch(store.Tag)
        {
            case(1):
                using (var connection = new SQLiteConnection(_connectionString.ToString()))
                {
                    foreach(Guid relate in store.Relates)
                    {
                        connection.Execute("insert into artists_playlists(AID, PID) select @aid, @pid where not EXISTS(SELECT 1 from artists_playlists where AID = @aid and PID = @pid)", new {aid=store.Holder.ToString(), pid=relate.ToString() });
                    }
                }
                break;
            case(2):
                using (var connection = new SQLiteConnection(_connectionString.ToString()))
                {
                    foreach(Guid relate in store.Relates)
                    {
                        connection.Execute("insert into artists_tracks(AID, TID) select @aid, @tid where not EXISTS(SELECT 1 from artists_tracks where AID = @aid and TID = @tid)", new {aid=store.Holder.ToString(), tid=relate.ToString() });
                    }
                }
                break;
            case(3):
                using (var connection = new SQLiteConnection(_connectionString.ToString()))
                {
                    foreach(Guid relate in store.Relates)
                    {
                        connection.Execute("insert into artists_playlists(AID, PID) select @aid, @pid where not EXISTS(SELECT 1 from artists_playlists where AID = @aid and PID = @pid)", new {aid=relate.ToString(), pid=store.Holder.ToString() });
                    }
                }

                break;
            case(4):
                using (var connection = new SQLiteConnection(_connectionString.ToString()))
                {
                    foreach(Guid relate in store.Relates)
                    {
                        connection.Execute("insert into playlists_tracks(PID, TID) select @pid, @tid where not EXISTS(SELECT 1 from playlists_tracks where PID = @pid and TID = @tid)", new {pid=store.Holder.ToString(), tid=relate.ToString() });
                    }
                }
                break;
            case(5):
                using (var connection = new SQLiteConnection(_connectionString.ToString()))
                {
                    foreach(Guid relate in store.Relates)
                    {
                        connection.Execute("insert into artists_tracks(AID, TID) select @aid, @tid where not EXISTS(SELECT 1 from artists_tracks where AID = @aid and TID = @tid)", new {aid=relate.ToString(), tid=store.Holder.ToString() });
                    }
                }
                break;
            case(6):
                using (var connection = new SQLiteConnection(_connectionString.ToString()))
                {
                    foreach(Guid relate in store.Relates)
                    {
                        connection.Execute("insert into playlists_tracks(PID, TID) select @pid, @tid where not EXISTS(SELECT 1 from playlists_tracks where PID = @pid and TID = @tid)", new {pid=relate.ToString(), tid=store.Holder.ToString() });
                    }
                }
                break;
            case(7):
                break;
            default:break;
        }
    }



    public void Delete<T>(T entity)
    {}

    public void Edit<T>(T entity)
    {}
}
