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

            face01.V3 = new Engine_Point(-10, 0, -10, 0, PointType.Vertex);
            face01.V2 = new Engine_Point(10, 0, -10, 0, PointType.Vertex);
            face01.V1 = new Engine_Point(10, 0, 10, 0, PointType.Vertex);

            face01.Tx1 = new Engine_Point(0, 0, 0, 0, PointType.Texture);
            face01.Tx2 = new Engine_Point(1, 0, 0, 0, PointType.Texture);
            face01.Tx3 = new Engine_Point(1, 1, 0, 0, PointType.Texture);

            face01.N3 = new Engine_Point(0, 1, 0, 0, PointType.Normal);
            face01.N2 = new Engine_Point(0, 1, 0, 0, PointType.Normal);
            face01.N1 = new Engine_Point(0, 1, 0, 0, PointType.Normal);

            face01.Tan1 = new Engine_Point(0, 0, 0, 0, PointType.Tangent);
            face01.Tan2 = new Engine_Point(0, 0, 0, 0, PointType.Tangent);
            face01.Tan3 = new Engine_Point(0, 0, 0, 0, PointType.Tangent);

            var face02 = new Engine_Face();

            face02.V1 = new Engine_Point(10, 0, 10, 0, PointType.Vertex);
            face02.V2 = new Engine_Point(-10, 0, -10, 0, PointType.Vertex);
            face02.V3 = new Engine_Point(-10, 0, 10, 0, PointType.Vertex);

            face02.Tx1 = new Engine_Point(0, 0, 0, 0, PointType.Texture);
            face02.Tx2 = new Engine_Point(1, 1, 0, 0, PointType.Texture);
            face02.Tx3 = new Engine_Point(0, 1, 0, 0, PointType.Texture);

            face02.N1 = new Engine_Point(0, 1, 0, 0, PointType.Normal);
            face02.N2 = new Engine_Point(0, 1, 0, 0, PointType.Normal);
            face02.N3 = new Engine_Point(0, 1, 0, 0, PointType.Normal);

            face02.Tan1 = new Engine_Point(0, 0, 0, 0, PointType.Tangent);
            face02.Tan2 = new Engine_Point(0, 0, 0, 0, PointType.Tangent);
            face02.Tan3 = new Engine_Point(0, 0, 0, 0, PointType.Tangent);

            var geo = new EngineViewer.Serializable.Engine_Geometry();
            geo.Engine_Faces.Add(face02);
            geo.Engine_Faces.Add(face01);

            DefaultScene.Instance.CreateCustomShape2(geo);
        }
    }
}