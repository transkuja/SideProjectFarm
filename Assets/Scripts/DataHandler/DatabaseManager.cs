using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using System;


[XmlInclude(typeof(HarvestableData))]
[System.Serializable]
public class Data
{
    public string id;
    public Data() { }
    public Data(Data _from)
    {
        id = _from.id;
    }

    public virtual string Serialize()
    {
        return "";
    }

    public virtual void Deserialize(List<KeyValuePair<int, string>> _values)
    {
    }
}

[System.Serializable]
public class Database
{
    public string path;
    public List<Data> loadedData;
    public Type parserClass;
    public char separator;

    public Database(string _path)
    {
        path = _path;
        loadedData = new List<Data>();
        if (path.LastIndexOf('.') != -1)
        {
            string extension = path.Substring(path.LastIndexOf('.') + 1);

            if (extension.Equals("xml"))
                parserClass = typeof(XmlUtils);
            else if (extension.Equals("csv"))
                parserClass = typeof(CSVParser);
        }
    }

    public Database(string _path, Type _parserClass)
    {
        path = _path;
        loadedData = new List<Data>();
        parserClass = _parserClass;
    }

    public Database(string _path, Type _parserClass, char _separator)
    {
        path = _path;
        loadedData = new List<Data>();
        parserClass = _parserClass;
        separator = _separator;
    }

    public Database()
    {
        loadedData = new List<Data>();
    }
}

public static class DatabaseManager
{
    static bool databasesInitialized = false;

    static Dictionary<Type, Database> databases = new Dictionary<Type, Database>();
    static Dictionary<Type, Type> mapTypeEnum = new Dictionary<Type, Type>();

    /// <summary>
    /// Initialize all databases.
    /// </summary>
    public static void Init(bool _forceDatabaseReload = false)
    {
        if (_forceDatabaseReload)
            databasesInitialized = false;

        if (databasesInitialized)
            return;

        databases.Clear();
        databases.Add(typeof(FishData), new Database("Assets/Resources/fishs.xml"));
        databases.Add(typeof(BuildingData), new Database("Assets/Resources/buildings.xml"));
        databases.Add(typeof(TreeData), new Database("Assets/Resources/trees.xml"));

        mapTypeEnum.Clear();

        LoadDatabase<FishData>();
        LoadDatabase<BuildingData>();
        LoadDatabase<TreeData>();

        databasesInitialized = true;
    }

    /// <summary>
    /// Returns a row from database of type T based on its id.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="_id"></param>
    /// <returns></returns>
    public static T GetRowFromId<T>(string _id) where T : Data
    {
        if (GetDatabaseInfo<T>() == null)
        {
            //Debug.LogWarning("Database for " + typeof(T).ToString() + " has not been initialized");
            return null;
        }
        return (T)GetDatabaseInfo<T>().loadedData.Find(x => x.id == _id);
    }

    /// <summary>
    /// Retrieve all rows from database of type T
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public static List<T> GetAllRows<T>() where T : Data
    {
        if (GetDatabaseInfo<T>() == null)
            return null;

        return GetDatabaseInfo<T>().loadedData.Cast<T>().ToList();
    }

    /// <summary>
    /// Overwrite current database with _rows. All previous data will be lost!
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="_rows"></param>
    public static void OverwriteAllRows<T>(List<T> _rows) where T : Data
    {
        Database database = GetDatabaseInfo<T>();

        database.loadedData = _rows.Cast<Data>().ToList();

        Serialize<T>(database);
    }

    /// <summary>
    /// Save multiple rows stored in _rows to database. Modifies existing id or create new entries if the id does not exist.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="_rows"></param>
    public static void SaveMultipleRows<T>(List<T> _rows) where T : Data
    {
        foreach (T row in _rows)
            SaveEntryToDatabase<T>(row);
    }

    /// <summary>
    /// Permanently remove an entry from database of type T based on id. 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="_toRemoveId"></param>
    public static void RemoveEntryFromDatabase<T>(string _toRemoveId) where T : Data
    {
        Database database = GetDatabaseInfo<T>();
        if (database == null)
        {
            //Debug.LogWarning("Database for " + typeof(T).ToString() + " has not been initialized");
            return;
        }
        List<T> entries = GetAllRows<T>();

        entries.Remove(entries.Find(x => x.id == _toRemoveId));
        database.loadedData = entries.Cast<Data>().ToList();

        Serialize<T>(database);
    }

    /// <summary>
    /// Save or modify an entry from database of type T. 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="_newEntry"></param>
    public static void SaveEntryToDatabase<T>(T _newEntry) where T : Data
    {
        Database database = GetDatabaseInfo<T>();
        if (database == null)
        {
            //Debug.LogWarning("Database for " + typeof(T).ToString() + " has not been initialized");
            return;
        }
        List<T> entries = GetAllRows<T>();

        if (entries == null)
            entries = new List<T>();

        bool isANewEntry = entries.Find(x => x.id == _newEntry.id) == null;
        if (!isANewEntry)
            entries[entries.IndexOf(entries.Find(x => x.id == _newEntry.id))] = _newEntry;
        else
            entries.Add(_newEntry);

        database.loadedData = entries.Cast<Data>().ToList();

        Serialize<T>(database);
    }

    static Database GetDatabaseInfo<T>()
    {
        if (databases.ContainsKey((typeof(T))))
            return databases[typeof(T)];

        return null;
    }

    static void LoadDatabase<T>() where T : Data, new()
    {
        Database database = GetDatabaseInfo<T>();
#if UNITY_EDITOR
        CreateFileIfDoesNotExistOrEmpty<T>(database);
#endif
        Deserialize<T>(database);
    }

    static void CreateFileIfDoesNotExistOrEmpty<T>(Database _db) where T : Data
    {
        if (!File.Exists(_db.path))
        {
            using (StreamWriter sw = File.CreateText(_db.path))
            {
                sw.Close();

                Serialize<T>(_db);
            }
        }
        else
        {
            StreamReader reader = new StreamReader(_db.path);
            if (string.IsNullOrEmpty(reader.ReadLine()))
            {
                reader.Close();
                Serialize<T>(_db);
            }
            else
                reader.Close();
        }
    }

    static void Serialize<T>(Database _database) where T : Data
    {
        if (_database.parserClass == typeof(XmlUtils))
            XmlUtils.Serialize(_database.loadedData, _database.path);
        else if (_database.parserClass == typeof(CSVParser))
            CSVParser.Serialize(_database.loadedData, _database.path, mapTypeEnum[typeof(T)]);
    }

    static void Deserialize<T>(Database _database) where T : Data, new()
    {
        if (_database.parserClass == typeof(XmlUtils))
            _database.loadedData = XmlUtils.Deserialize<List<Data>>(_database.path);
        else if (_database.parserClass == typeof(CSVParser))
            _database.loadedData = CSVParser.Deserialize<T>(_database.path).Cast<Data>().ToList();               

    }
}
