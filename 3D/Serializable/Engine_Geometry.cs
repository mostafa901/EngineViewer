using System.Collections.Generic;
using System.Linq;
using Urho3DNet;

namespace EngineViewer.Serializable
{
    public class Engine_Geometry
    {
        public string Name { get; set; } = "";
        public Engine_Point Position { get; set; }
        public List<Engine_Face> Engine_Faces { get; set; }

        public bool UseLargeIndex = false;
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

        public float[] ToArray(int faceIndex)
        {
            List<float> floatedPoints = new List<float>();
            var face = Engine_Faces[faceIndex];
            floatedPoints.AddRange(face.V1.ToFloatArray());
            floatedPoints.AddRange(face.N1.ToFloatArray());
            if (face.Tx1 != null) floatedPoints.AddRange(face.Tx1.ToFloatArray());
            if (face.Tan1 != null) floatedPoints.AddRange(face.Tan1.ToFloatArray());

            floatedPoints.AddRange(face.V2.ToFloatArray());
            floatedPoints.AddRange(face.N2.ToFloatArray());
            if (face.Tx2 != null) floatedPoints.AddRange(face.Tx2.ToFloatArray());
            if (face.Tan2 != null) floatedPoints.AddRange(face.Tan2.ToFloatArray());

            floatedPoints.AddRange(face.V3.ToFloatArray());
            floatedPoints.AddRange(face.N3.ToFloatArray());
            if (face.Tx2 != null) floatedPoints.AddRange(face.Tx3.ToFloatArray());
            if (face.Tan2 != null) floatedPoints.AddRange(face.Tan3.ToFloatArray());

            return floatedPoints.ToArray(); ;
        }

        public float[] GetVbArray()
        {
            List<float> floatPoints = new List<float>();
            for (int i = 0; i < Engine_Faces.Count; i++)
            {
                floatPoints.AddRange(ToArray(i));
            }
            return floatPoints.ToArray();
        }

        public short[] GetIndex()
        {
            List<short> indexPoints = new List<short>(0);
            for (short i = 0; i < Engine_Faces.Count * 3; i++)
            {
                indexPoints.Add(i);
            }
        //    indexPoints.Reverse();
            return indexPoints.ToArray();
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

            public Engine_Point(float x, float y, float z, float l, PointType pType)
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

        public uint GetVBSize()
        {
            var face = Engine_Faces[0];
            uint count = 6;
            if (face.Tan1 != null) count += 4;
            if (face.Tx1 != null) count += 2;

            return count *sizeof(float);
        }

        int TangentStart()
        {
            var f = Engine_Faces[0];
            return f.Tx1 == null ? 6 : 8;
        }

        int IndexSize()
        {
            return UseLargeIndex ? sizeof(int) : sizeof(short);
        }
        internal void GenerateTangents()
        {
            var vbPoints = GetVbArray();

            Urho3D.GenerateTangents(vbPoints, GetVBSize(), GetIndex(), IndexSize(), 0, Engine_Faces.Count * 3, TangentStart());

            var ps = vbPoints.ToList();
            var facescount = Engine_Faces.Count;

            for (int i = 0; i < facescount; i++)
            {
                var f = Engine_Faces[i];

                int rangestart = i * 36;
                var range = ps.GetRange(rangestart, 36);

                f.Tan1.X = range[8];
                f.Tan1.Y = range[9];
                f.Tan1.Z = range[10];
                f.Tan1.L = range[11];

                f.Tan2.X = range[8 + 12];
                f.Tan2.Y = range[9 + 12];
                f.Tan2.Z = range[10 + 12];
                f.Tan2.L = range[11 + 12];

                f.Tan3.X = range[8 + 24];
                f.Tan3.Y = range[9 + 24];
                f.Tan3.Z = range[10 + 24];
                f.Tan3.L = range[11 + 24];
            }
        }
    }
}