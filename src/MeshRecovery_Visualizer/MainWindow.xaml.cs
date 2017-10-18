using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Web.Script.Serialization;
using System.Collections.Generic;

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
            //test graph drawing with simplw graph
           var nodes = new List<Json.Node>();
            for (var i = 0; i < 5; ++i)
            {
                nodes.Add(new Json.Node(Convert.ToString(i + 1), Convert.ToString(i + 1), 10.0, 30 + i * 2, 30 - i * 2));
            }
            var edges = new List<Json.Edge>();
            for (var i = 0; i < 4; ++i)
            {
                edges.Add(new Json.Edge(Convert.ToString(i + 1), Convert.ToString(i + 1), Convert.ToString(i + 2)));
            }
            var graph = new Json.Graph(nodes, edges);
            var result = new JavaScriptSerializer().Serialize(graph);
            LoadGraph(result);
        }

        private void LoadGraph(object graphJson)
        {
            WebBrowser.InvokeScript("loadGraph", new object[] { graphJson });
        }
    }
}
