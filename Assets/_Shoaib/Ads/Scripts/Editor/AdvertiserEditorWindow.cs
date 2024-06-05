#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using SH.Ads.Editor.Base;
using System.IO;
using SH.Ads.Editor;
using SH.Ads.Piplines;

namespace SH.Ads
{
    public class AdvertiserEditorWindow : EditorWindow
    {
        const string JSON_SAVE_PATH_PIPLINE = "Assets/_Shoaib/Ads/Json/Ad Setting.json";
        const string JSON_SAVE_PATH_ConfigData = "Assets/_Shoaib/Ads/Json/Remote Config Data.json";
        static AdSettings AdSetting;
        static Vector2 ScrollPosition = Vector2.zero;

        static int CurrentPanelIndex { get => EditorPrefs.GetInt("Wellcome_Current_Panel", 0); set => EditorPrefs.SetInt("Wellcome_Current_Panel", value); }

        static ITab[] AllWindows;

        static ITab currentWindow = null;

        [MenuItem("SH/Ad Manager")]
        public static void ShowWindow()
        {
            var tem = GetWindow<AdvertiserEditorWindow>("Ad Manager");
            tem.minSize = new Vector2(730,530);
            tem.Show();
            Extensions.CheckForInstalledPackages();
        }

        private void OnEnable()
        {
            Init();
            AllWindows = Extensions.GetInstanceOfAllSubClasses<ITab>().ToArray();
            currentWindow = AllWindows[CurrentPanelIndex];
            currentWindow.OnEnable(AdSetting);
           
        }
        private void OnDisable()
        {
            EditorUtility.SetDirty(AdSetting);
            EditorUtility.SetDirty(AdSetting.CurrentPipline);
        }

        protected void OnGUI()
        {
            Top();
            EditorGUILayout.BeginHorizontal();
            Left();
            Right();
            EditorGUILayout.EndHorizontal();
        }

        void Left()
        {
            EditorGUILayout.BeginVertical(new GUIStyle(EditorStyles.helpBox) { fixedWidth = 180, stretchHeight=true});
           for(int i=0;i< AllWindows.Length;i++)
                AddContentItem(AllWindows[i],i);
            EditorGUILayout.EndVertical();
        }

        void AddContentItem(ITab window,int index)
        {
            if (GUILayout.Button(window.Title, new GUIStyle(EditorStyles.toolbarButton) { margin = new RectOffset(5, 5, 5, 5), fixedHeight = 30, alignment = TextAnchor.MiddleLeft, normal = { textColor = (CurrentPanelIndex == index ? Color.green : Color.black) } }))
            { 
                currentWindow = window;
                currentWindow.OnEnable(AdSetting);
                CurrentPanelIndex = index;
            }
        }

        void Right()
        {
            ScrollPosition = EditorGUILayout.BeginScrollView(ScrollPosition, new GUIStyle(EditorStyles.helpBox) { stretchHeight = true });
            currentWindow?.OnGUI();
            EditorGUILayout.EndScrollView();
        }

        void Top()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            if (GUILayout.Button(new GUIContent("To Json", "Convert to json for remote config or custom API call"), new GUIStyle(EditorStyles.toolbarButton) { margin = new RectOffset(5, 5, 5, 5), fixedHeight = 30, alignment = TextAnchor.MiddleLeft }))
            {
                SetDirty();
                Selection.activeObject= AssetDatabase.LoadAssetAtPath(JSON_SAVE_PATH_PIPLINE, typeof(TextAsset));
            }
            if (GUILayout.Button(new GUIContent("Select Ad Setting", "Ad setting scriptable object"), new GUIStyle(EditorStyles.toolbarButton) { margin = new RectOffset(5, 5, 5, 5), fixedHeight = 30, alignment = TextAnchor.MiddleRight }))
            {
                Selection.activeObject = AdSetting;
            }
            EditorGUILayout.EndHorizontal();
        }

        public static new void SetDirty()
        {
            string json = JsonUtility.ToJson(AdSetting.CurrentPipline, true);
           
            File.WriteAllText(JSON_SAVE_PATH_PIPLINE, json);
#if RemoteConfig
            json = JsonUtility.ToJson(Ads.Adons.RemoteConfig.Load().m_RemoteData, true);
            File.WriteAllText(JSON_SAVE_PATH_ConfigData, json);
#endif
            AssetDatabase.Refresh();
        }

        public static string AfterSertialization()
        {
            return File.ReadAllText(JSON_SAVE_PATH_PIPLINE);
        }

        
        static void Init()
        {
            FileHelper.CreateFolderHierarchy();
            AdSetting = AdSettings.Load();
            if (AdSetting.CurrentPipline == null)
                AdSetting.CurrentPipline = Waterfall.Load();
            for (int i = 0; i < AdSetting.CurrentPipline.Advertisers.Count; i++)
            {
                if (AdSetting.CurrentPipline.Advertisers[i].advertiser == SupportedAdvertisers.Admob)// Update AD ID in google admob settings
                {
                    AdSetting.CurrentPipline.Advertisers[i].UpdateAdmobSettings();
                    break;
                }
            }
        }

        internal static void ShowPanel<T>() where T : ITab
        {
           foreach(var t in AllWindows)
            {
                if(typeof(T) == t.GetType())
                {
                    currentWindow = t;
                    currentWindow.OnEnable(AdSetting);
                }
            }
        }
    }
}
#endif