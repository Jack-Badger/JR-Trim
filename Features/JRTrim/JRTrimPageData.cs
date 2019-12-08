using CodeStack.SwEx.Common.Attributes;
using CodeStack.SwEx.PMPage.Attributes;
using Jack.PropertyPage;
using SolidWorks.Interop.swconst;

namespace Jack.Features.JRTrim
{
    [PageOptions(swPropertyManagerPageOptions_e.swPropertyManagerOptions_OkayButton
               | swPropertyManagerPageOptions_e.swPropertyManagerOptions_CancelButton 
               | swPropertyManagerPageOptions_e.swPropertyManagerOptions_CanEscapeCancel)]
    [Title("J.R. Trim")]
    //TODO[Icon(typeof(Resources), nameof(Resources.jrTrim_icon))]
    public class JRTrimPageData : IBodyHider
    {
        public MessageGroup Message { get; set; }
        public PanelGroup Panel { get; set; }
        public BeamGroup Beam { get; set; }
        public FitGroup Fit { get; set; }

        public JRTrimPageData()
        {
            Message = new MessageGroup("https://forum.solidworks.com/thread/237287");
            Panel = new PanelGroup();
            Beam = new BeamGroup();
            Fit = new FitGroup();
        }

        public JRTrimPageData(JRTrimFeatureData featureData) : this()
        {
            Panel.Thickness = featureData.PanelThickness;
            Beam.Thickness = featureData.BeamThickness;
            Beam.Faces = featureData.BeamFaces;
            Panel.Faces = featureData.PanelFaces;
        }

        public void HideBodies(bool hide)
        {
            //Panel.Face?.IGetBody().HideBody(hide);
        }
    }
}
