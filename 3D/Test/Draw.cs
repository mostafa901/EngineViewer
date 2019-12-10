using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineViewer._3D.Test
{
    class Draw
    {
        public void DrawRectangle()
        {
            var geoList = new List<Serializable.Engine_Geometry>();
            var geo = new EngineViewer.Serializable.Engine_Geometry();
            geo.Position = new Serializable.Engine_Geometry.Engine_Point();

            geo.Engine_Points.Add(new Serializable.Engine_Geometry.Engine_Point()
            {
                EngPointType = Serializable.Engine_Geometry.PointType.Vertex,
                X = 0,
                Y = 0,
                Z = 0
            });
            geo.Engine_Points.Add(new Serializable.Engine_Geometry.Engine_Point()
            {
                EngPointType = Serializable.Engine_Geometry.PointType.Texture,
                X = 0,
                Y = 0,
            });
            geo.Engine_Points.Add(new Serializable.Engine_Geometry.Engine_Point()
            {
                EngPointType = Serializable.Engine_Geometry.PointType.Normal,
                X = 0,
                Y = 1,
                Z = 0
            });
            geo.Engine_Points.Add(new Serializable.Engine_Geometry.Engine_Point()
            {
                EngPointType = Serializable.Engine_Geometry.PointType.Vertex,
                X = 10,
                Y = 0,
                Z = 0
            });
            geo.Engine_Points.Add(new Serializable.Engine_Geometry.Engine_Point()
            {
                EngPointType = Serializable.Engine_Geometry.PointType.Normal,
                X = 0,
                Y = 1,
                Z = 0
            });
            geo.Engine_Points.Add(new Serializable.Engine_Geometry.Engine_Point()
            {
                EngPointType = Serializable.Engine_Geometry.PointType.Texture,
                X = 1,
                Y = 0
            });
            geo.Engine_Points.Add(new Serializable.Engine_Geometry.Engine_Point()
            {
                EngPointType = Serializable.Engine_Geometry.PointType.Vertex,
                X = 10,
                Y = 0,
                Z = 10
            });
            geo.Engine_Points.Add(new Serializable.Engine_Geometry.Engine_Point()
            {
                EngPointType = Serializable.Engine_Geometry.PointType.Normal,
                X = 0,
                Y = 1,
                Z = 0
            });
            geo.Engine_Points.Add(new Serializable.Engine_Geometry.Engine_Point()
            {
                EngPointType = Serializable.Engine_Geometry.PointType.Texture,
                X = 1,
                Y = 1
            });
            geo.Engine_Points.Add(new Serializable.Engine_Geometry.Engine_Point()
            {
                EngPointType = Serializable.Engine_Geometry.PointType.Vertex,
                X = 0,
                Y = 0,
                Z = 10
            });
            geo.Engine_Points.Add(new Serializable.Engine_Geometry.Engine_Point()
            {
                EngPointType = Serializable.Engine_Geometry.PointType.Normal,
                X = 0,
                Y = 1,
                Z = 0
            });
            geo.Engine_Points.Add(new Serializable.Engine_Geometry.Engine_Point()
            {
                EngPointType = Serializable.Engine_Geometry.PointType.Texture,
                X = 0,
                Y = 1
            });
            
         DefaultScene.Instance.CreateCustomShape2(geo);
        }

    }
}
