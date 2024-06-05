#if UNITY_EDITOR
using System.Collections.Generic;
using System;
using UnityEditor;
using UnityEngine;
using System.Linq;
using SH.Ads.Base;

namespace SH.Ads.Editor.Base
{
    public class IPipelineEditor
    {
        
        private AdType SelectedAdType = AdType.Banner;
        private string AdvertiserIDCache=string.Empty;
        protected static GUIStyle m_EditorButonStyle, m_EditorFoldOutSyle;

        public virtual void OnGUI<T>(ref T CurrentPipline) where T : IPipeline
        {
            var pipeline = CurrentPipline as IPipeline;
            if (pipeline == null)
                return;

            if (m_EditorButonStyle == null)
                m_EditorButonStyle = new GUIStyle(EditorStyles.objectField) { fixedHeight = 40, padding = new RectOffset() { top = 10 } };
            if (m_EditorFoldOutSyle == null)
                m_EditorFoldOutSyle = new GUIStyle(EditorStyles.foldout) { fontStyle = FontStyle.Bold };


            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            Header();
            Content(ref pipeline);
            Footer();
            EditorGUILayout.EndScrollView();
        }

        void Header()
        {
            EditorGUILayout.Space(5);
            EditorGUILayout.LabelField("Active Advertisers -:", EditorStyles.boldLabel);
            
        }

