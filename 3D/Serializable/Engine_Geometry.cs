using EngineViewer._3D.Extention;
using System;
using System.Collections.Generic;
using System.Linq;
using Urho3DNet;
using Logger = Shared_Utility.Logger.Logger;

namespace EngineViewer.Serializable
{
    public class Engine_Geometry
    {
        public string Name { get; set; } = "";
        public Engine_Point Position { get; set; }
        public List<Engine_ModelPoint> ModelPoints { get; set; }
        public List<Engine_Face> Engine_Faces { get; set; }
        public Engine_Point Rotation { get; set; }
        public Engine_Point Minimum { get; set; }
        public Engine_Point Maximum { get; set; }
        public Engine_Point Color { get; set; }
        public bool UseLargeIndex = false;

        public Engine_Geometry()
        {
            Engine_Faces = new List<Engine_Face>();
            ModelPoints = new List<Engine_ModelPoint>();
            Position = new Engine_Point() { EngPointType = PointType.Position };
            Rotation = new Engine_Point() { EngPointType = PointType.Rotation };

        }

        public enum PointType
        {
            Color,
            Vertex,
            Normal,
            Texture,
            Position,
            Tangent,
            Rotation
        }

        public Engine_Face GetFace(int index)
        {
            return Engine_Faces[index];
        }

        public void Scale(float value)
        {
            foreach (var facePoint in ModelPoints)
            {
                facePoint.Position.Scale(value);
                facePoint.TextureCoor.Scale(value);
            }
        }

        public struct Engine_ModelPoint
        {
            public Engine_Point Position;
            public Engine_Point TextureCoor;
            public Engine_Point TangentCoor;
        }

        public struct Engine_Triangle
        {
            public Engine_Point Normal;
            public int V1;
            public int V2;
            public int V3;

            internal int[] GetIndecees()
            {
                return new int[] { V1, V2, V3 };
            }
        }

        public struct Engine_Face
        {
            public string FaceId;
            public List<Engine_Triangle> TriangleIndecees;

            public int[] GetIndecees()
            {
                List<int> indecees = new List<int>();
                foreach (var Tri in TriangleIndecees)
                {
                    indecees.AddRange(Tri.GetIndecees());
                }
                return indecees.ToArray();
            }
        }

        public float[] GetFaceFloatPoints(int faceIndex)
        {
            List<float> floatedPoints = new List<float>();
            var face = Engine_Faces[faceIndex];
            var triangleCount = face.TriangleIndecees.Count();
            for (int i = 0; i < triangleCount; i++)
            {
                var triangle = face.TriangleIndecees[i];
                for (int vindex = 0; vindex < 3; vindex++)
                {
                    floatedPoints.AddRange(ModelPoints[vindex].Position.ToFloatArray());
                    floatedPoints.AddRange(triangle.Normal.ToFloatArray());
                    if (ModelPoints[vindex].TextureCoor != null) floatedPoints.AddRange(ModelPoints[vindex].TextureCoor.ToFloatArray());
                    if (ModelPoints[vindex].TangentCoor != null) floatedPoints.AddRange(ModelPoints[vindex].TangentCoor.ToFloatArray());
                }
            }

            return floatedPoints.ToArray(); ;
        }

        public float[] GetVbArray()
        {
            List<float> floatPoints = new List<float>();
            for (int i = 0; i < Engine_Faces.Count; i++)
            {
                floatPoints.AddRange(GetFaceFloatPoints(i));
            }
            return floatPoints.ToArray();
        }

        public Color GetColor()
        {
            if (Color == null) Color = new Engine_Point(.28f, .28f, .28f, 1, PointType.Color);
            return new Color(Color.X, Color.Y, Color.Z, Color.L);
        }

        public short[] GetShortIndex()
        {
            var inds = Engine_Faces.SelectMany(o => o.GetIndecees());
            List<short> indpoints = new List<short>();
            int i = 0;
            foreach (var ind in inds)
            {
                indpoints.Add((short)i);
                i++;
            }
            return indpoints.ToArray();
        }

        public int[] GetLongIndexData()
        {
            var inds = Engine_Faces.SelectMany(o => o.GetIndecees());
            List<int> indpoints = new List<int>();
            int i = 0;
            foreach (var ind in inds)
            {
                indpoints.Add(i);
                i++;
            }
            return indpoints.ToArray();
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

            public void Scale(float value)
            {
                X *= value;
                Y *= value;
                Z *= value;
                L *= value;
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

        public int GetVBSize()
        {
            Logger.Log("Getting VBSize");
 
            int count = 6;
            if (ModelPoints[0].TangentCoor != null) count += 4;
            if (ModelPoints[0].TextureCoor != null) count += 2;

            return count * sizeof(float);
        }

        private int TangentStart()
        {
            Logger.Log("Getting TangentStart");
            var f = Engine_Faces[0];
            return ModelPoints[0].TextureCoor == null ? 6 : 8;
        }

        private int IndexSize()
        {
            Logger.Log("Getting IndexSize");
            return UseLargeIndex ? sizeof(int) : sizeof(short);
        }

        private void GenerateNormals()
        {
            for (int i = 0; i < Engine_Faces.Count; i++)
            {
                var f = Engine_Faces[i];

                for (int triangleIndex = 0; triangleIndex < f.TriangleIndecees.Count(); triangleIndex++)
                {
                    var triangle = f.TriangleIndecees[triangleIndex];

                    var v1 = ModelPoints[triangle.V1];
                    var v2 = ModelPoints[triangle.V2];
                    var v3 = ModelPoints[triangle.V3];
                    var dv1 = v3.Position.ToVec3() - v1.Position.ToVec3();
                    var dv2 = v2.Position.ToVec3() - v1.Position.ToVec3();
                    var normalvec = dv1.CrossProduct(dv2);
                    triangle.Normal = new Engine_Point(normalvec.ToFloatArray(), PointType.Normal);
                }
            }
        }

        internal bool GenerateTangents()
        {
            Logger.Log("Generating Tangents");
            UseLargeIndex = true;
            var vbPoints = GetVbArray();
            var IndexData = GetLongIndexData();
            if (IndexData == null)
            {
                Logger.Log($"Maximum Limits Reached for this Model {Name}");
                return false;
            }

            Urho3D.GenerateTangents(vbPoints, GetVBSize(), GetLongIndexData(), IndexSize(), 0, IndexData.Length, TangentStart());



            var ps = vbPoints.ToList();
            var facescount = Engine_Faces.Count;

            for (int i = 0; i < facescount; i++)
            {
                var f = Engine_Faces[i];
                var triangleIndecees = f.GetIndecees();
                for (int tindex = 0; tindex < triangleIndecees.Length; tindex++)
                {
                    int rangestart = i * triangleIndecees.Length * 12;
                    var range = ps.GetRange(rangestart, triangleIndecees.Length * 12);

                    for (int p = 0; p < triangleIndecees.Length; p++)
                    {
                        var pindex = triangleIndecees[p];
                        ModelPoints[pindex].TangentCoor.X = range[08 + (12 * p)];
                        ModelPoints[pindex].TangentCoor.Y = range[09 + (12 * p)];
                        ModelPoints[pindex].TangentCoor.Z = range[10 + (12 * p)];
                        ModelPoints[pindex].TangentCoor.L = range[11 + (12 * p)];
                    }
                }
            }

            return true;
        }
    }
}