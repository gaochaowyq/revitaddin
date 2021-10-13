using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Autodesk.Revit.DB;
using RevitAddin.WPF;
using RhinoInside.Revit.Convert.Geometry;
using System.Diagnostics;

namespace RevitAddin
{
    /// <summary>
    /// Create methods here that need to be wrapped in a valid Revit Api context.
    /// Things like transactions modifying Revit Elements, etc.
    /// </summary>
    internal class Methods
    {
        /// <summary>
        /// Method for collecting sheets as an asynchronous operation on another thread.
        /// </summary>
        /// <param name="doc">The Revit Document to collect sheets from.</param>
        /// <returns>A list of collected sheets, once the Task is resolved.</returns>
        private static async Task<List<ViewSheet>> GetSheets(Document doc)
        {
            return await Task.Run(() =>
            {
                Util.LogThreadInfo("Get Sheets Method");
                return new FilteredElementCollector(doc)
                    .OfClass(typeof(ViewSheet))
                    .Select(p => (ViewSheet)p).ToList();
            });
        }

        /// <summary>
        /// Rename all the sheets in the project. This opens a transaction, and it MUST be executed
        /// in a "Valid Revit API Context", otherwise the add-in will crash. Because of this, we must
        /// wrap it in a ExternalEventHandler, as we do in the App.cs file in this template.
        /// </summary>
        /// <param name="ui">An instance of our UI class, which in this template is the main WPF
        /// window of the application.</param>
        /// <param name="doc">The Revit Document to rename sheets in.</param>
        public static void SheetRename(Ui ui, Document doc)
        {
            Util.LogThreadInfo("Sheet Rename Method");

            // get sheets - note that this may be replaced with the Async Task method above,
            // however that will only work if we want to only PULL data from the sheets,
            // and executing a transaction like below from an async collection, will crash the app
            List<ViewSheet> sheets = new FilteredElementCollector(doc)
                .OfClass(typeof(ViewSheet))
                .Select(p => (ViewSheet)p).ToList();

            // report results - push the task off to another thread
            Task.Run(() =>
            {
                Util.LogThreadInfo("Sheet Rename Show Results");

                // report the count
                string message = $"There are {sheets.Count} Sheets in the project";
                ui.Dispatcher.Invoke(() =>
                    ui.TbDebug.Text += "\n" + (DateTime.Now).ToLongTimeString() + "\t" + message);
            });

            // rename all the sheets, but first open a transaction
            using (Transaction t = new Transaction(doc, "Rename Sheets"))
            {
                Util.LogThreadInfo("Sheet Rename Transaction");

                // start a transaction within the valid Revit API context
                t.Start("Rename Sheets");

                // loop over the collection of sheets using LINQ syntax
                foreach (string renameMessage in from sheet in sheets
                                                 let renamed = sheet.LookupParameter("Sheet Name")?.Set("TEST")
                                                 select $"Renamed: {sheet.Title}, Status: {renamed}")
                {
                    ui.Dispatcher.Invoke(() =>
                        ui.TbDebug.Text += "\n" + (DateTime.Now).ToLongTimeString() + "\t" + renameMessage);
                }

                t.Commit();
                t.Dispose();
            }

            // invoke the UI dispatcher to print the results to report completion
            ui.Dispatcher.Invoke(() =>
                ui.TbDebug.Text += "\n" + (DateTime.Now).ToLongTimeString() + "\t" + "SHEETS HAVE BEEN RENAMED");
        }

        /// <summary>
        /// Print the Title of the Revit Document on the main text box of the WPF window of this application.
        /// </summary>
        /// <param name="ui">An instance of our UI class, which in this template is the main WPF
        /// window of the application.</param>
        /// <param name="doc">The Revit Document to print the Title of.</param>
        public static void DocumentInfo(Ui ui, Document doc)
        {
            ui.Dispatcher.Invoke(() => ui.TbDebug.Text += "\n" + (DateTime.Now).ToLongTimeString() + "\t" + doc.Title);
        }

