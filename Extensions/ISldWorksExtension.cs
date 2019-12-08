using SolidWorks.Interop.sldworks;
using SolidWorks.Interop.swconst;

namespace Jack.Extensions
{
    public static class ISldWorksExtension
    {
        public static swMessageBoxResult_e SendMsg2User(this ISldWorks app, string message, swMessageBoxIcon_e icon, swMessageBoxBtn_e buttons)
        {
            return (swMessageBoxResult_e) app.SendMsgToUser2(message, (int)icon, (int)buttons);
        }
    }
}
