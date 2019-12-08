using CodeStack.SwEx.PMPage.Base;
using CodeStack.SwEx.PMPage.Controls;
using Jack.Extensions;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

namespace Jack.PropertyPage
{
    /// <summary>
    /// Filters for a planar face on a surface body.
    /// </summary>
    public class PlanarSurfaceFaceSelectionCustomFilter : SelectionCustomFilter<IFace2>
    {
        protected override bool Filter(IPropertyManagerPageControlEx selBox, IFace2 selection, swSelectType_e selType, ref string itemText)
        {
            return selection.IsSheetBody() && selection.IsPlanarFace();
        }
    }

    /// <summary>
    /// Filters for a face on a surface body.
    /// </summary>
    public class SurfaceFaceSelectionCustomFilter : SelectionCustomFilter<IFace2>
    {
        protected override bool Filter(IPropertyManagerPageControlEx selBox, IFace2 selection, swSelectType_e selType, ref string itemText)
        {
            return selection.IsSheetBody();
        }
    }
}
