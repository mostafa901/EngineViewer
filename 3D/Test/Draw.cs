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
            var mat = Material_Ext.SetMaterialFromColor(color, true);
            // mat.FillMode = FillMode.FillWireframe;

            cusLine.SetMaterial(mat);
            cusLine.DefineVertex(vector.ToVec3());
            cusLine.DefineColor(color);
            cusLine.Commit();
            linechild.Position = position.ToVec3();
        }

        public void DrawRectangle()
        {
            var face01 = new Engine_Face();

            var p01 = new Engine_ModelPoint()
            {
                Position = new Engine_Point(10, 0, 10, 0, PointType.Vertex),
                TextureCoor = new Engine_Point(0, 0, 0, 0, PointType.Texture),
                TangentCoor = new Engine_Point(0, 0, 0, 0, PointType.Tangent)
            };

            var p02 = (new Engine_ModelPoint()
            {
                Position = new Engine_Point(10, 0, -10, 0, PointType.Vertex),
                TextureCoor = new Engine_Point(1, 0, 0, 0, PointType.Texture),
                TangentCoor = new Engine_Point(0, 0, 0, 0, PointType.Tangent)
            });

            var p03 = (new Engine_ModelPoint()
            {
                Position = new Engine_Point(-10, 0, -10, 0, PointType.Vertex),
                TextureCoor = new Engine_Point(1, 1, 0, 0, PointType.Texture),
                TangentCoor = new Engine_Point(0, 0, 0, 0, PointType.Tangent)
            });

            var p04 = (new Engine_ModelPoint()
            {
                Position = new Engine_Point(-10, 0, 10, 0, PointType.Vertex),
                TextureCoor = new Engine_Point(0, 1, 0, 0, PointType.Texture),
                TangentCoor = new Engine_Point(0, 0, 0, 0, PointType.Tangent)
            });

            var triNormal01 = new Engine_Point(0, 1, 0, 0, PointType.Normal);
            var triNormal02 = new Engine_Point(0, 1, 0, 0, PointType.Normal);
            Engine_Triangle tri01 = new Engine_Triangle() { V1 = 0, V2 = 1, V3 = 2, Normal = triNormal01 };
            Engine_Triangle tri02 = new Engine_Triangle() { V1 = 0, V2 = 2, V3 = 3, Normal = triNormal02 };
            face01.TriangleIndecees = new List<Engine_Triangle>() { tri01, tri02 };

            var geo = new EngineViewer.Serializable.Engine_Geometry();
            geo.Engine_Faces.Add(face01);
            geo.ModelPoints = new List<Engine_ModelPoint>();
            geo.ModelPoints.Add(p01);
            geo.ModelPoints.Add(p02);
            geo.ModelPoints.Add(p03);
            geo.ModelPoints.Add(p04);

            DefaultScene.Instance.CreateCustomShape2(geo);
        }
    }
}