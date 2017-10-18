using System.Windows.Controls;
using System.Windows.Navigation;

namespace MeshRecovery_Visualizer
{
    public class JSInvoker
    {
        private WebBrowser browser;

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
            browser.InvokeScript("loadGraph", new object[] { graphJson });
        }
    }
 
}