        /// <summary>
        /// Count the walls in the Revit Document, and print the count
        /// on the main text box of the WPF window of this application.
        /// </summary>
        /// <param name="ui">An instance of our UI class, which in this template is the main WPF
        /// window of the application.</param>
        /// <param name="doc">The Revit Document to count the walls of.</param>
        public static void WallInfo(Ui ui, Document doc)
        {
            Task.Run(() =>
            {
                Util.LogThreadInfo("Wall Count Method");

                // get all walls in the document
                ICollection<Wall> walls = new FilteredElementCollector(doc)
                    .OfCategory(BuiltInCategory.OST_Walls).WhereElementIsNotElementType()
                    .Select(p => (Wall)p).ToList();

                // format the message to show the number of walls in the project
                string message = $"There are {walls.Count} Walls in the project";

                // invoke the UI dispatcher to print the results to the UI
                ui.Dispatcher.Invoke(() =>
                    ui.TbDebug.Text += "\n" + (DateTime.Now).ToLongTimeString() + "\t" + message);
            });
        }

        public static void ImportRhinoBlock(ImportRhinoFile ui, Document doc)
        {
            Util.LogThreadInfo("ImportRhinoBlock Method");

            //get rhino file path from ui
            string rhinofile = ui.FilePath.Text;
            //get FamilySymbol from UI
            FamilySymbol familySymbol = ui.familyFilert.SelectedItem as FamilySymbol;
            //get Transform 
            ReadRhino readRhino = new ReadRhino(rhinofile);

            var transforms = readRhino.GetRhinoElementLocations();

            var lengthUnits = UnitUtils.GetValidUnits(new ForgeTypeId("autodesk.spec.aec:length-2.0.0"));

            foreach(var i in lengthUnits)
            {
                Util.LogThreadInfo(i.TypeId);
            }
            // rename all the sheets, but first open a transaction
            using (Transaction t = new Transaction(doc, "ImportRhinoBlock"))
            {
                try { 
                Util.LogThreadInfo("ImportRhinoBlock Transaction");

                // start a transaction within the valid Revit API context
                t.Start("ImportRhinoBlock");

                R_FamilyInstance r_FamilyInstance = new R_FamilyInstance();

                foreach(var i in transforms)
                {                  
                   
                    Transform transform = GeometryEncoder.ToTransform(i);
                        // for move and rotate
                        /*
                        XYZ transloation = new XYZ(0, 0, 0);
                        XYZ axisX = transform.OfVector(transform.BasisX);
                        XYZ axisY = transform.OfVector(transform.BasisY);
                        XYZ axisZ = transform.OfVector(transform.BasisZ);
                        XYZ basisY = new XYZ(transform.BasisY.X,transform.BasisY.Y, transform.BasisY.Z);
                        XYZ basisX = new XYZ(transform.BasisX.X, transform.BasisX.Y, transform.BasisX.Z);
                        // rotate by x axis
                        double rotXRadian = basisY.AngleOnPlaneTo(axisY, axisX);
                        double rotXDegree = rotXRadian * (180 / Math.PI);
                        // rotate by y axis
                        double rotYRadian = basisX.AngleOnPlaneTo(axisX, axisY);
                        double rotYDegree = rotYRadian * (180 / Math.PI);
                        // rotate by z axis
                        double rotZRadian = basisX.AngleOnPlaneTo(axisX, axisZ);
                        double rotZDegree = rotZRadian * (180 / Math.PI);
                        double rotRadian = (location as LocationPoint).Rotation;
                        double rotDegree = rotRadian * (180 / Math.PI);
                        XYZ point = (familyInstance.Location as LocationPoint).Point;
                        Line axisz = Line.CreateBound(point, new XYZ(point.X, point.Y, point.Z + 10));
                        location.Rotate(axisz, rotZDegree);       
                        Line axisy = Line.CreateBound(point, new XYZ(point.X, point.Y + 10, point.Z));
                        location.Rotate(axisy, rotYDegree);
                        Line axisx = Line.CreateBound(point, new XYZ(point.X + 10, point.Y, point.Z));
                        location.Rotate(axisx, rotXDegree);
                        */

                        FamilyInstance newfamily=AdaptiveComponentInstanceUtils.CreateAdaptiveComponentInstance(doc, familySymbol);

                        AdaptiveComponentInstanceUtils.MoveAdaptiveComponentInstance(newfamily, transform, false);



                    string name = i.ToString();
                    Util.LogThreadInfo($"ImportRhinoBlock{name}");
                }

                // loop over the collection of sheets using LINQ syntax              

                t.Commit();
                t.Dispose();
                }
                catch(Exception e) 
                {
                    Util.LogThreadInfo(e.Message);
                    
                }
            }

        }



    }
}
