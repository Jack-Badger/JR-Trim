using CodeStack.SwEx.AddIn;
using CodeStack.SwEx.AddIn.Attributes;
using CodeStack.SwEx.AddIn.Enums;
using CodeStack.SwEx.Common.Attributes;
using Jack.Features.JRTrim;
using Jack.Extensions;
using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;
using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Jack
{
    [ComVisible(true), Guid("6A806C14-F5FB-411B-9B63-9AC080F656AC")]
#if DEBUG
    [AutoRegister("JRTrim", "Experimantal SolidWorksFeatures")]
#endif
    public class JRTrimAddIn : SwAddInEx
    {
        #region Commands
        [Title("JR Trim")]
        [Description("Experimental SW Features")]
        //[Icon(typeof(Resources), nameof(Resources.jrTrim_icon))]
        private enum Commands_e
        {
            [Title("JR Trim")]
            [Description("https://forum.solidworks.com/thread/237287")]
            //[Icon(typeof(Resources), nameof(Resources.jrTrim_icon))]
            [CommandItemInfo(hasMenu: true, hasToolbar: true, suppWorkspaces: swWorkspaceTypes_e.Part, showInCmdTabBox: false)]
            JRTrim
        }
        #endregion

        public override bool OnConnect()
        {
            AddCommandGroup<Commands_e>(OnCommandActivate, OnCommandEnable);
            return true;
        }

        public override bool OnDisconnect()
        {
            return base.OnDisconnect();
        }

        private void OnCommandActivate(Commands_e cmd)
        {
            try
            {
                switch (cmd)
                {
                    case Commands_e.JRTrim:
                        new JRTrimPMP(App).Insert(App.IActiveDoc2);
                        break;
                }
            }
            catch(Exception ex)
            {
                App.SendMsg2User(ex.Message, swMessageBoxIcon_e.swMbStop, swMessageBoxBtn_e.swMbOk);
            }
        }

        private void OnCommandEnable(Commands_e cmd, ref CommandItemEnableState_e state)
        {
            var model = App.ActiveDoc;
            if (model is PartDoc)
            {
                state = CommandItemEnableState_e.DeselectEnable;
            }

        }
    }
}