#if UNITY_EDITOR
using SH.Ads.Editor.Base;
using SH.Ads.Piplines;
using UnityEditor;
using UnityEngine;

namespace SH.Ads.Editor
{
    public class SelectPipline : ITab
    {
        static Waterfall waterPipeline;
        static CustomWaterfall CustomWaterfallPipeline;
        static AdSettings adSettings;

        public override GUIContent Title => new GUIContent("Select Pipeline", Texture(), "Panel to select Pipline Active Pipline");

        protected override string IconBase64 => "iVBORw0KGgoAAAANSUhEUgAAABkAAAAZCAYAAADE6YVjAAACw0lEQVRIDZ3VW2jOYQDH8c2cijmOjI3GnA+hzLgxF1zIIQlJJu4VFy6QSIl2sxtuhAtKLJKcWqQVGiblkMzFhmxTZs6HbU7f77tn69/b9u5996vPnv/zP7zP//+clpGWlpaFaViPZfiH3hgE09xWdPt3KXf0Qh6mowax+GNFWIf++I4FqEY6WnEW92HjibKQi96TjXG4iVhs5DzewK8w13EaA1GAYjTgNXoUP68Ud7ESvs1J/EAJbqMWdmePYyM7MBwHUIZcDEUh7K4mZKLHsbsmYgw+BZMpjQNvH/+FA1mERBkduejgF4V6nY34ow52fByT9szkwHsTJSdycQLHS0K90hnUD33CiWhxj8oMbIFfcwGJsouLFXB2TcJhmFbfbjHmWYuL66c9zryx7ZUuylmcrwjX7BnH2lTZyBPUW4vLpkj9IcflkXpnhyMjJ19wfCnUG23ET5sdTkSLAVS2w7XiLJMPV+AX4vMhcuIdx758LE7hP2jpxMFw7g6lK/43VuMoHNik45fUwYHtLk5ntxgXbTH2IZpRVJzqjuXg6AUbcTUXRk+G4xWUl5EPv+J9KEsozyA+TtlvyMBabEO3eRzucAq7Q5vKtqKjDNVY8Za/u7EHbrQd8UtSiV3mMz/hZLGbndpOkpfIxBBUwRf7iKepNmKX5eEI1mAYxsHudAKdgo0cwwb0xfFUG7nKQ1txCDfgeC6HDfjW1UjHFTTD369MtZFzPLQTJ/AcbqIj8BWueLvNzEUN3N0LUm3kMw/tRz5y4SY6FU7ZO2iCqYfnHaeaRI04qF63dBO1Gxx4+9+VL3Oxrej4m83RItyCs/ELuoxrZE7wiLIlSb6E/1Gfwa2o1LfrKg7oZuxFLRzIZFLMTePDjdcoHyRqxJW7EfPDA3ZbMpnCTQ1wvPyqskSNcD22RWRROmvcTJNJDjetQiPK8eo/MfmgMkWxNNQAAAAASUVORK5CYII=";

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