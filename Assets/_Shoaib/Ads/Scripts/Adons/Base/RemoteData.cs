using UnityEngine;
[CreateAssetMenu(fileName = "RemoteData", menuName = "SH/Ads/Adons/Create Remote Config Data", order = 1)]
public class RemoteData : ScriptableObject
{
#if UNITY_EDITOR
    const string Location = "Assets/_Shoaib/Ads/Data/RemoteData.asset";
    public static RemoteData Load()
    {
        var remoteConfig = UnityEditor.AssetDatabase.LoadAssetAtPath<RemoteData>(Location);
        if (remoteConfig == null)
        {
            remoteConfig = CreateInstance<RemoteData>();
            UnityEditor.AssetDatabase.CreateAsset(remoteConfig, Location);
            UnityEditor.AssetDatabase.SaveAssets();
        }
        return remoteConfig;
    }
#endif

    public bool ShowInterstialMainMenu=true;
    public bool LoadScene0=true;
}
