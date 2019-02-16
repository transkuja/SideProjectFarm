using System.IO;
using System.Xml.Serialization;
using UnityEngine;
using System;
using System.Collections.Generic;

public static class XmlUtils {

    public static void Serialize(object _item, string _path, Type[] _extraTypes = null)
    {
        XmlSerializer serializer = new XmlSerializer(_item.GetType(), _extraTypes);
        StreamWriter writer = new StreamWriter(_path);
        serializer.Serialize(writer.BaseStream, _item);
        writer.Close();
    }

    public static T Deserialize<T>(string path, Type[] _extraTypes = null)
    {
#if UNITY_EDITOR
        XmlSerializer serializer = new XmlSerializer(typeof(T), _extraTypes);
        StreamReader reader = new StreamReader(path);
        T deserialized = (T)serializer.Deserialize(reader.BaseStream);
        reader.Close();
        return deserialized;
#else
        string fileXml = path.Substring(path.LastIndexOf('/') + 1);
        fileXml = fileXml.Substring(0, fileXml.LastIndexOf('.'));

        TextAsset _xml = Resources.Load<TextAsset>(fileXml);

        // System.Text.Encoding wind1252 = System.Text.Encoding.GetEncoding(1252);
        //  string textData = wind1252.GetString(_xml.bytes);
        string textData = System.Text.Encoding.UTF8.GetString(_xml.bytes);

        XmlSerializer serializer = new XmlSerializer(typeof(T));
        StringReader reader = new StringReader(textData);
        T deserialized = (T)serializer.Deserialize(reader);
        reader.Close();
        return deserialized;
#endif
    }
}