        void Content(ref IPipeline pipeline)
        {
            for (int i = 0; i < pipeline.Advertisers.Count; i++)
            {
                pipeline.Advertisers[i].order = i;
                DisplayAdvertiser(pipeline.Advertisers[i],ref pipeline);
            }
        }
        void Footer()
        {
           
        }
        void DisplayAdvertiser(Ads.Base.Advertiser advertiser , ref IPipeline pipeline)
        {
EditorGUILayout.BeginHorizontal(m_EditorButonStyle);

           if (advertiser.order >= 1)
            {
                if (GUILayout.Button(new GUIContent("^", "Move Up in Priority"), new GUIStyle(GUI.skin.button) { alignment = TextAnchor.MiddleCenter, fixedWidth = 20 }))
                {
                    SwapAdvertisers(advertiser.order, advertiser.order-1, ref pipeline);
                    EditorUtility.SetDirty(pipeline);
                }
            }
            if (advertiser.order < pipeline.Advertisers.Count - 1)
            {
                if (GUILayout.Button(new GUIContent("v", "Move Down in Priority"), new GUIStyle(GUI.skin.button) { alignment = TextAnchor.MiddleCenter, fixedWidth = 20 }))
                {
                    SwapAdvertisers(advertiser.order, advertiser.order + 1,ref pipeline);
                    EditorUtility.SetDirty(pipeline);
                }
            }
           advertiser.Folded = EditorGUILayout.Foldout(advertiser.Folded, advertiser.advertiser.ToString(), true, m_EditorFoldOutSyle);
           
          EditorGUILayout.TextField(new GUIContent("", "Current version of advertiser"), advertiser.advertiser.InstalledVersion(), EditorStyles.boldLabel);
          if (GUILayout.Button(new GUIContent("Update", $"Check of {advertiser.advertiser} update"), new GUIStyle(GUI.skin.button) { alignment = TextAnchor.MiddleCenter, fixedWidth = 80 }))
          {
              advertiser.advertiser.CheckUpdate();
                EditorUtility.SetDirty(pipeline);
          }
          if (GUILayout.Button(new GUIContent("-", "Remove this Advertiser"), new GUIStyle(GUI.skin.button) { alignment = TextAnchor.MiddleCenter, fixedWidth = 80 }))
          {
              pipeline.Advertisers.Remove(advertiser);
              advertiser.advertiser.RemovefromRegistry();
              EditorUtility.SetDirty(pipeline);
          }

EditorGUILayout.EndHorizontal();

            if (!advertiser.Folded)
                return;

            advertiser.ID = EditorGUILayout.TextField(new GUIContent("Advertiser ID:", "Unique ID for the advertiser"), advertiser.ID.ToString());

           if (advertiser.advertiser == SupportedAdvertisers.Admob && AdvertiserIDCache != advertiser.ID)// Update AD ID in google admob settings
            {
                AdvertiserIDCache = advertiser.ID;
                advertiser.UpdateAdmobSettings();
                EditorUtility.SetDirty(pipeline);
            }
EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Ad Units", EditorStyles.boldLabel);
            FilteredAdUnitDropdown(advertiser);

            if (GUILayout.Button(new GUIContent("+", "Add selected Ad Unit"), new GUIStyle(GUI.skin.button) { alignment = TextAnchor.MiddleCenter, fixedWidth = 80 }))
            {
                if (!advertiser.Ads.Any(ad => ad.type == SelectedAdType))
                    advertiser.Ads.Add(new AD() { type = SelectedAdType, ADIds = new List<string>() });
                else
                    EditorUtility.DisplayDialog("Warning", $"Ad unit [{SelectedAdType}] already exist in current Advertiser.", "OK");
                EditorUtility.SetDirty(pipeline);
            }
EditorGUILayout.EndHorizontal();
           
            for(int i=0;i<advertiser.Ads.Count;i++)
            {
                DisplayAdUnit(advertiser.Ads[i], advertiser, ref pipeline);
            }
        }
        /// <summary>
        /// Change Proiorty of Advertiser
        /// </summary>
        /// <param name="index1"></param>
        /// <param name="index2"></param>
        void SwapAdvertisers(int index1, int index2, ref IPipeline pipeline)
        {
            Ads.Base.Advertiser temp = pipeline.Advertisers[index1];
            pipeline.Advertisers[index1] = pipeline.Advertisers[index2];
            pipeline.Advertisers[index2] = temp;
        }
        /// <summary>
        /// Add a filter for advertiser to disallow multiple ad unit of same type
        /// </summary>
        /// <param name="advertiser"> Setting for the Advertiser</param>
        private void FilteredAdUnitDropdown(Ads.Base.Advertiser advertiser)
        {
            List<AdType> availableAdTypes = Enum.GetValues(typeof(AdType))
                .Cast<AdType>()
                .Where(adType =>
                    advertiser.advertiser.SupportsAd(adType) &&
                    !advertiser.Ads.Any(ad => ad.type == adType))
                .ToList();
            int selectedIndex = availableAdTypes.IndexOf(SelectedAdType);
            selectedIndex = EditorGUILayout.Popup(new GUIContent("", "Select Ad unit type to add"), selectedIndex, availableAdTypes.Select(adType => adType.ToString()).ToArray());

            if (selectedIndex >= 0 && selectedIndex < availableAdTypes.Count)
            {
                SelectedAdType = availableAdTypes[selectedIndex];
            }
        }
        /// <summary>
        /// Resonsible for painting AD unit on editor
        /// </summary>
        /// <param name="ad"></param>
        /// <param name="advertiser"></param>
        private void DisplayAdUnit(AD ad, Ads.Base.Advertiser advertiser, ref IPipeline pipeline)
        {
            
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.BeginHorizontal();
            ad.Folded = EditorGUILayout.Foldout(ad.Folded, ad.type.ToString(), true, new GUIStyle(EditorStyles.foldout) { fontStyle = FontStyle.Bold });
            if ((ad.type != AdType.Banner && ad.type != AdType.BigBanner))
            {
                ad.LoadAtStart = EditorGUILayout.ToggleLeft(new GUIContent("Load Ad At Start", "Load ad after intialization."), ad.LoadAtStart, GUILayout.Width(120));
                ad.LoadAfterClose = EditorGUILayout.ToggleLeft(new GUIContent("Load AD On Close", "Load ad after showing ad is closed."), ad.LoadAfterClose, GUILayout.Width(130));
            }
            else if (pipeline.Advertisers.Select(a => a.Ads).All(a => a.Where(a => (a.type == AdType.Banner || a.type == AdType.BigBanner) && a.LoadAtStart).Count() == 0))
                ad.LoadAtStart = EditorGUILayout.ToggleLeft(new GUIContent("Show banner at start", "Load ad and show after intialization."), ad.LoadAtStart, GUILayout.Width(150));
            else if (ad.LoadAtStart)
                ad.LoadAtStart = EditorGUILayout.ToggleLeft(new GUIContent("Show banner at start", "Load ad and show after intialization."), ad.LoadAtStart, GUILayout.Width(150));
           
            
            if (GUILayout.Button(new GUIContent("+", "Add new Ad ID"), GUILayout.Width(80)))
            {
                ad.ADIds.Add(string.Empty);
                EditorUtility.SetDirty(pipeline);
            }
            if (GUILayout.Button(new GUIContent("-", "Remove Ad Unit"), GUILayout.Width(80)))
            {
                advertiser.Ads.Remove(ad);
                EditorUtility.SetDirty(pipeline);
            }
            
            EditorGUILayout.EndHorizontal();
            if (ad.Folded)
            {
                DisplayAdIDsField(ad);
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.Space();
        }
        /// <summary>
        /// Paint Ad ids of the AD unit
        /// </summary>
        /// <param name="ad"></param>
        private void DisplayAdIDsField(AD ad)
        {
            GUILayout.BeginVertical();
            
            ad.ADReshowTime = EditorGUILayout.Slider(new GUIContent("Ad Reshown Time", "That ad is not shown until that much time has passed in sec"), ad.ADReshowTime,0,60);
            if (ad.ADReshowTime < 0)
                ad.ADReshowTime = 0;

            for (int i = 0; i < ad.ADIds.Count; i++)
            {
                EditorGUILayout.BeginHorizontal();
                ad.ADIds[i] = EditorGUILayout.TextField(new GUIContent($"ID {i + 1}:", "Enter the ID for this ad unit"), ad.ADIds[i]);
                if (GUILayout.Button(new GUIContent("-", "Remove Ad ID"), GUILayout.Width(80)))
                {
                    ad.ADIds.RemoveAt(i);
                }
                EditorGUILayout.EndHorizontal();
            }
            GUILayout.EndVertical();
        }
    }
}
#endif