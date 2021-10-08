using System;
using System.Collections.Generic;
using Autodesk.Revit.UI;
using System.Windows;
using Autodesk.Revit.DB;
using Rhino;
using System.Diagnostics;

namespace RevitAddin.WPF
{
    /// <summary>
    /// Interaction logic for ImportRhinoFile.xaml
    /// </summary>
    /// 
    public class RevitFamilySymbl_DataAccess
    
    {
        public List<Element> Elements { get; set; }
        public RevitFamilySymbl_DataAccess(ICollection<Element> _Elements)
        {
            Elements = new List<Element>();
            foreach(var i in _Elements) 
            {
                Elements.Add(i);
            }
        }
    
    
    }

    public partial class ImportRhinoFile : Window
    {
        private readonly Document _doc;
        //private readonly UIApplication _uiApp;
        //private readonly Autodesk.Revit.ApplicationServices.Application _app;
        private readonly UIDocument _uiDoc;

        private readonly EventHandlerWithStringArg _mExternalMethodStringArg;
        private readonly EventHandlerWith_ImportRhinoFile _mExternalMethodWpfArg;
        public ImportRhinoFile(UIApplication uiApp, EventHandlerWithStringArg evExternalMethodStringArg,
            EventHandlerWith_ImportRhinoFile eExternalMethodWpfArg)
        {
            _uiDoc = uiApp.ActiveUIDocument;
            _doc = _uiDoc.Document;
            //_app = _doc.Application;
            //_uiApp = _doc.Application;
            FilteredElementCollector collector1 = new FilteredElementCollector(_doc)

                         .OfClass(typeof(FamilySymbol));

            ICollection<Element> collection = collector1.WhereElementIsElementType().ToElements();
            InitializeComponent();
            _mExternalMethodStringArg = evExternalMethodStringArg;
            _mExternalMethodWpfArg = eExternalMethodWpfArg;

            DataContext = new RevitFamilySymbl_DataAccess(collection);
        
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();

            // Set filter for file extension and default file extension 
            //dlg.DefaultExt = ".json";
            //dlg.Filter = "Json Files (*.json)";

            // Display OpenFileDialog by calling ShowDialog method 
            Nullable<bool> result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                FilePath.Text = dlg.FileName;
               
            }
        }

        private void Commit_Click(object sender, RoutedEventArgs e)
        {
            _mExternalMethodWpfArg.Raise(this);

            this.Close();
        }
    }
}
