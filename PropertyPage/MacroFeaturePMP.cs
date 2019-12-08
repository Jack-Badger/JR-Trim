using CodeStack.SwEx.MacroFeature;
using CodeStack.SwEx.PMPage;
using CodeStack.SwEx.PMPage.Base;
using Jack.Extensions;
using Jack.PropertyPage;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Jack.Features
{
    public abstract class MacroFeaturePMP<TMacroFeature, TPageData, TFeatureData>
        where TPageData : class, IBodyHider, new()
        where TFeatureData : class, IBodyCreator, new()
        where TMacroFeature : MacroFeatureEx<TFeatureData>
    {
        private IModelDoc2 m_Model;
        protected readonly ISldWorks m_App;
        private IFeature m_swFeature;
        private IMacroFeatureData m_swFeatureData;
        private PropertyManagerPageEx<PMPHandler, TPageData> m_Page;
        protected TPageData PageData { get; set; }
        protected TFeatureData FeatureData { get; set; }
        private List<IBody2> m_PreviewBodies;
        private readonly (int rgb, double[] materialProperties) m_PreviewColor;

        public MacroFeaturePMP(ISldWorks app)
        {
            m_App = app;

            m_Page = new PropertyManagerPageEx<PMPHandler, TPageData>(m_App);
            m_Page.Handler.Closing += OnPageClosing;
            m_Page.Handler.DataChanged += OnDataChanged;
            m_Page.Handler.Closed += OnPageClosed;

            m_PreviewColor = InitPreviewColor(Color.Yellow);
        }

        private (int, double[]) InitPreviewColor(Color color)
        {
            return (ConvertColor(), CreateMaterialPropertiesData());

            int ConvertColor()
            {
                return (color.R << 0) | (color.G << 8) | (color.B << 16);
            }

            double[] CreateMaterialPropertiesData()
            {
                return new double[]{ color.R / 255,
                                     color.G / 255,
                                     color.B / 255,
                                     0.5,    // Ambient
                                     0.5,    // Diffuse
                                     0.5,    // Specular
                                     0.5,    // Shininess
                                     0.5,    // Transparency
                                     0.5     // Emissions
                                   };
            }
        }

        public void Insert(IModelDoc2 model)
        {
            PageData = new TPageData();
            m_Model = model;
            m_Page.Show(PageData);
        }

        public void Edit(IModelDoc2 model, IFeature feature)
        {
            m_Model = model;
            m_swFeature = feature;
            m_swFeatureData = (IMacroFeatureData)feature.GetDefinition();

            try
            {
                if (m_swFeatureData.AccessSelections(model, null))
                {
                    FeatureData = m_swFeatureData.GetParameters<TFeatureData>(feature, model);
                    PageData = GetPageData();
                    m_Page.Show(PageData);
                    PageData.HideBodies(true);
                    UpdatePreviewBodies();
                }
                else
                {
                    throw new Exception("Failed to Edit Feature");
                }
            }
            catch
            {
                PageData.HideBodies(false);
                m_swFeatureData.ReleaseSelectionAccess();
            }
        }

        private void OnPageClosed(swPropertyManagerPageCloseReasons_e reason)
        {
            DestroyPreviewBodies();
            PageData.HideBodies(false);

            switch (reason)
            {
                case swPropertyManagerPageCloseReasons_e.swPropertyManagerPageClose_Okay:

                    FeatureData = GetFeatureData();

                    if (Editing)
                    {
                        m_swFeatureData.SetParameters<TFeatureData>(m_swFeature, m_Model, FeatureData);
                        m_swFeature.ModifyDefinition(m_swFeatureData, m_Model, null);
                    }
                    else //Inserting New Feature
                    {
                        m_swFeature = m_Model.FeatureManager.InsertComFeature<TMacroFeature, TFeatureData>(FeatureData);
                        if (m_swFeature == null)
                        {
                            throw new Exception("Failed to Insert Com Feature");
                        }
                    }

                    break;

                case swPropertyManagerPageCloseReasons_e.swPropertyManagerPageClose_UnknownReason:
                case swPropertyManagerPageCloseReasons_e.swPropertyManagerPageClose_Cancel:
                case swPropertyManagerPageCloseReasons_e.swPropertyManagerPageClose_UserEscape:

                    if (Editing)
                    {
                        m_swFeatureData.ReleaseSelectionAccess();
                    }
                    else //Inserting New Feature
                    {
                        m_Model.ClearSelection2(true);
                    }

                    break;
            }
        }

        private bool Editing
        {
            get => m_swFeature != null;
        }

        private void OnDataChanged()
        {
            
            UpdatePreviewBodies();
        }

        private void OnPageClosing(swPropertyManagerPageCloseReasons_e reason, ClosingArg arg)
        {
            if ( !ValidateData(out string error) )
            {
                arg.Cancel = true;
                arg.ErrorMessage = error;
            }
        }

        private void UpdatePreviewBodies()
        {
            try
            {
                PageData.HideBodies(false);
                DestroyPreviewBodies();
                if(ValidateData(out _ ))
                {
                    CreatePreviewBodies();
                    DisplayPreviewBodies();
                    PageData.HideBodies(true);
                }
            }
            catch
            {
            }
        }

        private void DestroyPreviewBodies()
        {
            if (m_PreviewBodies != null)
            {
                foreach (var body in m_PreviewBodies)
                {
                    body.Hide(m_Model);
                    Marshal.ReleaseComObject(body);       
                }
                m_PreviewBodies = null;
            }
        }

        private void DisplayPreviewBodies()
        {
            if (m_PreviewBodies != null)
            {
                foreach (var body in m_PreviewBodies)
                {
                    body.Display3(m_Model, m_PreviewColor.rgb, (int)swTempBodySelectOptions_e.swTempBodySelectOptionNone);

                    //TODO seems a bit much.. Can I set just the transparency here?
                    body.MaterialPropertyValues2 = m_PreviewColor.materialProperties;
                }
            }
        }

        private void CreatePreviewBodies()
        {
            try
            {
                m_PreviewBodies = GetFeatureData().CreateBodies(m_App, preview: true);
            }
            catch (Exception ex)
            {
                //TODO Not sure where to catch exceptions and what they will be yet
                m_App.SendMsg2User(ex.Message, swMessageBoxIcon_e.swMbWarning, swMessageBoxBtn_e.swMbOk);
            }
        }

        protected abstract bool ValidateData(out string error);
        protected abstract TPageData GetPageData();
        protected abstract TFeatureData GetFeatureData();
    }
}
