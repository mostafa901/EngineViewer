using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EngineViewer.Serializable
{
    public class Engine_Geometry
    {
        public string Name { get; set; }
        public Engine_Point Position { get; set; }       
        public List<Engine_Point> Engine_Points { get; set; }
        public Engine_Geometry()
        {
            Engine_Points = new List<Engine_Point>();
            Position = new Engine_Point() { EngPointType = PointType.Position };
        }
         
        public enum PointType
        {
            Color,
            Vertex,
            Normal,
            Texture,
            Position,
            Tangent
        }

        public class Engine_Point
        {
            public float X { get; set; } = 0f;
            public float Y { get; set; } = 0f;
            public float Z { get; set; } = 0f;
            public float L { get; set; } = 0f;
            public PointType EngPointType { get; set; } = PointType.Vertex;
            public int GroupId { get; set; }

            public Engine_Point()
            {

            }

            public Engine_Point(float[] floatArray,PointType pType,int groupId)
            {
                X = floatArray[0];
                Y = floatArray[1];
                Z = floatArray[2];
                EngPointType = pType;
            }
            public override string ToString()
            {
                return $"{X}, {Y}, {Z}";
            }
        }
    }
}
