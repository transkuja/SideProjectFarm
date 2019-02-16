using System;
using System.Collections.Generic;
using UnityEngine;

public enum BundleName {
    FishIcons,
    WoodIcons,
    CommonIcons,
    Size
}

public static class AssetsBundlesManager {

    public static AssetBundle[] loadedBundles = new AssetBundle[(int)BundleName.Size];
    public static bool[] isBundleLoaded = new bool[(int)BundleName.Size];

    public static AssetBundle GetAssetBundle(BundleName _bundleName)
    {
        if (!isBundleLoaded[(int)_bundleName] || loadedBundles[(int)_bundleName] == null)
        {
            if (loadedBundles[(int)_bundleName] != null)
                loadedBundles[(int)_bundleName].Unload(false);

            LoadAssetBundle(_bundleName);
        }

        return loadedBundles[(int)_bundleName];
    }

    public static void LoadAssetBundle(BundleName _bundleName)
    {
        string fullPath = GetAssetsPathFromBundleName(_bundleName);


        loadedBundles[(int)_bundleName] = AssetBundle.LoadFromFile(fullPath);
        isBundleLoaded[(int)_bundleName] = true;
    }

    public static void UnloadBundle(BundleName _bundleName, bool _unloadAllLoadedObjects)
    {
        if (isBundleLoaded[(int)_bundleName])
        {
            loadedBundles[(int)_bundleName].Unload(_unloadAllLoadedObjects);
            isBundleLoaded[(int)_bundleName] = false;
        }
    }

    public static void UnloadAllAssetsBundles(bool _unloadAllLoadedObjects)
    {
        AssetBundle.UnloadAllAssetBundles(_unloadAllLoadedObjects);
        for (int i = 0; i < (int)BundleName.Size; i++)
            isBundleLoaded[i] = false;
    }

    static string GetAssetsPathFromBundleName(BundleName _bundleName)
    {
        return System.IO.Path.Combine(Application.streamingAssetsPath, Enum.GetName(typeof(BundleName), _bundleName).ToLower());
    }
}
