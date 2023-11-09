using SH.Ads.Editor.Base;
using SH.Ads.Piplines;

namespace SH.Ads.Editor
{
    public class ManageAdvertiser : IWindow
    {
        public override string Name => "Manage Advertiser";
        static AdSettings adSettings;
        static IPipelineEditor pipelineEditor;
        public override void OnEnable(AdSettings settings)
        {
            adSettings= settings;
            if (settings.CurrentPipline.GetType() == typeof(Waterfall))
                pipelineEditor = new WaterfallEditor();
            else 
                pipelineEditor = new CustomWaterfallEditor();
        }

        public override void OnGUI()
        {
            pipelineEditor.OnGUI(ref adSettings.CurrentPipline);
        }
    }
}