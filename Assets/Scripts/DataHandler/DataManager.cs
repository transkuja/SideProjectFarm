using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class DataManager : Singleton<DataManager> {

    [System.Serializable]
    public class DataSave
    {
        public string id;
        public object data;

        public DataSave() { }

        public DataSave(string _id, object _data)
        {
            id = _id;
            data = _data;
        }
    }

    [SerializeField]
    Dictionary<string, object> datas = new Dictionary<string, object>();

    public delegate void OnSaveDelegate();
    public delegate void OnLoadDelegate();

    public event OnLoadDelegate OnLoadCallback;
    public event OnSaveDelegate OnSaveCallback;

    List<Type> extraTypes = new List<Type>();

    public bool GetData<T>(string _id, out T _data)
    {
        if(datas.ContainsKey(_id))
        {
            _data = (T)datas[_id];
            return true;
        }
        else
        {
            _data = default(T);
            return false;
        }
    }

    public void SetData<T>(string _id, T _data)
    {
        datas[_id] = _data;
    }

    public void AddExtraType(Type _type)
    {
        if (!extraTypes.Contains(_type))
        {
            extraTypes.Add(_type);
        }
    }

    public void Load()
    {
        List<DataSave> listDatas = XmlUtils.Deserialize<List<DataSave>>("Assets/Scripts/Manager/Resources/test_save.xml", extraTypes.ToArray());
        SetDataList(listDatas);

        if (OnLoadCallback != null)
        {
            OnLoadCallback();
        }
    }

    public void Save()
    {
        if (OnSaveCallback != null)
        {
            OnSaveCallback();
        }

        XmlUtils.Serialize(GetDataList(), "Assets/Scripts/Manager/Resources/test_save.xml", extraTypes.ToArray());
    }

    List<DataSave> GetDataList()
    {
        List<DataSave> listDatas = new List<DataSave>();
        foreach(var data in datas)
        {
            listDatas.Add(new DataSave(data.Key, data.Value));
        }
        return listDatas;
    }

    void SetDataList(List<DataSave> _listDatas)
    {
        datas = new Dictionary<string, object>();
        foreach (var data in _listDatas)
        {
            datas[data.id] = data.data;
        }
    }
}
