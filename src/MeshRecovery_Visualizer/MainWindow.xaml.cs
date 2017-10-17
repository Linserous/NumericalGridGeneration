using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;

namespace MeshRecovery_Visualizer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
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
    }
}
