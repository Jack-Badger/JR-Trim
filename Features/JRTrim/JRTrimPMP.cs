using SolidWorks.Interop.sldworks;
using System;
using System.Collections.Generic;

namespace Jack.Features.JRTrim
{
    public class JRTrimPMP : MacroFeaturePMP<JRTrimMacroFeature, JRTrimPageData, JRTrimFeatureData>
    {
        public JRTrimPMP(ISldWorks app) : base(app) {}

        protected override JRTrimFeatureData GetFeatureData()
        {
            return new JRTrimFeatureData(PageData);
        }

        protected override JRTrimPageData GetPageData()
        {
            return new JRTrimPageData(FeatureData);
        }

        protected override bool ValidateData(out string error)
        {
            List<string> errorList = new List<string>();

            if ((PageData.Panel.Faces?.Count ?? 0 ) == 0)
            {
                errorList.Add("No Panel Selected");
            }

            if ((PageData.Beam.Faces?.Count ?? 0 ) == 0)
            {
                errorList.Add("No Beam Selected");
            }

            if (errorList.Count > 0)
            {
                error = String.Join(", ", errorList);
                return false;
            }
            else
            {
                error = "";
                return true;
            }
        }
    }
}
