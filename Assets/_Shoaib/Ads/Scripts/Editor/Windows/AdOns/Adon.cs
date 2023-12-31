#if UNITY_EDITOR
using UnityEditor;

namespace SH.Ads.Editor.Adons
{
    public abstract class Adon
    {
        public abstract string Name { get; }
        protected abstract string Symbol { get; }
        public abstract bool SymbolPresent { get; }
        public abstract void AddSymbol();
        public abstract void RemoveSymbol();
        public abstract string Description { get; }
        public abstract bool IsInstalled { get; }
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