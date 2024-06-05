#if UNITY_EDITOR
using UnityEditor;

namespace SH.Ads.Editor.Mediation.Base
{
    public abstract class BaseType
    {
        protected bool SymbolPresent { get; set; }
        public bool IsInstalled { get; protected set; }
        public bool IsParentEnabled {  get; protected set; }
        public string ParentName {  get; protected set; }


        public bool FoldOut { get; set; }

        protected abstract string Symbol { get; }
        protected abstract string Description { get; }
     
        public abstract string Name { get; }
        public abstract string PackageName { get; }
       

        public abstract void AddSymbol();
        public abstract void RemoveSymbol();
        public abstract void OnGUI();
        public abstract void CheckIfInstalled();

        protected bool CheckIfSymboIsPresent(string PackageId)
        {
            BuildTargetGroup targetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            string defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(targetGroup);

            return defines.Contains(PackageId);
        }
    }
}
#endif