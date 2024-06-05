#if UNITY_EDITOR

namespace SH.Ads.Editor.Mediation.Base
{
    public abstract class MediationUnit
    {
        internal abstract string Name { get; }
        internal abstract BaseType[] MediationType { get; }


        internal abstract void OnEnable(AdSettings adSettings);
        internal abstract void OnGUI();
    }
}
#endif