#if UNITY_EDITOR
using SH.Ads.Editor.Adons;
using SH.Ads.Editor.Base;
using UnityEditor;
using UnityEngine;
namespace SH.Ads.Editor
{
    public class DeletePackage : ITab
    {
        static Vector2 scrollPos = Vector2.zero;
        public override GUIContent Title => new GUIContent("Delete Packages", Texture(), "Delete Installed Packages. Only Installed by this window");

        protected override string IconBase64 => "iVBORw0KGgoAAAANSUhEUgAAABkAAAAZCAYAAADE6YVjAAABzklEQVRIDe2VuUoEQRRFHXcM3BJBBQNBBA38AREMxkgQTARNxEzmFwxE/8DYWDBxyxRx0G8QBDERETRwi9z13Jl+zUx19TKZgRfOVNV971VXV3fX5OrS1UJKB9R7Ur/xnuHNEwutxrDn7zRgz0IBWj0pr3gbsA1fnngmSxNrkp8EFPctALss3xZYTG0OLGeJfnMFGkuKKy9WbnCFzEkwXxMMQi9cwB2YeugMwy1cgZ6PpLs+hnUNfDrFVHLS9qTFVF+EULZiM/rpdIHrWzxLq0U8wk1S8gzBNehOSvLElL8K055YxDrC+YDRSCTZUL7qDt00PVhXTRj6fipjU4w3YQQkTahxXoNAyled3sAqycyiBZLm4RrOQR/oImjCyMrxqlS52qqAM9Dd6WWwVarVWH6qsl4kdaKkhP+LJO1OJPantktHhWSHoLXml6Mxv1m/k13q+2A/mOeAdgJ2gnHNTZEKrXCsxkrlq+7ErfM9k4cgyY4QtyZubGed1cfllfwCv9pz/UHloQ30ZcehuM62e1DdMqRqgIw9eAfdflaUr2en+irp/HElbwjmYBw6wbet2CVp9U9wBltwCVpYKN9FLKhDsB2yvIGf5L2A7iaiX74tY26BXJXJAAAAAElFTkSuQmCC";

        InstalledPackages InstalledPackages;

        public Adon[] allAdons = new Adon[]
        {
            new FirebaseAnalytics(),
            new RemoteConfig(),
            new GoogleReview(),
        };
        public override void OnEnable(AdSettings settings)
        {
            EditorUtility.SetDirty(settings);
            for (int i = 0; i < allAdons.Length; i++)
            {
                allAdons[i].CheckIfInstalled();
            }
            InstalledPackages = InstalledPackages.Load();
        }
        public override void OnGUI()
        {
            Header();
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, new GUIStyle(EditorStyles.helpBox));
            for(int i = 0; i < InstalledPackages.Installed.Count; i++)
            {
                InstalledPackages.Installed[i].OnGUI();
            }

            EditorGUILayout.EndScrollView();
        }

        void Header()
        {
            EditorGUILayout.HelpBox("This window is used to delete installed packages", MessageType.Info);
            EditorGUILayout.Space();
        }
    }
}
#endif