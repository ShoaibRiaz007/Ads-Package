#if UNITY_EDITOR
using SH.Ads.Base;
using SH.Ads.Editor.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
namespace SH.Ads.Editor
{
    public class AddAdvertiser : ITab
    {
        public override GUIContent Title => new GUIContent("Add Advertiser", Texture(), "Panel to add new advertiser to active list");

        protected override string IconBase64 => "iVBORw0KGgoAAAANSUhEUgAAABkAAAAZCAYAAADE6YVjAAADQklEQVRIDZXVWYiOURjAcfsyQ1nHyBaNdbLLFlKWC+5ky76WMBdjSWquFJEiWUKyhBB3RJYYe0LWcENqZmxjxtiNbfz/X9+ZXl/zzfDUb97znnd5znnOeb+pXq3yqM7ldAxGJ+QjFwX4jRCNaTSB90fjMydvEjujN9juiCXIwxO0QwZ24Q5CHKDxET9DB8caaIpsG5XFYi5ex3s0xx6cwlREoysnWVgJZ9kGO1GE1KqSjOKm4xiKnrgEZ98X0ejPibPIRDNMwH3ESlor3rmMo9MNkUpjC76jLn6gDPfwLY5Dedyi5UDGYxxO4jxiYZJGcFE3YSAsTRjREdpTcBRfcBO+6AqikcbJJ3hfIcoT0K5mEsM6XoQleQMX2nCBc9ALLvQkOKPVsNQO7gUsVUM8wDMYteH7y0ISO8ciBT4QwlEtgzXvgYO4BsvYB9lx6zk+RPR9rkcuiqKdNemoD4/RcD2uxnmtMSzpLGxFK5zFFlQYiburqu9mDG/Zjs7YALf1AtRB0ghJrHknuLgZ6IdomHxR3DaOBcjDK6yFpUoaTt/6lsbv8Gh5buMGPHfnLEUm5uNx/NzRX8YwrIC70GRv8VeE8rSg1/3tC41f2IfaWIh87MBXGN2xCVPhZklHA3xCNJzp85DE8szAYbgrjNZw5C7uOVjKED5ncjfBGuyFOzEMgmZsp/mO5WF3uTbFsEQ+mAMXtyX87Yom8F5LdRpuXddyBNxlzrwerITPbEZqSEK7PEbTctqWbxomwtmkogtcm26wRM7cUhr+/MzESJgsC7FITOLaTMdFOJpDGIS5GIDXeIkLyEUpQjiIjnD0K+FgruJDYpIxdL5Db/hABrLRBpPwBF4z8WlE4wMnd+F39AgmcMZfw3dCOxYn+Gvfe+zAHDzDbsyGZXTbpyAafgZ+mGcwHAvhzOYhLXEmhXROhuHLTGidh8CfnGSxkwv+gzNZCJ/1mylJTBJu8Oia6Bic/gski41caAkHFsJf6xIUu9/b4z7cHf8T+dzc4V8ecEpm3I+mcM+3RZ0kOtPv1vUYLQ2nycNy+REWYB3cdp/xFBWF/9RewZ20qqIbKuqzXPJ3x5lUtkZcjoUzL4XJfsZ6qvjzB9LdvrwmFp1xAAAAAElFTkSuQmCC";

        static AdSettings AdSetting;
        static SupportedAdvertisers selectedAdvertiser = SupportedAdvertisers.Admob;
        bool newAdvertiserIsAndroid = true;
        bool newAdvertiserIsIOS = false;
        string newAdvertiserID;

        public override void OnEnable(AdSettings settings)
        {
            AdSetting = settings;
            EditorUtility.SetDirty(settings);
        }

        public override void OnGUI()
        {
            Header();
            Content();
        }
        void Header()
        {
            EditorGUILayout.HelpBox("This window is used to used to add Advertisers in the project. ", MessageType.Info);
            EditorGUILayout.Space();
        }

        void Content()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.HelpBox("You can add multiple advertisers. When you add an advertiser, it may take some time as it requires recompilation of the code. You can edit advertiser IDs even after they have been added.", MessageType.Info);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Add New Advertiser", EditorStyles.boldLabel);
            FilteredAdvertiserDropdown();
            newAdvertiserID = EditorGUILayout.TextField(new GUIContent("Advertiser ID:", "Enter a unique ID for the advertiser"), newAdvertiserID);
            EditorGUILayout.BeginHorizontal();
            newAdvertiserIsAndroid = EditorGUILayout.Toggle(new GUIContent("Is Android:", "Check if this advertiser is for Android"), newAdvertiserIsAndroid);
            newAdvertiserIsIOS = EditorGUILayout.Toggle(new GUIContent("Is iOS:", "Check if this advertiser is for iOS"), newAdvertiserIsIOS);
            EditorGUILayout.EndHorizontal();
            if (GUILayout.Button(new GUIContent("Add Advtiser", "Add selectd Advertiser to active list")))
            {
                OnAddAdvertiserClick();
            }
            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }
        private void OnAddAdvertiserClick()
        {
            if (!AdSetting.CurrentPipline.Advertisers.Any(advertiser => advertiser.advertiser == selectedAdvertiser))
            {
                if (selectedAdvertiser.IsInstalled())
                {

                    Ads.Base.Advertiser newAdvertiser = new Ads.Base.Advertiser
                    {
                        advertiser = selectedAdvertiser,
                        ID = newAdvertiserID,
                        IsAndroid = newAdvertiserIsAndroid,
                        IsIOS = newAdvertiserIsIOS,
                        Ads = new List<AD>()
                    };
                    AdSetting.CurrentPipline.Advertisers.Add(newAdvertiser);
                    EditorUtility.SetDirty(AdSetting);
                    selectedAdvertiser.AddToRegistry();
                }
                else
                    AdvertiserEditorWindow.ShowPanel<InstallPackage>();
            }
            else
                EditorUtility.DisplayDialog("Warning", $"Advertiser [{selectedAdvertiser}] already exist in active list.", "OK");
        }

        private void FilteredAdvertiserDropdown()
        {
            List<SupportedAdvertisers> availableAdvertisers = Enum.GetValues(typeof(SupportedAdvertisers))
                .Cast<SupportedAdvertisers>()
                .Where(advertiser => !AdSetting.CurrentPipline.Advertisers.Any(a => a.advertiser == advertiser))
                .ToList();

            int selectedIndex = availableAdvertisers.IndexOf(selectedAdvertiser);
            selectedIndex = EditorGUILayout.Popup(new GUIContent("Advertiser:", "Select an advertiser to add"), selectedIndex, availableAdvertisers.Select(a => a.ToString()).ToArray());

            if (selectedIndex >= 0 && selectedIndex < availableAdvertisers.Count)
            {
                selectedAdvertiser = availableAdvertisers[selectedIndex];
            }
        }
    }
}
#endif