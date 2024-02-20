#if UNITY_EDITOR
using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace SH.Ads.Editor
{
    public class InstalledPackages : ScriptableObject
    {
        const string Location = "Assets/_Shoaib/Ads/Scripts/Editor/Data/InstalledPackages.asset";

        [System.Serializable]
        class PackageFiles
        {
            public string PackageName;
            public string[] files;
        }

        [SerializeField] List<PackageFiles> Installed = new List<PackageFiles>();
        public string ActivePackage;

        
        public static InstalledPackages Load()
        {
            var installedPackage = AssetDatabase.LoadAssetAtPath<InstalledPackages>(Location);
            if (installedPackage == null)
            {
                installedPackage = CreateInstance<InstalledPackages>();
                AssetDatabase.CreateAsset(installedPackage, Location);
                AssetDatabase.SaveAssets();
            }
            return installedPackage;
        }
        public void AddInstalled(string packageID, string[] addedObjects)
        {
            ActivePackage = string.Empty;
            if (Installed.Count!=0 &&HasPackage(packageID))
            {
                SearchPackageByName(packageID).files=addedObjects;
                EditorUtility.SetDirty(this);
                return;
            }

            Installed.Add(new PackageFiles() { PackageName=packageID, files = addedObjects});
            EditorUtility.SetDirty(this);
        }

        PackageFiles SearchPackageByName(string packageName)
        {
            return Installed.Find(package => package.PackageName == packageName);
        }

        public bool HasPackage(string packageID) => Installed.Find(package => package.PackageName == packageID) != null;

        public void RemoveInstalled(string packageID)
        {
            if (!HasPackage(packageID))
                return;
            foreach(var t in SearchPackageByName(packageID).files)
            {
                if (!string.IsNullOrEmpty(t))
                {
                    if (File.Exists(t))
                    {
                        AssetDatabase.DeleteAsset(t);
                    }
                }
            }
            Installed.Remove(SearchPackageByName(packageID));
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
    }
}
#endif