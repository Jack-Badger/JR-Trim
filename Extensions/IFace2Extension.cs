using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

namespace Jack.Extensions
{
    public static class IFace2Extension
    {
        public static bool IsSheetBody(this IFace2 face)
        {
            return (face.GetBody() as Body2).GetType() == (int)swBodyType_e.swSheetBody;
        }

        public static bool IsPlanarFace(this IFace2 face)
        {
            return face.IGetSurface().IsPlane();
        }
    }
}
