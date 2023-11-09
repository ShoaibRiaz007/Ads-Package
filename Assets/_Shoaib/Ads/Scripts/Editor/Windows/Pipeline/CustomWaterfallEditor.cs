#if UNITY_EDITOR
using SH.Ads.Editor.Base;
using SH.Ads.Piplines;
using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace SH.Ads.Editor
{
    public class CustomWaterfallEditor : IPipelineEditor
    {
        static readonly string[] Adtypes = Enum.GetNames(typeof(AdType));
        static readonly string[] Advertisers = Enum.GetNames(typeof(SupportedAdvertisers));

        public override void OnGUI<T>(ref T CurrentPipline)
        {
            if (CurrentPipline is CustomWaterfall customWaterfall)
            {
EditorGUILayout.BeginVertical(EditorStyles.objectField);
EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Custom Waterfall Ad Rules", EditorStyles.boldLabel);

                if (GUILayout.Button(new GUIContent("+", "Add Custom Rules for Ad unit"), GUILayout.Width(80)))
                {
                    customWaterfall.m_AdRules.Add(new CustomWaterfall.CustomAdvertiser() { m_AdType = AdType.Banner, m_Advertiser = SupportedAdvertisers.Admob });
                    EditorUtility.SetDirty(customWaterfall);
                }
EditorGUILayout.EndHorizontal();
                for (int i = customWaterfall.m_AdRules.Count - 1; i >= 0; i--)
                {
EditorGUILayout.BeginHorizontal();
                    customWaterfall.m_AdRules[i].m_AdType = (AdType)EditorGUILayout.Popup(new GUIContent("", "Rule effecting Ad type"), (int)customWaterfall.m_AdRules[i].m_AdType, Adtypes);
                    customWaterfall.m_AdRules[i].m_Advertiser = (SupportedAdvertisers)EditorGUILayout.Popup(new GUIContent("", "Rule effecting Ad Advertiser"), (int)customWaterfall.m_AdRules[i].m_Advertiser, Advertisers);
                    if (GUILayout.Button(new GUIContent("-", "Remove Custom Rules for Ad unit"), GUILayout.Width(80)))
                    {
                        customWaterfall.m_AdRules.RemoveAt(i);
                        EditorUtility.SetDirty(customWaterfall);
                    }
EditorGUILayout.EndHorizontal();
                    if (i >= 0 && i < customWaterfall.m_AdRules.Count && !customWaterfall.Advertisers.Any(a => a.advertiser == customWaterfall.m_AdRules[i].m_Advertiser))
                        EditorGUILayout.LabelField($"<color=red>Warning... You have not added '{customWaterfall.m_AdRules[i].m_Advertiser}' Advertiser in active list</color>", new GUIStyle(EditorStyles.miniLabel) { richText = true });
                }
EditorGUILayout.EndVertical();
                EditorGUILayout.Space(20);
            }
            base.OnGUI(ref CurrentPipline);
            
        }

    }
}
#endif