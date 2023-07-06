using ShareInstances.Instances;

using System;
using System.Threading.Tasks;
using System.Data.SQLite;
using Dapper;
using System.Collections.Concurrent;
using System.IO;

namespace Cube.Storage;

public class GuidoForklift //Cars from pixar (lol)
{
    private string path;
    private ReadOnlyMemory<char> connectionString;
    private int capacity;

    private ConcurrentQueue<ReadOnlyMemory<char>> queries = new();

    public GuidoForklift(string dbPath,
                        int capacity)
    {
        this.path = dbPath;
        this.connectionString = $"Data Source = {this.path}".AsMemory();
        this.capacity = capacity;
    }

    //check database and table existance
    //in negative case it creates from scratch
    public void ForkliftUp()
    {
        if(!File.Exists(path))
        {
            SQLiteConnection.CreateFile(path);
        }
        using (var connection = new SQLiteConnection(connectionString.ToString()))
        {
            connection.Execute("create table if not exists artists(Id integer primary key, Name varchar(50))");
        }
    }

    #region CRUD
    //insert new entity
    public async Task AddEntity<T>(T entity)
    {
        if (entity is Artist artist)
        {
        }
        else if(entity is Playlist playlist)
        {            
        }
        else if(entity is Track track)
        {
        }
    }

    //update(edit) existed entity
    public void EditEntity<T>(T entity)
    {}

    //delete specific entity by it own id
    public void DeleteEntity<T>(Guid entityId)
    {}

    //upload whole stuff from db
    public void Load()
    {}
    #endregion
    
}
