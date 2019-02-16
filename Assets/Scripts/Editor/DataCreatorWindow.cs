using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;

public class DataCreatorWindow : EditorWindow
{
    static GUIStyle windowTitleStyle = new GUIStyle();
    static GUIStyle textStyle = new GUIStyle();
    static GUIStyle errorTextStyle = new GUIStyle();

    static HarvestableData dataToSave = new HarvestableData();

    [MenuItem("Tools/Data Creator")]
    static void Init()
    {
        DatabaseManager.Init();
        LoadDatabase();

        DataCreatorWindow window = (DataCreatorWindow)EditorWindow.GetWindow(typeof(DataCreatorWindow));
        window.minSize = new Vector2(400, 400);

        windowTitleStyle.alignment = TextAnchor.UpperCenter;
        windowTitleStyle.fontStyle = FontStyle.Bold;
        windowTitleStyle.fontSize = 15;
        textStyle.alignment = TextAnchor.MiddleCenter;
        textStyle.fontStyle = FontStyle.Bold;

        errorTextStyle.normal.textColor = Color.red;
        dataToSave = new FishData() as HarvestableData;
        window.Show();
    }

    Sprite pickedIcon;
    public int oldType = 0;
    private void OnGUI()
    {
        GUILayout.Label("Data creator", windowTitleStyle);
        currentType = GUILayout.Toolbar(currentType, toolbarNames);
        if (currentType != oldType)
        {
            switch (currentType)
            {
                case 0:
                    dataToSave = new FishData(dataToSave);
                    break;
                case 1:
                    dataToSave = new TreeData(dataToSave);
                    break;
                case 2:
                    dataToSave = new BuildingData(dataToSave);
                    break;
                default:
                    break;
            }
        }
        
        oldType = currentType;

        GUILayout.Space(10.0f);
        dataToSave.id = EditorGUILayout.TextField("Id", dataToSave.id);
        dataToSave.name = EditorGUILayout.TextField("Name", dataToSave.name);
        dataToSave.description = EditorGUILayout.TextField("description", dataToSave.description);

        dataToSave.initialQuantity = EditorGUILayout.IntSlider("Initial quantity on collect", dataToSave.initialQuantity, 0, 2000);
        GUILayout.BeginHorizontal();
        {
            pickedIcon = (Sprite)EditorGUILayout.ObjectField("Icon", pickedIcon, typeof(Sprite), false);
            dataToSave.spriteId = (pickedIcon != null) ? pickedIcon.name : "";
            GUILayout.FlexibleSpace();
        }
        GUILayout.EndHorizontal();
        dataToSave.respawnTime = EditorGUILayout.Slider("Respawn time", dataToSave.respawnTime, 0, 2000);
        dataToSave.requiredLevel = EditorGUILayout.IntSlider("Required level to collect", dataToSave.requiredLevel, 0, GlobalDesigner.jobsMaxLevel);
        dataToSave.initialProcessTime = EditorGUILayout.Slider("Time to collect", dataToSave.initialProcessTime, 0, 2000);

        dataToSave.sellPrice = EditorGUILayout.IntField("Sell price", dataToSave.sellPrice);
        GUILayout.FlexibleSpace();
        DatabaseHandlerEditor("data", ResetData, SaveDataToDatabase, LoadData, RemoveDataFromDatabase, dataToSave, harvestableDataNames);
    }

    static List<HarvestableData> harvestableDatas = new List<HarvestableData>();
    static string[] harvestableDataNames;
    static string[] toolbarNames = new string[] { "Fish", "Trees", "Buildings" };
    static int currentType = 0;

    static void LoadDatabase()
    {
        switch (currentType)
        {
            case 0:
                harvestableDatas = DatabaseManager.GetAllRows<FishData>().Cast<HarvestableData>().ToList();
                break;
            case 1:
                harvestableDatas = DatabaseManager.GetAllRows<TreeData>().Cast<HarvestableData>().ToList();
                break;
            case 2:
                harvestableDatas = DatabaseManager.GetAllRows<BuildingData>().Cast<HarvestableData>().ToList();
                break;
            default:
                break;
        }

        List<string> getStringNames = new List<string>();

        if (harvestableDatas != null)
        {
            for (int i = 0; i < harvestableDatas.Count; i++)
                getStringNames.Add(harvestableDatas[i].id);
        }

        harvestableDataNames = getStringNames.ToArray();
    }

    void ResetData()
    {
        switch (currentType)
        {
            case 0:
                dataToSave = new FishData();
                break;
            case 1:
                dataToSave = new TreeData();
                break;
            case 2:
                dataToSave = new BuildingData();
                break;
            default:
                break;
        }
        pickedIcon = null;
    }

    void LoadData()
    {
        ResetData();
        switch (currentType)
        {
            case 0:
                DatabasePickerWrapper.OpenWindow<FishData>(OnLoadFishData);
                break;
            case 1:
                DatabasePickerWrapper.OpenWindow<TreeData>(OnLoadTreeData);
                break;
            case 2:
                DatabasePickerWrapper.OpenWindow<BuildingData>(OnLoadBuildingData);
                break;
            default:
                break;
        }
    }

