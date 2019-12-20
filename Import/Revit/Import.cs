using EngineViewer.Actions._3D.RbfxUtility;
using Logger=Shared_Utility.Logger.Logger;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Urho3DNet;
using Utility.Constant;

namespace EngineViewer.Import.Revit
{
    class Import
    {
        public Node fromXML(Node rootNode, Serializable.Engine_Geometry geom)
        {
            Logger.Log($"Generating Geometry [{geom.Name}]");

            if (geom.Engine_Faces.Count == 0)
            {
                Logger.Log($"Geometry: [{geom.Name}] has no faces", "", Logger.ErrorType.Warrning);
                return null;
            }

            var geonode = rootNode.CreateChild(geom.Name);

            geonode.Scale(geom.Flip.ToVec3());
            if (geom.Rotation != null)
                geonode.Rotate(new Quaternion(geom.Rotation.ToVec3()));
            float scaleValue = (float)DynConstants.FeettoMeter;
            geonode.Scale(new Vector3(scaleValue, scaleValue, scaleValue));

            geom.GenerateNormals();
            if (!geom.GenerateTangents())
            {
                var failChild = geonode.CreateChild($"{geom.Name} Failed");
                failChild.Position = geom.Position.ToVec3();
                var model = rootNode.Context.Cache.GetResource<Model>("Models/Box.mdl");
                var stcomp = failChild.CreateComponent<StaticModel>();
                stcomp.SetModel(model);
                return null;
            }

            var faceColorGroups = geom.Engine_Faces.GroupBy(o => o.FaceColor.ToString());
            string dir = System.IO.Path.GetDirectoryName(geom.FileName);

            var files = System.IO.Directory.GetFiles(dir).ToList();
            int index = 0;
            foreach (var faceColorGroup in faceColorGroups)
            {
            //todo: not sure which is better, Create one Node with multiple geometries with multi MaterialIds
            //todo: OR Create multiple Nodes with a single geometry with a single MaterialIds
                var facechild = geonode.CreateChild($"{geom.Name}: {index}");
                index++;

                Material mat = null;

                var faceColor = faceColorGroup.ElementAt(0).FaceColor;

                if (faceColor.L != 1)
                {
                    mat = Material_Ext.TransParentMaterial(faceColor.ToColor());
                }
                else
                {
                    mat = Material_Ext.ColoredMaterial(faceColor.ToColor());
                }
                mat.CullMode = geom.GeoCullModel; 

                var cus = facechild.CreateComponent<CustomNodeComponent>();
                cus.OriginalMaterial = mat;

                var cusGeo = facechild.CreateComponent<CustomGeometry>();
                cusGeo.SetMaterial(mat);
                cusGeo.CastShadows = true;

                cusGeo.BeginGeometry(0, PrimitiveType.TriangleList);

                Logger.Log("Begin Geometry");
                List<Vector3> vcs = new List<Vector3>();
                foreach (var face in faceColorGroup)
                {
                    var triangles = face.EngTriangles;
                    var trianglesCount = face.EngTriangles.Count;
                    for (int triIndex = 0; triIndex < trianglesCount; triIndex++)
                    {
                        var triangle = triangles[triIndex];
                        var triPoints = triangle.GetPoints();
                        foreach (var engpoint in triPoints)
                        {
                            cusGeo.DefineVertex(engpoint.EngPosition.ToVec3());
                            cusGeo.DefineNormal(engpoint.EngNormal.ToVec3());
                            cusGeo.DefineTexCoord(engpoint.EngTexture.ToVec2());
                            cusGeo.DefineTangent(engpoint.EngTangent.ToVec4());
                            vcs.Add(engpoint.EngPosition.ToVec3());
                        }
                    }
                }
                cusGeo.Commit();

                Logger.Log("End Geometry");
            }
            return geonode;
        }

    }
}
