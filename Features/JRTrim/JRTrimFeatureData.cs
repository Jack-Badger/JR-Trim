using CodeStack.SwEx.MacroFeature.Attributes;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Jack.Features.JRTrim
{
    public class JRTrimFeatureData : IBodyCreator
    {
        [ParameterEditBody]
        public List<IBody2> PanelBodies { get; set; }

        [ParameterDimension(swDimensionType_e.swLinearDimension)]
        public double PanelThickness { get; set; }

        [ParameterDimension(swDimensionType_e.swLinearDimension)]
        public double BeamThickness { get; set; }

        [ParameterSelection]
        public List<IFace2> PanelFaces { get; set; }

        [ParameterSelection]
        public List<IFace2> BeamFaces { get; set; }

        public JRTrimFeatureData(JRTrimPageData pageData)
        {
            PanelBodies = CollectBodiesFromFaces(pageData.Panel.Faces);
            PanelThickness = pageData.Panel.Thickness;
            BeamThickness = pageData.Beam.Thickness;
            PanelFaces = pageData.Panel.Faces;
            BeamFaces = pageData.Beam.Faces;
        }

        private List<IBody2> CollectBodiesFromFaces(List<IFace2> faces)
        {
            var bodies = new HashSet<IBody2>();
            foreach(IFace2 face in faces)
            {
                bodies.Add(face.IGetBody());
            }
            return bodies.ToList();
        }

        public JRTrimFeatureData()
        {
        }

        public List<IBody2> CreateBodies(ISldWorks app, bool preview)

        {
            int errorCount = 0;

            var resBodies = new List<IBody2>();

            var modeler = app.GetModeler() as IModeler;

            List<IBody2> thinPanelBodies = new List<IBody2>();
            foreach(IFace2 face in PanelFaces)
            {
                thinPanelBodies.Add(face.ICreateSheetBody());
            }

            List<IBody2> thickBeamBodies = new List<IBody2>();
            foreach (IFace2 face in BeamFaces)
            {
                thickBeamBodies.Add(modeler.ThickenSheet(face.ICreateSheetBody(), BeamThickness / 2, (int)swThickenDirection_e.swThickenDirection_Both));
            }

            foreach (Body2 thinPanelBody in thinPanelBodies)
            {
                var perforatedPanel = default(Body2);

                if (!preview)
                {
                    perforatedPanel = thinPanelBody.ICopy();
                }

                foreach (Body2 thickBeamBody in thickBeamBodies)
                {
                    if (preview)
                    {
                        try
                        {
                            var thickPanelBody = modeler.ThickenSheet(thinPanelBody, PanelThickness / 2, (int)swThickenDirection_e.swThickenDirection_Both);

                            resBodies.Add(BodyOperation(thickPanelBody, thickBeamBody, swBodyOperationType_e.SWBODYINTERSECT));

                            //TODO Must Ask if I need to do this?
                            Marshal.ReleaseComObject(thickPanelBody);
                            thickPanelBody = null;
                        }

                        catch
                        {
                            errorCount++;
                        }
                    }
                    else
                    {
                        var thisPanelFaces = OffsetBothWays(thinPanelBody, PanelThickness / 2);

                        var thisHoleFaces = IntersectBothWays(thisPanelFaces, thickBeamBody);

                        var thisHoleBody = ThickenMerge(thisHoleFaces, PanelThickness);

                        if (thisHoleBody.GetFaceCount() > 6)
                        {
                            var faces = (thisHoleBody.GetFaces() as object[]).Cast<Face2>().ToList();
                            faces.Sort((x, y) => x.GetArea().CompareTo(y.GetArea()));
                            var smallFaces = faces.Take(thisHoleBody.GetFaceCount() - 6).ToArray();

                            thisHoleBody.DeleteFaces5(smallFaces,
                                                 (int)swHealActionType_e.swHealAction_GrowParent,
                                                 (int)swLoopProcessOption_e.swLoopProcess_Auto,
                                                 DoLocalCheck: true,
                                                 out object outBodies,
                                                 out bool localCheckResult);
                            // disabling checking didn't have any noticable speed improvements
                        }

                        var count = thisHoleBody.GetFaceCount();

                        perforatedPanel = BodyOperation(perforatedPanel, thisHoleBody, swBodyOperationType_e.SWBODYCUT);
                    }

                   
                }
                
                if (!preview)
                {
                    resBodies.Add(perforatedPanel);
                }
            }

            return resBodies;

            (Body2 normal, Body2 opposite) OffsetBothWays(IBody2 midBody, double distance)
            {
                return (midBody.MakeOffset(distance, false), midBody.MakeOffset(distance, true));
            }

            (Body2 normal, Body2 opposite) IntersectBothWays((Body2 normal, Body2 opposite) targets, Body2 tool)
            {
                return (BodyOperation(targets.normal,   tool, swBodyOperationType_e.SWBODYINTERSECT),
                        BodyOperation(targets.opposite, tool, swBodyOperationType_e.SWBODYINTERSECT));
            }

            Body2 ThickenMerge((Body2 normal, Body2 opposite) faces, double distance)
            {
                return BodyOperation(modeler.ThickenSheet(faces.normal,   PanelThickness, (int)swThickenDirection_e.swThickenDirection_Side2),
                                     modeler.ThickenSheet(faces.opposite, PanelThickness, (int)swThickenDirection_e.swThickenDirection_Side1),
                                     swBodyOperationType_e.SWBODYADD);
            }
        }

        /// <summary>
        /// There is no need to make a copy of bodies passed to this method.  Returns the first resulting body only.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="tool"></param>
        /// <param name="operation"></param>
        /// <returns>Return the first body of the result</returns>
        private Body2 BodyOperation(Body2 target, Body2 tool, swBodyOperationType_e operation)
        {
            var intersectBodies = (target.ICopy().Operations2((int)operation, tool.ICopy(), out int intError)) as object[];

            if (intError == 0) // swBodyOperationNoError
            {
                return intersectBodies[0] as Body2;
            }
            else
            {
                if (intError == -1)
                {
                    throw new Exception("swBodyOperationUnknownError");
                }
                else
                {
                    var error = (swBodyOperationError_e)intError;
                    throw new Exception($"{error}");
                }
            }
        }
    }
}
