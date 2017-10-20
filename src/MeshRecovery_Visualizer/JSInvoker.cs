using System.Windows.Controls;
using System.Windows.Navigation;
using System;

namespace MeshRecovery_Visualizer
{
    public class JSInvoker
    {
        private WebBrowser browser;

        public enum MessageType
        {
            Info = 0,
            Warning,
            Error
        }
        public JSInvoker(WebBrowser browser)
        {
            this.browser = browser;

            // TODO Liza: change navigation logic
            // This is a temporary navigation
            var assembly = System.Reflection.Assembly.GetEntryAssembly();
            var location = System.IO.Path.GetDirectoryName(assembly.Location);

            browser.Navigate(System.IO.Path.Combine(location, "../../src/index.html"));
        }

        public void Init()
        {
            browser.InvokeScript("init");
        }

        public void LoadGraph(object graphJson)
        {
            browser.InvokeScript("loadGraph", graphJson);
        }

        public void Render(bool isRender)
        {
            browser.InvokeScript("render", isRender);
        }

        public bool IsRendered()
        {
            return Convert.ToBoolean(browser.InvokeScript("isRendered"));
        }

        public void Notify(string message, MessageType type)
        {
            browser.InvokeScript("notify", message, Convert.ToInt32(type));
        }
    }
 
}
