#if UNITY_EDITOR
using UnityEditor;

namespace SH.Ads.Editor.Base
{
    public class Window : EditorWindow
    {
        protected virtual void OnGUI()
        {
            EditorGUILayout.HelpBox("Developed by Shoaib Riaz. \n Email : riazshoaib17@gmail.com \n It is free to use. For any bug or improvements contact me.", MessageType.Warning);
        }
    }
}
#endif