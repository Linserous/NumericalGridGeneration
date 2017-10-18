using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using MeshRecovery_Lib;
//using System.Web.Script.Serialization;
//using System.Collections.Generic;

namespace MeshRecovery_Visualizer
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            // TODO Liza: change navigation logic
            // This is a temporary navigation
            var assembly = System.Reflection.Assembly.GetEntryAssembly();
            var location = System.IO.Path.GetDirectoryName(assembly.Location);
            WebBrowser.Navigate(System.IO.Path.Combine(location, "../../src/index.html"));
        }

        void WebBrowser_LoadCompleted(object sender, NavigationEventArgs e)
        {
            JS_Init();
            
            //TODO: add file operations to import file and load data using Loader.js
            long[] xadj = { 0, 2, 4, 6, 8 };
            int[] adjncy = { 1, 2, 0, 3, 1, 3, 0, 2 };

            JS_LoadGraph(GraphToJson.Run(xadj, adjncy));
        }

        private void JS_LoadGraph(object graphJson)
        {
            WebBrowser.InvokeScript("loadGraph", new object[] { graphJson });
        }

        private void JS_Init()
        {
            WebBrowser.InvokeScript("init");
        }
    }
}
