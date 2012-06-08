
namespace TestRecorder.Core.Actions
{
    public class ActionThread
    {
        public ActionBase Action { get; set; }

        public void Run()
        {
            Action.Perform();
            Action.Context.ActivePage.Browser.WaitForComplete(30);
        }
    }
}
