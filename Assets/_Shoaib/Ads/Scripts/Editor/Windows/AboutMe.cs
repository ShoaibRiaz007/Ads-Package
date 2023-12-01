#if UNITY_EDITOR
using SH.Ads.Editor.Base;
using UnityEditor;
using UnityEngine;
namespace SH.Ads.Editor
{
    public class AboutMe : IWindow
    {
        const string DevName = "M. Shoaib Riaz";
        const string DevEmail = "riazshoaib17@gmail.com";
        const string Summary = "I am an accomplished game developer with a strong background in Unity and C#." +
            "\n Over the past four years, I have been actively involved in optimizing games and implementing cutting-edge features," +
            " specializing in multiplayer, AR, and VR experiences." +
            "\n\n My passion for staying up-to-date with industry trends and my dedication" +
            "\n to delivering high-quality work have contributed to the success of various projects. ";
        public override string Name => "About Me";

        public override string ToolTip => "About the developer";

        public override void OnEnable(AdSettings settings)
        {
            EditorUtility.SetDirty(settings);
        }

        public override void OnGUI()
        {
            Header();

            EditorGUILayout.BeginVertical();

            if (GUILayout.Button("<color=blue>Check For Update</color>", new GUIStyle(EditorStyles.label) { alignment = TextAnchor.LowerRight, fixedHeight = 30, richText = true }))
            {
                Application.OpenURL("https://github.com/ShoaibRiaz007/Ads-Package");
            }

            EditorGUILayout.BeginHorizontal();


            EditorGUILayout.LabelField("Developer Name ", new GUIStyle(EditorStyles.boldLabel) { margin = new RectOffset(5, 5, 5, 5), fixedHeight = 30, alignment = TextAnchor.MiddleLeft });
            EditorGUILayout.LabelField(DevName, new GUIStyle(EditorStyles.boldLabel) { margin = new RectOffset(5, 5, 5, 5), fixedHeight = 30, alignment = TextAnchor.MiddleRight });

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(20);

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("Developer Email ", new GUIStyle(EditorStyles.boldLabel) { margin = new RectOffset(5, 5, 5, 5), fixedHeight = 30, alignment = TextAnchor.MiddleLeft });
            EditorGUILayout.LabelField(DevEmail, new GUIStyle(EditorStyles.boldLabel) { margin = new RectOffset(5, 5, 5, 5), fixedHeight = 30, alignment = TextAnchor.MiddleRight });

            EditorGUILayout.EndHorizontal();

            if (GUILayout.Button("<color=blue>Check My Linkdin</color>", new GUIStyle(EditorStyles.label) {alignment = TextAnchor.MiddleRight,fixedHeight=30 ,richText=true}))
            {
                Application.OpenURL("https://www.linkedin.com/in/shoaib-riaz-game-dev/  ");
            }
            EditorGUILayout.Space(20);
            EditorGUILayout.LabelField("Summary ", new GUIStyle(EditorStyles.boldLabel) { margin = new RectOffset(5, 5, 5, 5), fixedHeight = 30, alignment = TextAnchor.MiddleLeft });
            EditorGUILayout.LabelField(Summary, new GUIStyle(EditorStyles.textArea) { margin = new RectOffset(5, 5, 5, 5), alignment = TextAnchor.MiddleLeft });
EditorGUILayout.EndVertical();
        }

        void Header()
        {
            EditorGUILayout.HelpBox("This window is used to show all about the developer. Thats me :D", MessageType.Info);
            EditorGUILayout.Space();
        }

    }
}
#endif