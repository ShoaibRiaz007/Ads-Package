#if UNITY_EDITOR
using SH.Ads.Editor.Base;
using UnityEditor;
using UnityEngine;

namespace SH.Ads.Editor
{
    public class InstallPackage : ITab
    {
        static Vector2 scrollPos;
        static int CurrentPanelIndex { get => EditorPrefs.GetInt("Install_Current_Panel", 0); set => EditorPrefs.SetInt("Install_Current_Panel", value); }

        InstalledPackages installedPackagesFiles = null;

        public override GUIContent Title => new GUIContent("Package Installer", Texture(), "Panel to intall new advertiser");

        protected override string IconBase64 => "iVBORw0KGgoAAAANSUhEUgAAABkAAAAZCAYAAADE6YVjAAABcklEQVRIDe1UsU6EQBBd5IwtlMaEeIl2xo7CWPMLNjR0F0ob+AITqSyJteEbrLSzN8bOwlDYXE5JCI2SA+edt3dkdWFJ6LxJht3ZefPeMtldVtc1U3HHcW4ZYzV3xCp1wGxRkaqdCEAxFtLrsI/IuqrnbCPSq2Gbdv3TdkmflCAILqknq2ekaw687JlpO12PRFwoNh844P82mfp0Ot2mign5J3nbHyE/AV7GJW0XCoqi2LFt+4JISolQiTxwMgGst4oAkKbprud5NyQyF4TmWEe+TUBJZCk0ph3fNUUQk8C4SwD5UZ7n+77vPxDBwpIk2eNzPlqW9WoYxhnF9+TH5E+Iaf2dY5qj67pvPI7j+FTLsuzANM0XvkjKGp+LYxiGR7R2Tn4VRdGzmOexpmk4KAsj/kMGEYpWp0fl97swTT7wt92Tn60M8B2JHFVV4X4Mar9EdF3/GlSByNAuXLSPoYmXfOAtITIjvybPyYc08IF39g1zwc79y7g06AAAAABJRU5ErkJggg==";
        public InstallPackage()
        {
            AssetDatabase.importPackageCompleted -= ImportPackageCompleted;
            AssetDatabase.importPackageCancelled -= ImportPackageCanceled;
            AssetDatabase.importPackageStarted -= ImportPackageStarted;
            AssetDatabase.importPackageFailed -= ImportPackageFailed;
            AssetDatabase.onImportPackageItemsCompleted -= ImportedItems;

            AssetDatabase.importPackageCompleted += ImportPackageCompleted;
            AssetDatabase.importPackageCancelled += ImportPackageCanceled;
            AssetDatabase.importPackageStarted += ImportPackageStarted;
            AssetDatabase.importPackageFailed += ImportPackageFailed;
            AssetDatabase.onImportPackageItemsCompleted += ImportedItems;
        }

        IInstaller[] AllInstallers;

        IInstaller ActiveInstaller;
        AdSettings settings;
        public override void OnEnable(AdSettings settings)
        {
            this.settings = settings;
            Extensions.CheckForInstalledPackages();
            EditorUtility.SetDirty(settings);
            installedPackagesFiles =InstalledPackages.Load();
            AllInstallers = Extensions.GetInstanceOfAllSubClasses<IInstaller>().ToArray();
            if (AllInstallers.Length != 0)
            {
                ActiveInstaller = AllInstallers[0];
                ActiveInstaller.OnEnable(settings);
            }
                
        }

        public override void OnGUI()
        {
            Header();
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, EditorStyles.helpBox);
            if (ActiveInstaller != null)
            {
                ActiveInstaller.OnGUI();
            }
            EditorGUILayout.EndScrollView();
        }
        void Header()
        {
            EditorGUILayout.BeginHorizontal(EditorStyles.helpBox);
            for (int i = 0; i < AllInstallers.Length; i++)
                AddContentItem(AllInstallers[i], i);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }
        void AddContentItem(IInstaller tab, int index)
        {
            if (GUILayout.Button(tab.Title, new GUIStyle(EditorStyles.toolbarButton) { margin = new RectOffset(5, 5, 5, 5), fixedHeight = 30, alignment = TextAnchor.MiddleLeft, normal = { textColor = (CurrentPanelIndex == index ? Color.green : Color.black) }}))
            {
                ActiveInstaller = tab;
                ActiveInstaller.OnEnable(settings);
                CurrentPanelIndex = index;
            }
        }
        private void ImportedItems(string[] obj)
        {
            if (!installedPackagesFiles ||string.IsNullOrEmpty(installedPackagesFiles.ActivePackage))
            {
                return;
            }
            if(ActiveInstaller.GetType() == typeof(AdonsPackages))
            {
                var t = (AdonsPackages)ActiveInstaller;
                for (int i = 0; i < t.allAdons.Length; i++)
                {
                    t.allAdons[i].CheckIfInstalled();
                }
            }
           
            installedPackagesFiles.AddInstalled(installedPackagesFiles.ActivePackage, obj);
           
        }
        private void ImportPackageCompleted(string packageName)
        {
            EditorUtility.DisplayDialog("Package successfully Imported", $"Package  [{packageName}] imported successfully", "OK");
        }
        private void ImportPackageStarted(string packageName)
        {
            
        }
        private void ImportPackageFailed(string packageName, string errorMessage)
        {
            EditorUtility.DisplayDialog("Importing package failled", $"Importing package [{packageName}] failled with error [{errorMessage}]", "OK");
            installedPackagesFiles.ActivePackage = string.Empty;

        }
        private void ImportPackageCanceled(string packageName)
        {
            EditorUtility.DisplayDialog("Importing package Canceled", $"Importing package [{packageName}] is cancelled by user", "OK");
            installedPackagesFiles.ActivePackage = string.Empty;
        }
    }
}
#endif