using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Win32;

namespace MeshRecovery_Visualizer
{
    public partial class MainWindow : Window
    {
        private JSInvoker js;
        public MainWindow()
        {
            InitializeComponent();
            js = new JSInvoker(WebBrowser);
        }

        #region WebBrowser handling
        void WebBrowser_LoadCompleted(object sender, NavigationEventArgs e)
        {
            js.Init();
        }
        #endregion

        #region Menu Handling
        private void OpenFile_OnClick(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true && WebBrowser.IsLoaded)
            {
                long[] xadj;
                int[] adjncy;
                Loader.LoadGraphFromMETISFormat(openFileDialog.FileName, out xadj, out adjncy);

                string json;
                var errorCode = Json.Graph2Json.Run(out json, xadj, adjncy);

                if (errorCode == Json.Graph2Json.ErrorCode.ThresholdExcess)
                {
                    js.Notify("The graph is very large for rendering.", JSInvoker.MessageType.Warning);
                }
                else
                {
                    js.LoadGraph(json);
                }
            }
        }

        private void Close_OnClick(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            Close();
        }
        #endregion

        #region  Buttons handlers
        private void StartRender_OnClick(object sender, RoutedEventArgs e)
        {
            js.Render(true);
        }
        private void StopRender_OnClick(object sender, RoutedEventArgs e)
        {
            js.Render(false);
        }
        #endregion

    }
}
