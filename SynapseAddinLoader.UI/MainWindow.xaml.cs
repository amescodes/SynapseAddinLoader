using System;
using System.Windows;
using System.Windows.Media;

using SynapseAddinLoader.Core;

using Microsoft.Xaml.Behaviors;

using MaterialDesignColors;
using MaterialDesignThemes.Wpf;

namespace SynapseAddinLoader.UI
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            App.StartSynapseClient();

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

        protected override void OnClosed(EventArgs e)
        {
            App.ShutdownSynapseClient();
            base.OnClosed(e);
        }
    }
}
