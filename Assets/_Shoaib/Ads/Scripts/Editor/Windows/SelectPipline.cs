#if UNITY_EDITOR
using SH.Ads.Editor.Base;
using SH.Ads.Piplines;
using UnityEditor;
using UnityEngine;

namespace SH.Ads.Editor
{
    public class SelectPipline : IWindow
    {
        static Waterfall waterPipeline;
        static CustomWaterfall CustomWaterfallPipeline;
        static AdSettings adSettings;
        public override string Name => "Select Pipeline";

        public override string ToolTip => "Panel to select Pipline Active Pipline";

        public override void OnEnable(AdSettings settings)
        {
            adSettings= settings;
            if (waterPipeline == null)
                waterPipeline = Waterfall.Load();
            if (CustomWaterfallPipeline == null)
                CustomWaterfallPipeline = CustomWaterfall.Load();

            EditorUtility.SetDirty(settings);
        }

        public override void OnGUI()
        {
            Header();
            EditorGUILayout.BeginHorizontal();
            Left();
            Right();
            EditorGUILayout.EndHorizontal();
        }
        void Header()
        {
            EditorGUILayout.HelpBox("This window is used to used to select active pipeline. Bellow is the details about every supported pipeline currently.", MessageType.Info);
            EditorGUILayout.Space();
        }
        void Left()
        {
            EditorGUILayout.BeginVertical(new GUIStyle(EditorStyles.helpBox) { stretchHeight = true });

            EditorGUILayout.LabelField(waterPipeline.Name, EditorStyles.boldLabel);
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Description",EditorStyles.boldLabel);
            EditorGUILayout.LabelField(waterPipeline.Description, EditorStyles.textArea);
            EditorGUILayout.Space(10);
            if (adSettings.CurrentPipline.GetType() != typeof(Waterfall))
            {
                if (GUILayout.Button(new GUIContent("Select", "Select As Main pipline"), new GUIStyle(GUI.skin.button) { alignment = TextAnchor.MiddleCenter }))
                {
                    waterPipeline.CopyValues(CustomWaterfallPipeline);
                    adSettings.CurrentPipline = waterPipeline;
                    EditorUtility.SetDirty(waterPipeline);
                    EditorUtility.SetDirty(adSettings);
                }
            }
            else
                EditorGUILayout.LabelField("<color=green>Selected</color>", new GUIStyle(EditorStyles.boldLabel) { richText=true,alignment= TextAnchor.MiddleCenter});

            EditorGUILayout.EndVertical();
        }

        void Right() 
        {
            EditorGUILayout.BeginVertical(new GUIStyle(EditorStyles.helpBox) { stretchHeight=true});

            EditorGUILayout.LabelField(CustomWaterfallPipeline.Name, EditorStyles.boldLabel);
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Description", EditorStyles.boldLabel);
            EditorGUILayout.LabelField(CustomWaterfallPipeline.Description, EditorStyles.textArea);
            EditorGUILayout.Space(10);
            if (adSettings.CurrentPipline.GetType() != typeof(CustomWaterfall))
            {
                if (GUILayout.Button(new GUIContent("Select", "Select As Main pipline"), new GUIStyle(GUI.skin.button) { alignment = TextAnchor.MiddleCenter }))
                {
                    CustomWaterfallPipeline.CopyValues(waterPipeline);
                    adSettings.CurrentPipline = CustomWaterfallPipeline;
                    EditorUtility.SetDirty(CustomWaterfallPipeline);
                    EditorUtility.SetDirty(adSettings);
                }
            }
            else
                EditorGUILayout.LabelField("<color=green>Selected</color>", new GUIStyle(EditorStyles.boldLabel) { richText = true, alignment = TextAnchor.MiddleCenter });

            EditorGUILayout.EndVertical();
        }
    }
}
#endif