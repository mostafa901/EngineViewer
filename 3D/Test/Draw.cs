using EngineViewer.Actions._3D.RbfxUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Urho3DNet;
using static EngineViewer.Serializable.Engine_Geometry;

namespace EngineViewer._3D.Test
{
    internal class Draw
    {
        public void DrawLine(Engine_Point position, Engine_Point vector, Color color, Node Parent)
        {
            var linechild = Parent.CreateChild("Line");
            var cusLine = linechild.CreateComponent<CustomGeometry>();
            cusLine.BeginGeometry(0, PrimitiveType.LineList);
            var mat = Material_Ext.noLitFromColor(color, true);
            // mat.FillMode = FillMode.FillWireframe;

            cusLine.SetMaterial(mat);
            cusLine.DefineVertex(vector.ToVec3());
            cusLine.DefineColor(color);
            cusLine.Commit();
            linechild.Position = position.ToVec3();
        }

        public Node DrawRectangle()
        {
            var face01 = new Engine_Face();

            var p01 = new Engine_ModelPoint()
            {
                EngPosition = new Engine_Point(10, 0, 10, 0, PointType.Vertex),
                EngNormal = new Engine_Point(0, 1, 0, 0, PointType.Normal),
                EngTexture = new Engine_Point(0, 0, 0, 0, PointType.Texture),
                EngTangent = new Engine_Point(0, 0, 0, 0, PointType.Tangent)
            };

            var p02 = (new Engine_ModelPoint()
            {
                EngPosition = new Engine_Point(10, 0, -10, 0, PointType.Vertex),
                EngNormal = new Engine_Point(0, 1, 0, 0, PointType.Normal),
                EngTexture = new Engine_Point(1, 0, 0, 0, PointType.Texture),
                EngTangent = new Engine_Point(0, 0, 0, 0, PointType.Tangent)
            });

            var p03 = (new Engine_ModelPoint()
            {
                EngPosition = new Engine_Point(-10, 0, -10, 0, PointType.Vertex),
                EngNormal = new Engine_Point(0, 1, 0, 0, PointType.Normal),
                EngTexture = new Engine_Point(1, 1, 0, 0, PointType.Texture),
                EngTangent = new Engine_Point(0, 0, 0, 0, PointType.Tangent)
            });

            var p04 = (new Engine_ModelPoint()
            {
                EngPosition = new Engine_Point(-10, 0, 10, 0, PointType.Vertex),
                EngNormal = new Engine_Point(0, 1, 0, 0, PointType.Normal),
                EngTexture = new Engine_Point(0, 1, 0, 0, PointType.Texture),
                EngTangent = new Engine_Point(0, 0, 0, 0, PointType.Tangent)
            });

            Engine_Triangle tri01 = new Engine_Triangle() { V1 = p01, V2 = p02, V3 = p03 };
            Engine_Triangle tri02 = new Engine_Triangle() { V1 = p03.Clone(), V2 = p04, V3 = p01.Clone() };
            face01.EngTriangles = new List<Engine_Triangle>() { tri01, tri02 };

            var geo = new EngineViewer.Serializable.Engine_Geometry();
            geo.Rotation = new Engine_Point(-90, 0, 0, 0, PointType.Rotation);          
            geo.Engine_Faces.Add(face01);

           return DefaultScene.Instance.CreateCustomShape2(geo);
            
        }
    }
}