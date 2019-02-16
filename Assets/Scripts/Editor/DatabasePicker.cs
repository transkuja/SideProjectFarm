using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
using UnityEditorInternal;
using System.Linq;
using System;

public interface IDatabasePicker
{
    void SetGUIData(EditorWindow data);
    void OnEnable();
    void OnGUI();
}

public class DatabasePickerBool
{
    public string propertyName;
    public bool isActive;
    public bool isTrue;

    public DatabasePickerBool(string _propertyName, bool _isActive, bool _isTrue)
    {
        propertyName = _propertyName;
        isActive = _isActive;
        isTrue = _isTrue;
    }
}

public class DatabasePickerInt
{
    public string propertyName;
    public bool isActive;
    public int value;
    public int operatorId;

    public DatabasePickerInt(string _propertyName, bool _isActive, int _value, int _operatorId)
    {
        propertyName = _propertyName;
        isActive = _isActive;
        value = _value;
        operatorId = _operatorId;
    }
}

public class DatabasePickerFloat
{
    public string propertyName;
    public bool isActive;
    public float value;
    public int operatorId;

    public DatabasePickerFloat(string _propertyName, bool _isActive, float _value, int _operatorId)
    {
        propertyName = _propertyName;
        isActive = _isActive;
        value = _value;
        operatorId = _operatorId;
    }
}

public class DatabasePickerEnum
{
    public string propertyName;
    public bool isActive;
    public Type enumType;
    public Enum selected;

    public DatabasePickerEnum(string _propertyName, bool _isActive)
    {
        propertyName = _propertyName;
        isActive = _isActive;
    }
}

public class DatabasePicker<T> : IDatabasePicker where T : Data
{
    public delegate void Del(T message);
    Del callbackPicker;

    public delegate void DelMultiple(T[] message);
    DelMultiple callbackPickerMultiple;


    bool isMultiple = false;

    public Vector2 scrollPos = new Vector2(0, 0);

    public ReorderableList listItem;

    public List<T> items;

    public List<T> selectedItems;

    EditorWindow data;

    string searchText = "";

    public List<DatabasePickerBool> filtersBool;
    public List<DatabasePickerInt> filtersInt;
    public List<DatabasePickerFloat> filtersFloat;
    public List<DatabasePickerEnum> filtersEnum;


    string[] visualOperators = {
        "<",
        "<=",
        "==",
        ">=",
        ">",
        "!="
    };

    public DatabasePicker(Del _CallbackPicker)
    {
        callbackPicker = _CallbackPicker;
    }

    public DatabasePicker(DelMultiple _CallbackPicker)
    {
        callbackPickerMultiple = _CallbackPicker;
        isMultiple = true;
    }
    

    DatabasePickerFilter<T> filterPicker;

    int indexLastSelected = 0;
    double timeLastSelected = 0.0f;

    public void OnEnable()
    {
        filtersBool = new List<DatabasePickerBool>();
        filtersInt = new List<DatabasePickerInt>();
        filtersFloat = new List<DatabasePickerFloat>();
        filtersEnum = new List<DatabasePickerEnum>();

        selectedItems = new List<T>();

   //     var types = AppDomain.CurrentDomain.GetAllDerivedTypes(typeof(DatabasePickerFilter<T>));
   // foreach (var type in types)
   //     if (types != null && types.Length > 0)
        {
            //     filterPicker = ((DatabasePickerFilter<T>)Activator.CreateInstance(types[0]));
            filterPicker = new DatabasePickerFilter<T>();

            foreach (string propertyName in filterPicker.GetProperties())
            {
                Type type = typeof(bool);

                if (typeof(T).GetProperty(propertyName) != null)
                    type = typeof(T).GetProperty(propertyName).PropertyType;
                else if (typeof(T).GetField(propertyName) != null)
                    type = typeof(T).GetField(propertyName).FieldType;

                if(type == typeof(bool))
                {
                    filtersBool.Add(new DatabasePickerBool(propertyName, false, true));
                }
                else if (type == typeof(int))
                {
                    filtersInt.Add(new DatabasePickerInt(propertyName, false, 0, 0));
                }
                else if (type == typeof(float))
                {
                    filtersFloat.Add(new DatabasePickerFloat(propertyName, false, 0, 0));
                }
                else if (type.IsEnum)
                {
                    DatabasePickerEnum pickerEnum = new DatabasePickerEnum(propertyName, false);
                    pickerEnum.enumType = type;
                    pickerEnum.selected = Activator.CreateInstance(pickerEnum.enumType) as Enum;
                    filtersEnum.Add(pickerEnum);
                }
            }
        }

        searchText = "";

        items = DatabaseManager.GetAllRows<T>().ToList();

        listItem = new ReorderableList(items, typeof(T), false, false, false, false);

        if (listItem.list.Count > 0)
        {
            listItem.index = 0;
        }

        listItem.drawElementCallback =
            (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                T item = listItem.list[index] as T;
                if (selectedItems.Contains(item))
                {
                    EditorGUI.LabelField(rect, " * " + items[index].id);
                }
                else
                {
                    EditorGUI.LabelField(rect, items[index].id);
                }
            };

        listItem.onSelectCallback = (ReorderableList l) => {

            if (!Event.current.control && indexLastSelected == l.index && EditorApplication.timeSinceStartup - timeLastSelected < 1.0f)
            {
                if (listItem.index >= 0)
                {
                    if (callbackPicker != null)
                        callbackPicker((T)listItem.list[listItem.index]);
                    if (callbackPickerMultiple != null)
                        callbackPickerMultiple(selectedItems.ToArray());
                    data.Close();
                }
            }

            if (isMultiple && listItem.index >= 0)
            {
                Event e = Event.current;
                if ( ! e.control)
                {
                    selectedItems.Clear();
                }
                T item = listItem.list[listItem.index] as T;
                if (selectedItems.Contains(item))
                {
                    selectedItems.Remove(item);
                }
                else
                {
                    selectedItems.Add(item);
                }
            }

            indexLastSelected = l.index;
            timeLastSelected = EditorApplication.timeSinceStartup;
        };
    }

