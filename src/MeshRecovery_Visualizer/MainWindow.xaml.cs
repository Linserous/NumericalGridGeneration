using System;
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
                Loader.LoadGraphFromMETISFormat(openFileDialog.FileName,out xadj, out adjncy);
                js.LoadGraph(Json.Graph2Json.Run(xadj, adjncy));
            }
        }

        private void Close_OnClick(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            Close();
        }
        #endregion
    }
}
