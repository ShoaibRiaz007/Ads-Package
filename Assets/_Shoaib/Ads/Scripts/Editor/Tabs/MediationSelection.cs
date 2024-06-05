#if UNITY_EDITOR
using SH.Ads.Editor.Base;
using UnityEditor;
using UnityEngine;
namespace SH.Ads.Editor
{
    public class MediationSelection : ITab
    {
        static Vector2 scrollPos = Vector2.zero;
        public override GUIContent Title => new GUIContent("Mediation", Texture(), "Add predifined mediation of other networks");

        protected override string IconBase64 => "iVBORw0KGgoAAAANSUhEUgAAABkAAAAZCAYAAADE6YVjAAADWElEQVRIDZ3TWYiOURjA8bHN2HeyRMOMrRhbxlqyjCFZsjUXyoWICzeKIknuLFmypCiyZLswKETKUtaxRjTDkLKHkAxj+/9f79HnM32Gp37feb/zvmd/Tlpa5aMdn+7FdexGJioVVfiqDTL+8nVV3m/BZpzAMMzATHxDqvjkIMvROP6qFmVHPMHLuM6iBkahFb6gOh7jGMoRohkPflOMj3Hla8tqsJHW4Qw2woFDveU15MNwQP8nvvd729nefsI7+4/CFeXhIty+VRiPxOjHn2fYiafoi8Twe9vZ3n7sz35/hS92oU9c04vyEKwPkcnDWXSIS/+HaM3DYfSOK+xnB6L2YUkTqWiCHHSDB20yLMNUhMN1n0sQ9pvH6NsVlOnoie5wBU1hv+sdpDY87OdxSRGFqVoAB09Mguhlwo/vB2IPXGWIFzzYb20H+Y5X2IZLCFGDB2diNqUK35thi+IyfJvLwxh8d5AyOOosjICDGqbiFbz3T4rwvd+thglhuF3tcRVlDuIstsDDSjzoIv6fRvJKXKHtQvh+GgbDcwhhKl9GuSO6p57LO7xFCDvLhoN3RguMxE3UxyPshunqeYYd4DFaiW1a45WDbIVL/oAjMJMcwHvgIHdxB2PRAfNwG+55PhrCMw0ZyGOUcSaDN3+oyx6CmZiPunBGNriPpXiCKcjCbLiKuXCLPYtMtERVhPCWu5JNcPVppciBjRL3mr9R9Ob3ANyuXnC163EPw+Gqk8N+7M9+SyvqNLlBPSrMGg92Asyik+iBOXASno1nVGEkLrHCD6j0Ul5AJ6yEA7idV7EQ3n7rR8NtMpx8eI4qwnYd5F8e0qPanxnn7BvBBhlxfXLhdnXFUWyD/XiOZt0APDC7nJH5bLqZYc2xF+PwGXVwHK7Ag06ObCrawkRZhnPwPBZgDG4gOhwzIMs/REcUYi0c2JktRjFM65ownKDp6yo2wGTYjkH4LdyG5zBT3sRvvDMe5im4moc4j7HwRqfjFmxbAO/DGjg563agHH8Nz6YEL5GLfegPV/wQE+FK6mMFnMRhTMc/h5fOs/BudMEuOOv98f8llJPhvucjA3+Ey0sVRby0A5NhEmrhMXrApMiC57QKTuQr/ivclgboCe/LRnjzLQsxACnv2w/lfruRovmsKwAAAABJRU5ErkJggg==";


        public Mediation.Base.MediationUnit[] allMediation = new Mediation.Base.MediationUnit[]{
            new Mediation.AdmobMediation(),
        };
        public override void OnEnable(AdSettings settings)
        {
            EditorUtility.SetDirty(settings);
        }
        public override void OnGUI()
        {
            Header();
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, new GUIStyle(EditorStyles.helpBox));
            for(int i = 0; i < allMediation.Length; i++)
            {
                allMediation[i].OnGUI();
            }
            EditorGUILayout.EndScrollView();
        }

        void Header()
        {
            EditorGUILayout.HelpBox("This window is used to show all mediation settings.", MessageType.Info);
            EditorGUILayout.Space();
        }
    }
}
#endif