    void OnLoadFishData(FishData _dataLoaded) 
    {
        dataToSave = new FishData(_dataLoaded);
        if (dataToSave.spriteId != null)
            pickedIcon = AssetsBundlesManager.GetAssetBundle(BundleName.FishIcons).LoadAsset<Sprite>(dataToSave.spriteId);
        Repaint();
    }

    void OnLoadTreeData(TreeData _dataLoaded)
    {
        dataToSave = new TreeData(_dataLoaded);
        if (dataToSave.spriteId != null)
            pickedIcon = AssetsBundlesManager.GetAssetBundle(BundleName.WoodIcons).LoadAsset<Sprite>(dataToSave.spriteId);
        Repaint();
    }

    void OnLoadBuildingData(BuildingData _dataLoaded)
    {
        dataToSave = new BuildingData(_dataLoaded);
        if (dataToSave.spriteId != null)
            pickedIcon = AssetsBundlesManager.GetAssetBundle(BundleName.CommonIcons).LoadAsset<Sprite>(dataToSave.spriteId);
        Repaint();
    }

    void SaveDataToDatabase()
    {
        switch (currentType)
        {
            case 0:
                DatabaseManager.SaveEntryToDatabase(new FishData((FishData)dataToSave));
                break;
            case 1:
                DatabaseManager.SaveEntryToDatabase(new TreeData((TreeData)dataToSave));
                break;
            case 2:
                DatabaseManager.SaveEntryToDatabase(new BuildingData((BuildingData)dataToSave));
                break;
            default:
                break;
        }


        //SaveLocalizationName();

        bool isANewEntry = harvestableDatas.Find(x => x.Equals(dataToSave)) == null;
        if (isANewEntry)
            confirmationMessage = "Data saved successfully!";
        else
            confirmationMessage = "Data modified successfully!";

        ShowConfirmationMessage = true;

        // Reload
        LoadDatabase();
    }

    void RemoveDataFromDatabase()
    {
        switch (currentType)
        {
            case 0:
                DatabaseManager.RemoveEntryFromDatabase<FishData>(dataToSave.id);
                break;
            case 1:
                DatabaseManager.RemoveEntryFromDatabase<TreeData>(dataToSave.id);
                break;
            case 2:
                DatabaseManager.RemoveEntryFromDatabase<BuildingData>(dataToSave.id);
                break;
            default:
                break;
        }

        confirmationMessage = "Data removed from database!";
        ShowConfirmationMessage = true;

        // Reload
        LoadDatabase();
    }

    // Confirmation message
    string confirmationMessage = "";
    float confirmationTimer = 0.0f;
    bool startTimerHideConfirmation = false;
    bool showConfirmationMessage = false;
    public bool ShowConfirmationMessage
    {
        get
        {
            return showConfirmationMessage;
        }

        set
        {
            showConfirmationMessage = value;
            if (showConfirmationMessage)
            {
                confirmationTimer = Time.realtimeSinceStartup;
                startTimerHideConfirmation = true;
            }
        }
    }
    void DatabaseHandlerEditor(string _toolName, Action _reset, Action _save, Action _load, Action _delete, Data _container, string[] _names)
    {
        GUILayout.BeginHorizontal();
        {
            GUILayout.FlexibleSpace();
            GUILayout.BeginVertical();
            {
                if (GUILayout.Button("Reset " + _toolName))
                {
                    _reset();
                }
                GUILayout.Space(20.0f);

                EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(_container.id));
                if (GUILayout.Button("Save current " + _toolName))
                {
                    if (new List<string>(_names).Contains(_container.id))
                    {
                        if (EditorUtility.DisplayDialog("Overwrite confirmation", _container.id + " already exists. Overwrite?", "Yes", "No"))
                        {
                            _save();
                        }
                    }
                    else
                        _save();
                }
                EditorGUI.EndDisabledGroup();

                GUILayout.BeginHorizontal();
                {
                    if (_names == null)
                        LoadDatabase();

                    if (GUILayout.Button("Load " + _toolName))
                    {
                        _load();
                    }
                    GUI.backgroundColor = Color.red;
                    EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(_container.id));
                    if (GUILayout.Button("Delete"))
                    {
                        if (EditorUtility.DisplayDialog("Delete confirmation", "Are you sure you want to delete " + _container.id + "?", "Yes", "No, I like it too much"))
                        {
                            _delete();
                            _container.id = "";
                        }
                    }
                    GUI.backgroundColor = Color.white;
                    EditorGUI.EndDisabledGroup();
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
            GUILayout.FlexibleSpace();
        }
        GUILayout.EndHorizontal();

        if (ShowConfirmationMessage)
        {
            GUILayout.Space(30.0f);
            GUILayout.Label(confirmationMessage, textStyle);
        }
        else
            GUILayout.Space(50.0f);
    }

    private void OnDestroy()
    {
        AssetsBundlesManager.UnloadAllAssetsBundles(false);
    }
}
