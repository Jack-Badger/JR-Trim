using CodeStack.SwEx.Common.Attributes;
using CodeStack.SwEx.MacroFeature;
using CodeStack.SwEx.MacroFeature.Attributes;
using CodeStack.SwEx.MacroFeature.Base;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Jack.Features.JRTrim
{
    [ComVisible(true)]
    [ProgId("JackBadger.JRTrimMacroFeature")]
    //[Icon(typeof(Resources), nameof(Resources.jrTrim_icon))]
    [Options("JRBeam-Trim")]
    public class JRTrimMacroFeature : MacroFeatureEx<JRTrimFeatureData>
    {
        protected override bool OnEditDefinition(ISldWorks app, IModelDoc2 model, IFeature feature)
        {
            new JRTrimPMP(app).Edit(model, feature);
            return true;
        }

        protected override MacroFeatureRebuildResult OnRebuild(ISldWorks app, IModelDoc2 model, IFeature feature, JRTrimFeatureData parameters)
        {
            try
            {
                var swMFD = feature.GetDefinition() as IMacroFeatureData;

                var resBodies = parameters.CreateBodies(app, preview: false).ToArray();
                
                return MacroFeatureRebuildResult.FromBodies(resBodies, swMFD);
            }
            catch(Exception ex)
            {
                return MacroFeatureRebuildResult.FromStatus(false, ex.Message);
            }
        }
    }
}
