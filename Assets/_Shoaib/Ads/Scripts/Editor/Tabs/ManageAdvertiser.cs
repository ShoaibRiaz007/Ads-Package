#if UNITY_EDITOR
using SH.Ads.Editor.Base;
using SH.Ads.Piplines;
using UnityEditor;
using UnityEngine;

namespace SH.Ads.Editor
{
    public class ManageAdvertiser : ITab
    {
        public override GUIContent Title => new GUIContent("Manage Advertiser", Texture(), "Panel to update setting of advertisers");

        protected override string IconBase64 => "iVBORw0KGgoAAAANSUhEUgAAABkAAAAZCAYAAADE6YVjAAADVklEQVRIDaXVaYhNYRjA8WvsjHVsoeyGsW+Zso19PkjSiEiWfJAofJP4oBBJWVLiizUKUZMYDCFGCKEYzRhhssTYdzP+/+mc23XvNZKnfvOe8573vNvz3jORSNVRn8ft0KDqZpFaPE/FIBzCJUxENSSNFGplzEcuFqIGfKk6YqMmN0uwFAdg52nYg7pIGKkOdevwGq4iA1uQg/d4gvZYH1xTRJrhMBYhEwPwGa+wFj/ilzOZylG4iuc4hXK4spHoH1w/pDwIoxV2YTzsrxMa4QZ8NyHc12NoicYYjWkYiyaBDZS9EYaDnAxvkpXh3vusHkagBF/gHpvIn+iLlfiOUvSA7ePDic3AYrQNH4ZJNEFr4HbsRHP0hHW3cR1TUYJCTMACFKEj3Ko7mILaeICZOIPycCUm3Jntw304o5f4BuMrzENrFOMQ7Hwu3M42mIMsOAlzWh8OGD2OduKJciZdkA9nXoYfaIgMHIerGIXN2ITLGArb30I23PY8uLqE8FTsh4nvB/OwHrPhSl3havRBGPGJt43vR8MfWGx05SbcortcyzauRmnwUNixs04Wb+Ir4wd5R4Om2Bo0vEh5BG7PRNSCbU4jjAouUuD+O0lzYx97YeIr/PHEhw0/wRdXwJXJgZ2lW+Hp+Q4jFTtwE27xUVzDcizA5/AIcx0NPwfO9i3uwVzkBtcO7qkrRxhOwNP0GPY3EJ3hJPIQ25bbf4uaNE/HPLSB927ZcOQE9xSJH8jKyr/88fx7uoahNT6gL/Jh+IkxJy9QiPJkOaG+8v9HJmU3uF3mwpPm0ifBZBfAeg+EP+YTwXUTyktwdadwIdkgWTzwFPVAMfzEN8OQgL+lL/gIOzcPyzAd5nEzXO02+CUvc5AsGO7pLJRgA5zln6IGDxzAHJiTcfAdkz8Zg7EKz7DbQUqwEcYtnIfbERuupD8s/UF6bB2oIbrjQnDvh9aP6RVkIx0FDmLlAFQVHXnoKjPQGW6ZX4BSeMwLgrKIcgxykItzeITKQSz/J1J42cHHwhW4jdFwyf8TDXi5J7LgarfDnfltu90ul3MQhr/oshgmP/be5x6QDjC5veCJsuPz8HlCOIgzCEduzLXJbR4wyX6rrGsBc2GnnqKzsOOn8DiHfXD5e/wC52rGmV2tODsAAAAASUVORK5CYII=";


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