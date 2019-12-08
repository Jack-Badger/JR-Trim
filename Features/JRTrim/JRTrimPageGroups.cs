using CodeStack.SwEx.PMPage.Attributes;
using Jack.PropertyPage;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Jack.Features.JRTrim
{
    public abstract class FaceDimGroup
    {
        public virtual List<IFace2> Faces { get; set; }

        [ControlAttribution(swControlBitmapLabelType_e.swBitmapLabel_Thickness1)]
        [Description("Actual Thickness of Surface")]
        [NumberBoxOptions(units: swNumberboxUnitType_e.swNumberBox_Length, minimum: 0.001, maximum: 1.0, inclusive: true, increment: 0.01, fastIncrement: 0.1, slowIncrement: 0.001)]
        public double Thickness { get; set; } = 0.01;
    }

    public class BeamGroup : FaceDimGroup
    {
        [SelectionBox(typeof(SurfaceFaceSelectionCustomFilter), swSelectType_e.swSelFACES, swSelectType_e.swSelSURFACEBODIES)]
        [Description("Surface Representing Beam")]
        [ControlAttribution(swControlBitmapLabelType_e.swBitmapLabel_SelectFaceSurface)]
        [ControlOptions(height:90)]
        public override List<IFace2> Faces { get; set; }
    }

    public class PanelGroup : FaceDimGroup
    {
        [SelectionBox(typeof(PlanarSurfaceFaceSelectionCustomFilter), swSelectType_e.swSelFACES, swSelectType_e.swSelSURFACEBODIES)]
        [Description("Planar Surface Representing Panel")]
        [ControlAttribution(swControlBitmapLabelType_e.swBitmapLabel_SelectFaceSurface)]
        [ControlOptions(height: 90)]
        public override List<IFace2> Faces { get; set; }
    }

    public class FitGroup
    {
        [ControlAttribution(swControlBitmapLabelType_e.swBitmapLabel_LinearDistance)]
        [Description("Offset")]
        [NumberBoxOptions(units: swNumberboxUnitType_e.swNumberBox_Length, minimum: 0.0001, maximum: 0.01, inclusive: true, increment: 0.001, fastIncrement: 0.001, slowIncrement: 0.0001)]
        public double Offset { get; set; } = 0.001;
    }
}

