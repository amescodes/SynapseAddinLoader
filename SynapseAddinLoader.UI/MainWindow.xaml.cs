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
using System.Windows.Shapes;
using MaterialDesignColors;
using MaterialDesignThemes.Wpf;
using Microsoft.Xaml.Behaviors;

namespace SynapseAddinLoader
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeAssemblyReferences();
            InitializeComponent();
        }

        /// <summary>
        /// Create dummy objects to force the assemblies to be loaded
        /// </summary>
        private void InitializeAssemblyReferences()
        {
            // 
            _ = new Card();
            _ = new Hue("Dummy", Colors.Black, Colors.White);
            _ = Interaction.GetBehaviors(this);
        }
    }
}
