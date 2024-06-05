using UnityEngine;

namespace SH.Ads.Editor
{

    public interface Tab
    {
        GUIContent Title { get; }
        string IconBase64 { get; }
        Texture2D Texture { get; }

        void OnGUI();
        void OnEnable();
    }
}