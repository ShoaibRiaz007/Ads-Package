#if UNITY_EDITOR
using Codice.CM.Common.Serialization.Replication;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace SH.Ads.Editor
{
    public class InstalledPackages : ScriptableObject
    {
        const string Location = "Assets/_Shoaib/Ads/Scripts/Editor/InstallHistory/Data/InstalledPackages.asset";

        [System.Serializable]
        internal class IPackageFiles
        {
            [System.Serializable]
            class IFile
            {
                [SerializeField] string m_FilePath;
                [SerializeField] internal bool m_Delete;

                internal IFile(string path)
                {
                    m_FilePath = path;
                    m_Delete = true;
                }

                public void Delete()
                {
                    if (!m_Delete)
                        return;

                    if (!string.IsNullOrEmpty(m_FilePath))
                    {
                        if (File.Exists(m_FilePath))
                        {
                            AssetDatabase.DeleteAsset(m_FilePath);
                        }
                    }
                }

                public void OnGUI()
                {
                    if (string.IsNullOrEmpty(m_FilePath))
                        return;

                    EditorGUILayout.BeginHorizontal(m_Delete ? EditorStyles.helpBox : EditorStyles.linkLabel);
                    m_Delete = EditorGUILayout.ToggleLeft("", m_Delete, GUILayout.Width(15));
                    EditorGUILayout.LabelField(m_FilePath);
                    if (GUILayout.Button("Ping", GUILayout.Width(50)))
                    {
                        Selection.activeObject = AssetDatabase.LoadAssetAtPath(m_FilePath,typeof(Object));
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }


            public string PackageName;
            [SerializeField] IFile[] m_IFiles;
            [HideInInspector,SerializeField]bool foldout=true;
            internal IPackageFiles(string packageName,string[] files)
            {
                PackageName=packageName;
                int count = files.Length;
                m_IFiles = new IFile[count];
                for(int i = 0; i < count; i++)
                {
                    m_IFiles[i] = new IFile(files[i]);
                }
            }

            public void OnGUI()
            {
                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button("Select All"))
                {
                    for (int i = 0; i < m_IFiles.Length; i++)
                        m_IFiles[i].m_Delete=true;
                }

                if (GUILayout.Button("Select None"))
                {
                    for (int i = 0; i < m_IFiles.Length; i++)
                        m_IFiles[i].m_Delete = false;
                }
                EditorGUILayout.EndHorizontal();

                foldout = EditorGUILayout.Foldout(foldout, PackageName, EditorStyles.foldoutHeader);

                EditorGUI.indentLevel++;
                if (foldout)
                    for(int i=0;i<m_IFiles.Length;i++)
                    m_IFiles[i].OnGUI();

                EditorGUI.indentLevel--;
            }

            public void Delete()
            {
                foreach (var t in m_IFiles)
                {
                    t.Delete();
                }
            }
        }


        [SerializeField] List<IPackageFiles> m_Installed = new List<IPackageFiles>();

        public string ActivePackage;

        public static InstalledPackages Load()
        {
            var m_InstalledPackage = AssetDatabase.LoadAssetAtPath<InstalledPackages>(Location);
            if (m_InstalledPackage == null)
            {
                m_InstalledPackage = CreateInstance<InstalledPackages>();
                AssetDatabase.CreateAsset(m_InstalledPackage, Location);
                AssetDatabase.SaveAssets();
            }
            return m_InstalledPackage;
        }
        public void AddInstalled(string packageID, string[] addedObjects)
        {
            ActivePackage = string.Empty;
            if (m_Installed.Count!=0 &&HasPackage(packageID))
            {
                for(int i=0;i<m_Installed.Count;i++)
                {
                    if (m_Installed[i].PackageName == packageID)
                    {
                        m_Installed[i] = new IPackageFiles(packageID, addedObjects);
                        break;
                    }
                }
                EditorUtility.SetDirty(this);
                return;
            }

            m_Installed.Add(new IPackageFiles(packageID, addedObjects));
            EditorUtility.SetDirty(this);
        }
        public bool HasPackage(string packageID) => m_Installed.Find(package => package.PackageName == packageID) != null;

        public void RemoveInstalled(string packageID)
        {
            if (!HasPackage(packageID))
                return;

            for (int i = 0; i < m_Installed.Count; i++)
            {
                if (m_Installed[i].PackageName == packageID)
                {
                    Window.ShowWindow(m_Installed[i]);
                    break;
                }
            }
           
        }

        class Window : EditorWindow
        {
            static IPackageFiles Package;
            static Vector2 ScrollPos = Vector2.zero;
            internal static void ShowWindow(IPackageFiles package)
            {
                GetWindow<Window>("");
                Package = package;
            }

            private void OnGUI()
            {
                if(Package == null)
                {
                    Close();
                    return;
                }

                ScrollPos = EditorGUILayout.BeginScrollView(ScrollPos, new GUIStyle(EditorStyles.helpBox));

                Package.OnGUI();

                EditorGUILayout.EndScrollView();

                if (GUILayout.Button("Delete Selected Package Files", new GUIStyle(EditorStyles.toolbarButton) { fixedHeight = 50, }))
                {
                    if (EditorUtility.DisplayDialog("Delete package files", $"Are you sure you want to delete all selected files. \n\n\n PLEASE NOTE THIS ACTION CAN NOT BE UNDONE", "Delete", "Cancel"))
                    {
                        Package.Delete();
                        Load().m_Installed.Remove(Package);
                        EditorUtility.SetDirty(this);
                        AssetDatabase.SaveAssets();
                        Close();
                        return;
                    }
                }
            }
        }
    }
}
#endif