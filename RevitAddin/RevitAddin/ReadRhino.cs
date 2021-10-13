using System.Collections.Generic;
using Rhino.FileIO;
using Rhino.Geometry;
using System.Diagnostics;
namespace RevitAddin
{
    class ReadRhino
    {
        private string Filename { get; set; }
        public ReadRhino(string _filename) 
        {
            Filename = _filename;
        }

        public File3dm ReadFile() 
        {
            File3dm rhinofile = File3dm.Read(Filename);
            
            return rhinofile;
        }

        public File3dmObject[] GetObjectByLayer(string layername)
        {
            File3dm rhinofile = File3dm.Read(Filename);

            File3dmObject[] objects = rhinofile.Objects.FindByLayer(layername);

            foreach ( var i in objects) 
            {
                Debug.WriteLine(i.Geometry.ObjectType.ToString());
            }

            return objects;
        }

        public Rhino.Geometry.InstanceDefinitionGeometry GetInstanceDefinitionGeometryByName(string name) 
        {

            return null;
        }

        public Transform  GetRhinoElementLocation(InstanceReferenceGeometry instanceReferenceGeometry ) 
        {
            Transform transform = instanceReferenceGeometry.Xform;
            Debug.WriteLine(transform.ToString());
            Debug.WriteLine(transform.IsRigid(0.01));

            return transform;
        }

        public List<Transform> GetRhinoElementLocations()
        {
            var RhinoObjects = GetObjectByLayer("Default");
            List<Transform> transforms = new List<Transform>();
            foreach(var i in RhinoObjects) 
            {
                InstanceReferenceGeometry ig = i.Geometry as InstanceReferenceGeometry;


                if (null != ig)
                {
                    transforms.Add( GetRhinoElementLocation(ig));
                }

            }
            return transforms;

        }

        

    }



}
