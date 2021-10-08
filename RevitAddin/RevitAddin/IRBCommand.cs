#region Namespaces

using System;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using RevitAddin.WPF;

#endregion

namespace RevitAddin
{
    [Autodesk.Revit.Attributes.Transaction(Autodesk.Revit.Attributes.TransactionMode.Manual)]
    class IRBCommand : IExternalCommand
    {
        private ImportRhinoFile _mMyForm;

        public virtual Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            try
            {
                // If we do not have a dialog yet, create and show it
                if (_mMyForm != null && _mMyForm == null) return Result.Cancelled;
                //EXTERNAL EVENTS WITH ARGUMENTS
                EventHandlerWithStringArg evStr = new EventHandlerWithStringArg();
                EventHandlerWith_ImportRhinoFile evWpf = new EventHandlerWith_ImportRhinoFile();

                // The dialog becomes the owner responsible for disposing the objects given to it.
                _mMyForm = new ImportRhinoFile(commandData.Application, evStr, evWpf);
                _mMyForm.Show();
                return Result.Succeeded;
            }
            catch (Exception ex)
            {
                message = ex.Message;
                return Result.Failed;
            }
        }
    }
}