    public void OnGUI()
    {
        Rect area = new Rect(0, 0, data.position.width, data.position.height - 50);
        GUILayout.BeginArea(area);
        if (listItem != null)
        {

            GUILayout.BeginHorizontal();

            string lastSearch = searchText;
            searchText = GUILayout.TextField(searchText, GUILayout.Width(300));
            if (searchText != lastSearch)
            {
                UpdateFilter();
            }

            if (GUILayout.Button("<"))
            {
                items = items.OrderBy(x => x.id).ToList();
                listItem.list = items;
            }

            if (GUILayout.Button(">"))
            {
                items = items.OrderBy(x => x.id).ToList();
                items.Reverse();
                listItem.list = items;
            }
            
            GUILayout.EndHorizontal();
            
            GUILayout.BeginHorizontal();

            GUILayout.BeginVertical(GUILayout.Width(data.position.width / 2));

            scrollPos = GUILayout.BeginScrollView(scrollPos);

            listItem.DoLayoutList();

            GUILayout.EndScrollView();

            GUILayout.EndVertical();

            GUILayout.BeginVertical();
            
            foreach (var filter in filtersBool)
            {
                GUILayout.BeginHorizontal();
                bool toogle = GUILayout.Toggle(filter.isActive, filter.propertyName, "button", GUILayout.Width(160));
                if (toogle != filter.isActive)
                {
                    filter.isActive = toogle;
                    UpdateFilter();
                }
                toogle = GUILayout.Toggle(filter.isTrue, "", GUILayout.Width(30));
                if (toogle != filter.isTrue)
                {
                    filter.isTrue = toogle;
                    UpdateFilter();
                }
                GUILayout.EndHorizontal();
            }

            foreach (var filter in filtersInt)
            {
                GUILayout.BeginHorizontal();
                bool toogle = GUILayout.Toggle(filter.isActive, filter.propertyName, "button", GUILayout.Width(160));
                if (toogle != filter.isActive)
                {
                    filter.isActive = toogle;
                    UpdateFilter();
                }

                int operatorIndex = EditorGUILayout.Popup(filter.operatorId, visualOperators, GUILayout.Width(40));
                if (filter.operatorId != operatorIndex)
                {
                    filter.operatorId = operatorIndex;
                    UpdateFilter();
                }

                int value = EditorGUILayout.IntField(filter.value, GUILayout.Width(60));
                if(filter.value != value)
                {
                    filter.value = value;
                    UpdateFilter();
                }

                GUILayout.EndHorizontal();
            }

            foreach (var filter in filtersFloat)
            {
                GUILayout.BeginHorizontal();
                bool toogle = GUILayout.Toggle(filter.isActive, filter.propertyName, "button", GUILayout.Width(160));
                if (toogle != filter.isActive)
                {
                    filter.isActive = toogle;
                    UpdateFilter();
                }

                int operatorIndex = EditorGUILayout.Popup(filter.operatorId, visualOperators, GUILayout.Width(40));
                if (filter.operatorId != operatorIndex)
                {
                    filter.operatorId = operatorIndex;
                    UpdateFilter();
                }

                float value = EditorGUILayout.FloatField(filter.value, GUILayout.Width(60));
                if (filter.value != value)
                {
                    filter.value = value;
                    UpdateFilter();
                }

                GUILayout.EndHorizontal();
            }

            foreach (var filter in filtersEnum)
            {
                GUILayout.BeginHorizontal();
                bool toogle = GUILayout.Toggle(filter.isActive, filter.propertyName, "button", GUILayout.Width(160));
                if (toogle != filter.isActive)
                {
                    filter.isActive = toogle;
                    UpdateFilter();
                }
                Enum enumSelected = EditorGUILayout.EnumPopup(filter.selected, GUILayout.Width(80));
                if(filter.selected != enumSelected)
                {
                    filter.selected = enumSelected;
                    UpdateFilter();
                }

                GUILayout.EndHorizontal();
            }

            GUILayout.EndVertical();

            GUILayout.EndHorizontal();


            if (GUILayout.Button("Pick"))
            {
                if (listItem.index >= 0)
                {
                    if (callbackPicker != null)
                        callbackPicker((T)listItem.list[listItem.index]);
                    if (callbackPickerMultiple != null)
                        callbackPickerMultiple(selectedItems.ToArray());
                    data.Close();
                }
            }
        }
        GUILayout.EndArea();


        if (GUI.changed) data.Repaint();
    }

