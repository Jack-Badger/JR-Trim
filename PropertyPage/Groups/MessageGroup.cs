using CodeStack.SwEx.PMPage.Attributes;

namespace Jack.PropertyPage
{
    public class MessageGroup
    {
        //TODO This isn't displaying as I'd like
        [ControlOptions(backgroundColor: System.Drawing.KnownColor.Yellow, height: 30)]
        public string Message { get; set; }

        public MessageGroup() {}

        public MessageGroup(string msg)
        {
            Message = msg;
        }
    }
}
