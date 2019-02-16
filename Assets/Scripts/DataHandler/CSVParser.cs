using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class CSVParser {

    public static char lineSeperator = '\n'; // It defines line seperator character
    public static char fieldSeperator = '|'; // It defines field seperator character

    public static void Serialize(List<Data> _items, string _path, Type _associatedEnum)
    {
        string header = WriteHeader(_path, _associatedEnum);

        StreamWriter writer = new StreamWriter(_path);
        writer.WriteLine(header);
        foreach (Data item in _items)
        {
            writer.WriteLine(item.Serialize());
        }
        writer.Close();
    }

    public static List<T> Deserialize<T>(string path) where T : Data, new()
    {
#if UNITY_EDITOR
        List<T> entries = new List<T>();
        StreamReader reader = new StreamReader(path);
        reader.ReadLine();
        while (!reader.EndOfStream)
        {
            string[] currentLine = reader.ReadLine().Split(fieldSeperator);
            T entry = new T();
            entry.id = currentLine[0];
            List<KeyValuePair<int, string>> data = new List<KeyValuePair<int, string>>();
            for (int i = 1; i < currentLine.Length; i++)
            {
                if (currentLine[i].Contains(","))
                    currentLine[i] = currentLine[i].Replace(",", ".");
                data.Add(new KeyValuePair<int, string>(i - 1, currentLine[i]));
            }
            entry.Deserialize(data);
            entries.Add(entry);
        }

        reader.Close();
        return entries;
#else
        List<T> entries = new List<T>();
        string fileCsv = path.Substring(path.LastIndexOf('/') + 1);
        fileCsv = fileCsv.Substring(0, fileCsv.LastIndexOf('.'));
        Debug.Log(fileCsv);
        TextAsset _csv = Resources.Load<TextAsset>(fileCsv);
        
        StringReader reader = new StringReader(_csv.ToString());
        reader.ReadLine();
        string line = reader.ReadLine();
        while (line != null)
        {
            string[] currentLine = line.Split(fieldSeperator);
            T entry = new T();
            entry.id = currentLine[0];
            List<KeyValuePair<int, string>> data = new List<KeyValuePair<int, string>>();
            for (int i = 1; i < currentLine.Length; i++)
            {
                if (currentLine[i].Contains(","))
                    currentLine[i] = currentLine[i].Replace(",", ".");
                data.Add(new KeyValuePair<int, string>(i - 1, currentLine[i]));
            }
            entry.Deserialize(data);
            entries.Add(entry);

            line = reader.ReadLine();

        }
        reader.Close();
        return entries;
#endif
    }

    static string WriteHeader(string path, Type _associatedEnum)
    {
        string[] enumValues = null;
        if (_associatedEnum != null)
            enumValues = Enum.GetNames(_associatedEnum);

        string header = "id" + fieldSeperator;
        if (enumValues != null)
        {
            for (int i = 0; i < enumValues.Length - 1; i++)
            {
                header += enumValues[i] + ((i == enumValues.Length - 2) ? "" : fieldSeperator.ToString());
            }
        }

        return header;
    }

    // Read Character stats data from CSV file : Format to define
    //public static Dictionary<string, Dictionary<StaticCharacterData.CharacterStat, float>> readCharacterStatsData(TextAsset _textToRead)
    //{
    //    //Dico<id, datas>
    //    Dictionary<string, Dictionary<StaticCharacterData.CharacterStat, float>> characterStatsDatas = new Dictionary<string, Dictionary<StaticCharacterData.CharacterStat, float>>();
    //    string[] records = _textToRead.text.Split(lineSeperator);
    //    string[] keys = records[0].Split(fieldSeperator);

    //    for (int i = 1; i < records.Length; i++)
    //    {
    //        string record = records[i];
    //        string[] fields = record.Split(fieldSeperator);

    //        string characterName = fields[0].Trim(); //1st colum is character name (id) : mandatory !
    //        if (!string.IsNullOrEmpty(characterName))
    //        {
    //            characterStatsDatas.Add(characterName, new Dictionary<StaticCharacterData.CharacterStat, float>());

    //            for (int j = 1; j < fields.Length - 1; j++)
    //            {
    //                //convert , to . (floating numbers)
    //                if (fields[j].Contains(","))
    //                    fields[j] = fields[j].Replace(",", ".");

    //                characterStatsDatas[characterName].Add((StaticCharacterData.CharacterStat)System.Enum.Parse(typeof(StaticCharacterData.CharacterStat), keys[j].Trim()), float.Parse(fields[j].Trim()));
    //            }
    //        }
    //    }

    //    return characterStatsDatas;
    //}
}
