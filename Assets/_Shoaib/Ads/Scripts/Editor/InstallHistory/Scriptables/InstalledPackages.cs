#if UNITY_EDITOR
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
                [SerializeField] internal string m_FilePath;
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
                    m_FilePath = null;
                }

                public void OnGUI()
                {
                    if (string.IsNullOrEmpty(m_FilePath))
                        return;

                    EditorGUILayout.BeginHorizontal(m_Delete ? EditorStyles.helpBox : EditorStyles.linkLabel);
                    m_Delete = EditorGUILayout.ToggleLeft(string.Empty, m_Delete, GUILayout.Width(30));
                    EditorGUILayout.LabelField(m_FilePath);
                    if (GUILayout.Button("Ping", GUILayout.Width(50)))
                    {
                        Selection.activeObject = AssetDatabase.LoadAssetAtPath(m_FilePath,typeof(Object));
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }


            public string PackageName;
            [SerializeField] List<IFile> m_IFiles;
            [HideInInspector,SerializeField]bool m_FoldOut=false;
            internal IPackageFiles(string packageName,string[] files)
            {
                PackageName=packageName;
                int count = files.Length;
                m_IFiles = new  List<IFile>();
                for(int i = 0; i < count; i++)
                {
                    if (!string.IsNullOrEmpty(files[i]))
                        m_IFiles.Add(new IFile(files[i]));
                }
            }

            public void OnGUI()
            {
                EditorGUILayout.BeginHorizontal();
                
               

                m_FoldOut = EditorGUILayout.Foldout(m_FoldOut, PackageName, EditorStyles.foldoutHeader);

                if (GUILayout.Button("Delete Selected Package Files", new GUIStyle(EditorStyles.toolbarButton)
                {
                    fixedWidth = 200,
                    normal = { textColor = Color.blue },
                    hover = { textColor = Color.red }// Set the text color to red
                }))
                {
                    if (EditorUtility.DisplayDialog("Delete package files", $"Are you sure you want to delete all selected [{m_IFiles.Count}] files. \n\n\n PLEASE NOTE THIS ACTION CAN NOT BE UNDONE", "Delete", "Cancel"))
                    {
                        Delete();
                        m_IFiles.RemoveAll(file => string.IsNullOrEmpty(file.m_FilePath));
                        if (m_IFiles.Count == 0)
                        {
                            Load().m_Installed.Remove(this);
                        }
                        if (m_IFiles.Count == 0)
                            Load().m_Installed.Remove(this);
                        EditorUtility.SetDirty(Load());
                        AssetDatabase.SaveAssets();
                        return;
                    }
                }


                if (GUILayout.Button("Select All", GUILayout.Width(80)))
                {
                    for (int i = 0; i < m_IFiles.Count; i++)
                        m_IFiles[i].m_Delete = true;
                }

                if (GUILayout.Button("Select None", GUILayout.Width(80)))
                {
                    for (int i = 0; i < m_IFiles.Count; i++)
                        m_IFiles[i].m_Delete = false;
                }
                EditorGUILayout.EndHorizontal();
                EditorGUI.indentLevel++;
                if (m_FoldOut)
                    for(int i=0;i<m_IFiles.Count;i++)
                    m_IFiles[i].OnGUI();


                EditorGUI.indentLevel--;

                EditorGUILayout.Space(20);
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


        internal ref List<IPackageFiles> Installed =>ref m_Installed;

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

            AdvertiserEditorWindow.ShowPanel<DeletePackage>();
        }
    }
}
#endif