namespace SH.Ads.Editor.Base
{
    public abstract class IWindow
    {
        public abstract string Name { get; }
        public abstract void OnGUI();
        public abstract void OnEnable(AdSettings settings);
    }
}