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
            class IFolder
            {
                [SerializeField] internal string m_Name;
                [SerializeField] internal bool m_Delete;
                [SerializeField] internal List<IFolder> m_SubFolders;
                [SerializeField] internal List<IFile> m_Files;
                bool m_FoldOut = false;
                [SerializeField,HideInInspector] bool m_CacheDelete;

                int FilesAndFolderCount => m_Files.Count + m_SubFolders.Count;

                internal IFolder(string path)
                {
                    m_Name = path;
                    m_Delete = true;
                    m_SubFolders = new List<IFolder>();
                    m_Files = new List<IFile>();
                }

                public void Delete()
                {
                    if (!m_Delete)
                        return;

                    foreach (var file in m_Files)
                    {
                        file.Delete();
                    }
                    m_Files.RemoveAll(folder => string.IsNullOrEmpty(folder.m_FilePath));
                    foreach (var subFolder in m_SubFolders)
                    {
                        subFolder.Delete();
                    }

                    RemoveEmptyFolders(this);
                }

                void RemoveEmptyFolders(IFolder parentFolder)
                {
                    for (int i = m_SubFolders.Count - 1; i >= 0; i--)
                    {
                        var subFolder = m_SubFolders[i];
                        subFolder.RemoveEmptyFolders(this);
                    }

                    m_SubFolders.RemoveAll(a => a.FilesAndFolderCount == 0);

                    if (FilesAndFolderCount == 0)
                    {
                        if (parentFolder != null && parentFolder.m_SubFolders.Contains(this))
                        {
                            parentFolder.m_SubFolders.Remove(this);
                        }
                        m_Name = null;
                    }
                }


                void UpdateDeleteStatus(bool delete)
                {
                    foreach (var t in m_SubFolders)
                    {
                        t.m_Delete = delete;
                        t.UpdateDeleteStatus(delete);
                    }
                       
                    foreach (var t in m_Files)
                        t.m_Delete = delete;
                }
                public void OnGUI()
                {
                    if (string.IsNullOrEmpty(m_Name))
                        return;

                   
                    EditorGUILayout.BeginHorizontal(m_Delete ? EditorStyles.helpBox : EditorStyles.linkLabel);
                    
                    m_Delete = EditorGUILayout.ToggleLeft(string.Empty, m_Delete, GUILayout.Width(EditorGUI.indentLevel*20));
                    
                    if(m_Delete != m_CacheDelete)
                    {
                        UpdateDeleteStatus(m_Delete);
                    }
                    m_CacheDelete = m_Delete;

                    m_FoldOut = EditorGUILayout.Foldout(m_FoldOut, m_Name);

                    EditorGUILayout.LabelField(FilesAndFolderCount.ToString(),new GUIStyle(EditorStyles.boldLabel) { alignment= TextAnchor.MiddleRight});
                    EditorGUILayout.EndHorizontal();
                    if (!m_FoldOut)
                        return;
                    EditorGUI.indentLevel++;

                    foreach (var subFolder in m_SubFolders)
                    {
                        subFolder.OnGUI();
                    }

                    foreach (var file in m_Files)
                    {
                        file.OnGUI();
                    }

                    EditorGUI.indentLevel--;
                }
            }


            [System.Serializable]
            internal class IFile
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
                    m_Delete = EditorGUILayout.ToggleLeft(string.Empty, m_Delete, GUILayout.Width(EditorGUI.indentLevel * 20));
                    EditorGUILayout.LabelField(m_FilePath);
                    if (GUILayout.Button("Ping", GUILayout.Width(50)))
                    {
                        Selection.activeObject = AssetDatabase.LoadAssetAtPath(m_FilePath,typeof(Object));
                    }
                    EditorGUILayout.EndHorizontal();
                }
            }



            public string PackageName;
            [SerializeField] List<IFolder> m_IFolders;
            [HideInInspector,SerializeField]bool m_FoldOut=false;

            internal IPackageFiles(string packageName, string[] files)
            {
                PackageName = packageName;
                m_IFolders = new List<IFolder>();

                Dictionary<string, IFolder> folderDictionary = new Dictionary<string, IFolder>();

                foreach (var filePath in files)
                {
                    string[] pathParts = filePath.Split('/');
                    IFolder currentFolder = null;

                    for (int i = 0; i < pathParts.Length - 1; i++)
                    {
                        string folderPath = string.Join("/", pathParts, 0, i + 1);

                        if (!folderDictionary.TryGetValue(folderPath, out currentFolder))
                        {
                            currentFolder = new IFolder(folderPath);
                            folderDictionary.Add(folderPath, currentFolder);

                            if (i == 0)
                            {
                                m_IFolders.Add(currentFolder);
                            }
                            else
                            {
                                string parentFolderPath = string.Join("/", pathParts, 0, i);
                                IFolder parentFolder = folderDictionary[parentFolderPath];
                                parentFolder.m_SubFolders.Add(currentFolder);
                            }
                        }
                    }

                    IFile file = new IFile(filePath);
                    currentFolder.m_Files.Add(file);
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
                    if (EditorUtility.DisplayDialog("Delete package files", $"Are you sure you want to delete all selected [{m_IFolders.Count}] folders. \n\n\n PLEASE NOTE THIS ACTION CAN NOT BE UNDONE", "Delete", "Cancel"))
                    {
                        Delete();
                        m_IFolders.RemoveAll(folder => string.IsNullOrEmpty(folder.m_Name));
                        if (m_IFolders.Count == 0)
                        {
                            Load().m_Installed.Remove(this);
                        }
                        if (m_IFolders.Count == 0)
                            Load().m_Installed.Remove(this);
                        EditorUtility.SetDirty(Load());
                        AssetDatabase.SaveAssets();
                        return;
                    }
                }

                if (GUILayout.Button("Select All", GUILayout.Width(80)))
                {
                    foreach (var folder in m_IFolders)
                    {
                        folder.m_Delete = true;
                    }
                }

                if (GUILayout.Button("Select None", GUILayout.Width(80)))
                {
                    foreach (var folder in m_IFolders)
                    {
                        folder.m_Delete = false;
                    }
                }

                EditorGUILayout.EndHorizontal();

                EditorGUI.indentLevel++;

                if (m_FoldOut)
                {
                    foreach (var folder in m_IFolders)
                    {
                        folder.OnGUI();
                    }
                }

                EditorGUI.indentLevel--;

                EditorGUILayout.Space(20);
            }
            public void Delete()
            {
                foreach (var t in m_IFolders)
                {
                    t.Delete();
                }

                EditorUtility.SetDirty(Load());
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

           //AdvertiserEditorWindow.ShowPanel<DeletePackage>();
        }
    }
}
#endif