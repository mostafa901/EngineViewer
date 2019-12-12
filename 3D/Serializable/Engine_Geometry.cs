using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Urho3DNet;

namespace EngineViewer.Serializable
{
    public class Engine_Geometry
    {
        public string Name { get; set; } = "";
        public Engine_Point Position { get; set; }
        public List<Engine_Face> Engine_Faces { get; set; }
        public Engine_Geometry()
        {
            Engine_Faces = new List<Engine_Face>();
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

        public Engine_Face GetFace(int index)
        {
            return Engine_Faces[index];
        }



        public struct Engine_Face
        {
            public int FaceId;

            public Engine_Point V1;
            public Engine_Point V2;
            public Engine_Point V3;

            public Engine_Point N1;
            public Engine_Point N2;
            public Engine_Point N3;


            public Engine_Point Tx1;
            public Engine_Point Tx2;
            public Engine_Point Tx3;

            public Engine_Point Tan1;
            public Engine_Point Tan2;
            public Engine_Point Tan3;
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

            public Engine_Point(float x,float y, float z,float l, PointType pType)
            {
                X = x;
                Y = y;
                Z = z;
                L = l;
                EngPointType = pType;
            }
            public Engine_Point(float[] floatArray, PointType pType)
            {
                X = floatArray[0];
                Y = floatArray[1];
                Z = floatArray[2];
                EngPointType = pType;
            }
            public float[] ToFloatArray()
            {
                float[] pointFloat = null;
                switch (EngPointType)
                {
                    case PointType.Tangent:
                    case PointType.Color:
                        {
                            pointFloat = new float[4];
                            pointFloat[0] = X;
                            pointFloat[1] = Y;
                            pointFloat[2] = Z;
                            pointFloat[3] = L;

                        }
                        break;

                    case PointType.Texture:
                        {
                            pointFloat = new float[2];
                            pointFloat[0] = X;
                            pointFloat[1] = Y;
                        }
                        break;
                    case PointType.Vertex:
                    case PointType.Normal:
                    case PointType.Position:
                        {
                            pointFloat = new float[3];
                            pointFloat[0] = X;
                            pointFloat[1] = Y;
                            pointFloat[2] = Z;
                        }

                        break;
                    default:
                        break;
                }
                return pointFloat;
            }

            public Vector2 ToVec2()
            {
                return new Vector2(X, Y);
            }

            public Vector3 ToVec3()
            {
                return new Vector3(X, Y, Z);
            }
            public Vector4 ToVec4()
            {
                return new Vector4(X, Y, Z, L);
            }

            public override string ToString()
            {
                return $"{X}, {Y}, {Z}";
            }
        }
    }
}
