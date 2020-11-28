using MoonSharp.Interpreter;

namespace HowToBeAHelper.Scripting
{
    [MoonSharpUserData]
    public class UI : IUI
    {
        public void NotifySuccess(string message, int duration = 3000)
        {
            MainForm.Instance.SafeInvoke(() =>
            {
                MainForm.Instance.NotifySuccess(message, duration);
            });
        }

        public void NotifyError(string message, int duration = 3000)
        {
            MainForm.Instance.SafeInvoke(() =>
            {
                MainForm.Instance.NotifyError(message, duration);
            });
        }

        public void AlertSuccess(string text, string title = "Juhu!")
        {
            MainForm.Instance.SafeInvoke(() =>
            {
                MainForm.Instance.AlertSuccess(text, title);
            });
        }

        public void AlertError(string text, string title = "Juhu!")
        {
            MainForm.Instance.SafeInvoke(() =>
            {
                MainForm.Instance.AlertError(text, title);
            });
        }
    }
}
