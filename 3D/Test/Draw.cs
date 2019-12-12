using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static EngineViewer.Serializable.Engine_Geometry;

namespace EngineViewer._3D.Test
{
    class Draw
    {
        public void DrawRectangle()
        {
            var geoList = new List<Serializable.Engine_Geometry>();
            var geo = new EngineViewer.Serializable.Engine_Geometry();
            geo.Position = new Serializable.Engine_Geometry.Engine_Point();

            var face01 = new Engine_Face();
            face01.FaceId = 01;

            face01.V1 = new Engine_Point(-10, 0, -10, 0, PointType.Vertex);
            face01.V2 = new Engine_Point(10, 0, -10, 0, PointType.Vertex);
            face01.V3 = new Engine_Point(10, 0, 10, 0, PointType.Vertex);

            face01.Tx1 = new Engine_Point(0, 0, 0, 0, PointType.Texture);
            face01.Tx2 = new Engine_Point(1, 0, 0, 0, PointType.Texture);
            face01.Tx3 = new Engine_Point(1, 1, 0, 0, PointType.Texture);

            face01.N1 = new Engine_Point(0, 1, 0, 0, PointType.Normal);
            face01.N2 = new Engine_Point(0, 1, 0, 0, PointType.Normal);
            face01.N3 = new Engine_Point(0, 1, 0, 0, PointType.Normal);

            var face02 = new Engine_Face();
            face02.FaceId = 02;

            face02.V1 = new Engine_Point(-10, 0, -10, 0, PointType.Vertex);
            face02.V2 = new Engine_Point(10, 0, 10, 0, PointType.Vertex);
            face02.V3 = new Engine_Point(-10, 0, 10, 0, PointType.Vertex);

            face02.Tx1 = new Engine_Point(0, 0, 0, 0, PointType.Texture);
            face02.Tx2 = new Engine_Point(1, 0, 0, 0, PointType.Texture);
            face02.Tx3 = new Engine_Point(1, 1, 0, 0, PointType.Texture);

            face02.N1 = new Engine_Point(0, 1, 0, 0, PointType.Normal);
            face02.N2 = new Engine_Point(0, 1, 0, 0, PointType.Normal);
            face02.N3 = new Engine_Point(0, 1, 0, 0, PointType.Normal);

            geo.Engine_Faces.Add(face01);
            geo.Engine_Faces.Add(face02);

            DefaultScene.Instance.CreateCustomShape2(geo);
        }

    }
}
