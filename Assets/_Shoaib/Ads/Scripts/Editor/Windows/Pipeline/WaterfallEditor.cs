#if UNITY_EDITOR
using SH.Ads.Editor.Base;

namespace SH.Ads.Editor
{
    public class WaterfallEditor : IPipelineEditor
    {
        public override void OnGUI<T>(ref T CurrentPipline)
        {
            base.OnGUI(ref CurrentPipline);
        }
    }
}
#endif