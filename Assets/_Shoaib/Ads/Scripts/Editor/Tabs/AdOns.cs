#if UNITY_EDITOR
using SH.Ads.Editor.Adons;
using SH.Ads.Editor.Base;
using UnityEditor;
using UnityEngine;
namespace SH.Ads.Editor
{
    public class AdOns : ITab
    {
        static Vector2 scrollPos = Vector2.zero;
        public override GUIContent Title => new GUIContent("Adons", Texture(), "Add Extra Feature for extended functionalities");

        protected override string IconBase64 => "iVBORw0KGgoAAAANSUhEUgAAABkAAAAZCAYAAADE6YVjAAACMklEQVRIDd3UP0hVURzA8ddfrUjEpizKoSF0cBCXsByihgjqNQRFkzi1CaEITQZtbUnUUA6tYkV/hgqK4kHQUIQY/UGtbAjFP9E/Suv71XfsevDq4/Emf/Dhnj/3nnPv75xzM5mVEmsK+JBy7jmAFjRiAqP4i5LEekY5ix+YwTQ+YQ9KFnWMNI5HyKINw8hhNUoSrYwyiaNYhQ24iD+oREGx3NuYf1MWBizLl39zNYUFhW+XjN1UDiNMvplyO97DRdcDDOEKQjhhD76GhqWufXT6ll8wknCHsqlykreJdu/xa90UJ7BorI1a3a6f0QXf3nCrOukv9OMUnDDEEQqnkWwLfbPXeBIbv+M5XlqJwr5nUVstdbd2asST+NbrsAsV+IYPGEPRhy+ehLEy23AJW+AkN3EOb1BUhF0UHvaguU1f4SQu4xha4ReWJJx0OzblR3ML38CLRFu+K2MW9uMJXJOnOAjP1ZJRTe8+1MAzVIW7cCNsRAgHOoMp+NtxIlPr36ET8fmj6X9coPgR99GBq/AcnEcyXYeo/4SbIQcPsWtm3fsbkRr36PFg+W9yEA+mC78TybhGxXsewhcwvd14DE+95dRwktdwsY+jGaYmjls0mB77tyKspV/4Dr1IDScZQH3qHXMdXVxc7EFchxvEZ4fhn8H1mg93SBwucAMq445E3fyb1hq4BnvhuuyAX9KD1LhNjw8XI3yZKVwQ8VZrojeL+JAueGiRiptgCG53U7hC4x+zL4mbp8gM/wAAAABJRU5ErkJggg==";

        public Adon[] allAdons = new Adon[]
        {
            new FirebaseAnalytics(),
            new RemoteConfig(),
            new GoogleReview(),
            new InAppUpdate(),
        };
        public override void OnEnable(AdSettings settings)
        {
            EditorUtility.SetDirty(settings);
            for (int i = 0; i < allAdons.Length; i++)
            {
                allAdons[i].CheckIfInstalled();
            }
        }
        public override void OnGUI()
        {
            Header();
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, new GUIStyle(EditorStyles.helpBox));
            for(int i = 0; i < allAdons.Length; i++)
            {
                allAdons[i].OnGUI();
            }
            EditorGUILayout.EndScrollView();
        }

        void Header()
        {
            EditorGUILayout.HelpBox("This window is used to show all adons that can be installed", MessageType.Info);
            EditorGUILayout.Space();
        }
    }
}
#endif