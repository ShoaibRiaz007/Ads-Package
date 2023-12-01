#if UNITY_EDITOR
using SH.Ads.Editor.Base;
using SH.Ads.Piplines;
using UnityEditor;
using UnityEngine;

namespace SH.Ads.Editor
{
    public class ManageAdvertiser : IWindow
    {
        public override string Name => "Manage Advertiser";

        public override string ToolTip => "Panel to update setting of advertisers";

        static AdSettings adSettings;
        static IPipelineEditor pipelineEditor;
        public override void OnEnable(AdSettings settings)
        {
            adSettings= settings;
            if (settings.CurrentPipline.GetType() == typeof(Waterfall))
                pipelineEditor = new WaterfallEditor();
            else 
                pipelineEditor = new CustomWaterfallEditor();

            EditorUtility.SetDirty(settings);
        }

        public override void OnGUI()
        {
            Header();
            pipelineEditor.OnGUI(ref adSettings.CurrentPipline);
        }
        void Header()
        {
            EditorGUILayout.HelpBox("This window is used to used manage all active Advertisers. Bellow are the list of Advertisers you have added. You can add new ads and change their properties here", MessageType.Info);
            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            
            EditorGUILayout.LabelField("Current Pipeline", EditorStyles.boldLabel);
            EditorGUILayout.LabelField(adSettings.CurrentPipline.Name, new GUIStyle(EditorStyles.boldLabel) {alignment = TextAnchor.UpperRight });
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();
        }
    }
}
#endif