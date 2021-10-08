using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;


namespace RevitAddin
{
    class R_FamilyInstance
    {
        public R_FamilyInstance(){}

        public FamilyInstance AddFamilyInstance(Document doc,FamilySymbol familySymbol, XYZ xyz,Autodesk.Revit.DB.Structure.StructuralType structuralType)
        {
            FamilyInstance familyInstance= doc.Create.NewFamilyInstance(xyz, familySymbol, structuralType);
            return familyInstance;



        }


    }
}
