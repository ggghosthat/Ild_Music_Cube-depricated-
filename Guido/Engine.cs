using ShareInstances.Instances;
using Cube.Mapper.Entities;

using System.Linq;
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


                connection.Execute("create table if not exists tags(Id integer primary key, TagID text, Name text, Color text)");
                connection.Execute("create table if not exists tags_instances(Id integer primary key, TagID text, IID text)");
            }
        }
        catch(Exception ex)
        {
            throw ex;
        }
    }

    public void Add<T>(T entity)
    {
        ReadOnlySpan<char> dapperQuery;
        if (entity is ArtistMap artist)
        {
            dapperQuery = "insert or ignore into artists(AID, Name, Description, Avatar) values (@Buid, @Name, @Description, @Avatar)".AsSpan();
            using (var connection = new SQLiteConnection(_connectionString.ToString()))
            {                
                connection.Execute(dapperQuery.ToString(), artist);     
            }
        }
        else if (entity is PlaylistMap playlist)
        {
            dapperQuery = "insert or ignore into playlists(PID, Name, Description, Avatar) values (@Buid, @Name, @Description, @Avatar)".AsSpan();
            using (var connection = new SQLiteConnection(_connectionString.ToString()))
            {
                connection.Execute(dapperQuery.ToString(), playlist );
            }
        }
        else if (entity is TrackMap track)
        {
            dapperQuery = "insert or ignore into tracks(TID, Path, Name, Description, Avatar, Valid, Duration) values (@Buid, @Pathway, @Name, @Description, @Avatar, @IsValid, @Duration)".AsSpan();
            using (var connection = new SQLiteConnection(_connectionString.ToString()))
            {
                connection.Execute(dapperQuery.ToString(), track);
            }
        }
        else if (entity is TagMap tag)
        {
            dapperQuery = "insert or ignore into tags(TagID, Name, Color) values (@Buid, @Name, @Color)".AsSpan();
            using (var connection = new SQLiteConnection(_connectionString.ToString()))
            {
                connection.Execute(dapperQuery.ToString(), tag);
            }
        }
    }

    public void AddStores(ICollection<Store> stores)
    {
        stores.ToList().ForEach(store => 
        {   
            ReadOnlySpan<char> dapperQuery;
            switch(store.Tag)
            {
                case(1):
                    dapperQuery = "insert into artists_playlists(AID, PID) select @aid, @pid where not EXISTS(SELECT 1 from artists_playlists where AID = @aid and PID = @pid)".AsSpan();
                    using (var connection = new SQLiteConnection(_connectionString.ToString()))
                    {
                        foreach(Guid relate in store.Relates)
                        {
                            connection.Execute(dapperQuery.ToString(), new {aid=store.Holder.ToString(), pid=relate.ToString() });
                        }
                    }
                    break;
                case(2):
                    dapperQuery = "insert into artists_tracks(AID, TID) select @aid, @tid where not EXISTS(SELECT 1 from artists_tracks where AID = @aid and TID = @tid)".AsSpan();
                    using (var connection = new SQLiteConnection(_connectionString.ToString()))
                    {
                        foreach(Guid relate in store.Relates)
                        {
                            connection.Execute(dapperQuery.ToString(), new {aid=store.Holder.ToString(), tid=relate.ToString() });
                        }
                    }
                    break;
                case(3):
                    dapperQuery = "insert into artists_playlists(AID, PID) select @aid, @pid where not EXISTS(SELECT 1 from artists_playlists where AID = @aid and PID = @pid)".AsSpan();
                    using (var connection = new SQLiteConnection(_connectionString.ToString()))
                    {
                        foreach(Guid relate in store.Relates)
                        {
                            connection.Execute(dapperQuery.ToString(), new {aid=relate.ToString(), pid=store.Holder.ToString() });
                        }
                    }
                    break;
                case(4):
                    dapperQuery = "insert into playlists_tracks(PID, TID) select @pid, @tid where not EXISTS(SELECT 1 from playlists_tracks where PID = @pid and TID = @tid)".AsSpan();
                    using (var connection = new SQLiteConnection(_connectionString.ToString()))
                    {
                        foreach(Guid relate in store.Relates)
                        {
                            connection.Execute(dapperQuery.ToString(), new {pid=store.Holder.ToString(), tid=relate.ToString() });
                        }
                    }
                    break;
                case(5):
                    dapperQuery = "insert into artists_tracks(AID, TID) select @aid, @tid where not EXISTS(SELECT 1 from artists_tracks where AID = @aid and TID = @tid)";
                    using (var connection = new SQLiteConnection(_connectionString.ToString()))
                    {
                        foreach(Guid relate in store.Relates)
                        {
                            connection.Execute(dapperQuery.ToString(), new {aid=relate.ToString(), tid=store.Holder.ToString() });
                        }
                    }
                    break;
                case(6):
                    dapperQuery = "insert into playlists_tracks(PID, TID) select @pid, @tid where not EXISTS(SELECT 1 from playlists_tracks where PID = @pid and TID = @tid)";
                    using (var connection = new SQLiteConnection(_connectionString.ToString()))
                    {
                        foreach(Guid relate in store.Relates)
                        {
                            connection.Execute(dapperQuery.ToString(), new {pid=relate.ToString(), tid=store.Holder.ToString() });
                        }
                    }
                    break;
                case(7):
                    dapperQuery = "insert into tags_instances(TagID, IID) select @tagid, @iid where not EXISTS(SELECT 1 from tags_instances where TagID = @tagid and IID = @iid)";
                    using (var connection = new SQLiteConnection(_connectionString.ToString()))
                    {
                        foreach(Guid relate in store.Relates)
                        {
                            connection.Execute(dapperQuery.ToString(), new {tagid=relate.ToString(), iid=store.Holder.ToString() });
                        }
                    }
                    break;
                default:break;
            }
        });
    }

    public void Edit<T>(T entity)
    {
        ReadOnlySpan<char> dapperQuery;
        if (entity is Artist artist)
        {
            dapperQuery = "".AsSpan();
            using (var connection = new SQLiteConnection(_connectionString.ToString()))
            {
                foreach(Guid relate in store.Relates)
                {
                    connection.Execute(dapperQuery.ToString(), new {});
                }
            }
        }
        else if (entity is Playlist playlist)
        {
            dapperQuery = "".AsSpan();
            using (var connection = new SQLiteConnection(_connectionString.ToString()))
            {
                foreach(Guid relate in store.Relates)
                {
                    connection.Execute(dapperQuery.ToString(), new {});
                }
            }

        }
        else if (entity is Track track)
        {
            dapperQuery = "".AsSpan();
            using (var connection = new SQLiteConnection(_connectionString.ToString()))
            {
                foreach(Guid relate in store.Relates)
                {
                    connection.Execute(dapperQuery.ToString(), new {});
                }
            }

        }
    }

    public void EditStores(ICollection<Store> stores)
    {
        
    }

    public void Delete<T>(ref T entity)
    {
        ReadOnlySpan<char> dapperQuery;
        if(entity is Artist artist)
        {
            dapperQuery = @"delete from artists where AID = @aid;
                            delete from artists_playlists where AID = @aid;
                            delete from artists_tracks where AID = @aid;".AsSpan();

            using (var connection = new SQLiteConnection(_connectionString.ToString()))
            {
                connection.QueryMultiple(dapperQuery.ToString(), new {aid=artist.Id.ToString()});
            }
        }
        else if(entity is Playlist playlist)
        {
            dapperQuery = @"delete from playlists where PID = @pid;
                            delete from artists_playlists where PID = @pid;
                            delete from playlists_tracks where PID = @pid;".AsSpan();

            using (var connection = new SQLiteConnection(_connectionString.ToString()))
            {
                connection.QueryMultiple(dapperQuery.ToString(), new {pid=playlist.Id.ToString()});
            }
        }
        else if(entity is Track track)
        {
            dapperQuery = @"delete from tracks where TID = @tid;
                            delete from playlists_tracks where TID = @tid;
                            delete from artists_tracks where TID = @tid;".AsSpan();

            using (var connection = new SQLiteConnection(_connectionString.ToString()))
            {
                connection.QueryMultiple(dapperQuery.ToString(), new {tid=track.Id.ToString()});
            }

        }
    }
}
