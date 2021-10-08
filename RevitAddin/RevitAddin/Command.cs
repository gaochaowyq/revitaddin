#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using RevitAddin.WPF;

#endregion

namespace RevitAddin
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;
            // Filter all loaded Family Symbol



            FilteredElementCollector collector1 = new FilteredElementCollector(doc)

                         .OfClass(typeof(FamilySymbol));
            
            ICollection<Element> collection = collector1.WhereElementIsElementType().ToElements();
            //ImportRhinoFile openfile = new ImportRhinoFile(collection, doc);


            try { 
                using (Transaction tx = new Transaction(doc))
                    {
                        tx.Start("Transaction Name");
                
                        

                        //openfile.Show();
                        

                        Debug.WriteLine("dotast");

                        tx.Commit();
                    }
                return Result.Succeeded;
            }
            catch (Exception exp) 
                {
                Debug.WriteLine("do task bad");
                Debug.WriteLine(exp.Message);

                return Result.Failed;
            }

            
        }
    }
}