    void UpdateFilter()
    {
        items = DatabaseManager.GetAllRows<T>().ToList();
        items = items.Where(x => x.id.ToLower().Contains(searchText.ToLower())).ToList();

        foreach (var filter in filtersBool)
        {
            if (filter.isActive)
            {
                items = filterPicker.FilterBool(items, filter.propertyName, filter.isTrue);
            }
        }
        foreach (var filter in filtersInt)
        {
            if (filter.isActive)
            {
                items = filterPicker.FilterInt(items, filter.propertyName, filter.value, (DatabasePickerFilter<T>.FilterOperator)filter.operatorId);
            }
        }
        foreach (var filter in filtersFloat)
        {
            if (filter.isActive)
            {
                items = filterPicker.FilterFloat(items, filter.propertyName, filter.value, (DatabasePickerFilter<T>.FilterOperator)filter.operatorId);
            }
        }

        foreach (var filter in filtersEnum)
        {
            if (filter.isActive)
            {
                items = filterPicker.FilterEnum(items, filter.propertyName, filter.selected);
            }
        }

        items = items.OrderBy(x => x.id).ToList();

        listItem.list = items;
        listItem.index = 0;
    }

    public void SetGUIData(EditorWindow _data)
    {
        data = _data;
    }
}

public class DatabasePickerWrapper : EditorWindow
{
    static System.Type _type = null;
    static object myImplementation = null;


    public static void OpenWindow<T>(DatabasePicker<T>.Del _CallBackPicker) where T : Data
    {
        _type = typeof(DatabasePicker<>).MakeGenericType(typeof(T));
        myImplementation = System.Activator.CreateInstance(_type, _CallBackPicker);

        DatabasePickerWrapper window = CreateInstance<DatabasePickerWrapper>();
        window.titleContent = new GUIContent(typeof(T).ToString());
        window.Show();
    }

    public static void OpenWindow<T>(DatabasePicker<T>.DelMultiple _CallBackPicker) where T : Data
    {
        _type = typeof(DatabasePicker<>).MakeGenericType(typeof(T));
        myImplementation = System.Activator.CreateInstance(_type, _CallBackPicker);

        DatabasePickerWrapper window = CreateInstance<DatabasePickerWrapper>();
        window.titleContent = new GUIContent(typeof(T).ToString());
        window.Show();
    }
    /*
    public System.Type Type
    {
        get { return _type.GetGenericArguments()[0]; }
        set
        {
            // These two lines are how you get your specific implementation
            _type = typeof(DatabasePicker<>).MakeGenericType(value);
            myImplementation = System.Activator.CreateInstance(_type);
        }
    }
    */
    void OnGUI()
    {/*
        if (null == myImplementation)
        { // provide selection options or prevent this condition
            if (GUILayout.Button("GameObject")) Type = typeof(GameObject); // notice capital T, we're using the setter
            return;
        } */
        (myImplementation as IDatabasePicker).SetGUIData(this);
        (myImplementation as IDatabasePicker).OnGUI();
    }
    

    private void OnEnable()
    {
        DatabaseManager.Init();

        (myImplementation as IDatabasePicker).SetGUIData(this);
        (myImplementation as IDatabasePicker).OnEnable();

    }
    
}
