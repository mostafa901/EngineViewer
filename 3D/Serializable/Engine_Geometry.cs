using EngineViewer._3D.Extention;
using System;
using System.Collections.Generic;
using System.Linq;
using Urho3DNet;
using Utility.IO;
using Logger = Shared_Utility.Logger.Logger;

namespace EngineViewer.Serializable
{
    public class Engine_Geometry
    {
        public string Name { get; set; } = "";
        public Engine_Point Position { get; set; }
        public List<Engine_Face> Engine_Faces { get; set; }
        public Engine_Point Rotation { get; set; }
        public Engine_Point Minimum { get; set; }
        public Engine_Point Maximum { get; set; }
        public Engine_Point Color { get; set; }
        public bool UseLargeIndex = false;
        public CullMode GeoCullModel { get; set; }
        public Engine_Point Flip { get; set; }
        public string FileName { get; set; }

        public Engine_Geometry()
        {
            Engine_Faces = new List<Engine_Face>();
            Position = new Engine_Point() { EngPointType = PointType.Position };
            Rotation = new Engine_Point() { EngPointType = PointType.Rotation };
            Flip = new Engine_Point(1, 1, 1, 1, PointType.Normal);
            GeoCullModel = CullMode.CullCcw;
            FileName = System.IO.Path.GetTempPath();
        }

        public enum PointType
        {
            Color,
            Vertex,
            Normal,
            Texture,
            Position,
            Tangent,
            Rotation,
            All
        }

        public Engine_Face GetFace(int index)
        {
            return Engine_Faces[index];
        }

        public void Scale(Vector3 value)
        {
            var engFacesCount = Engine_Faces.Count();
            for (int i = 0; i < engFacesCount; i++)
            {
                var f = Engine_Faces[i];
                f.Scale(value);
            }
        }

        public class Engine_ModelPoint
        {
            public Engine_Point EngPosition;
            public Engine_Point EngNormal;
            public Engine_Point EngTexture;
            public Engine_Point EngTangent;
            public Engine_Point EngColor;

            internal int GetSize()
            {
                int size = 0;
                if (EngPosition != null) size += EngPosition.GetPointSize();
                if (EngNormal != null) size += EngPosition.GetPointSize();
                if (EngTexture != null) size += EngPosition.GetPointSize();
                if (EngTangent != null) size += EngPosition.GetPointSize();
                if (EngColor != null) size += EngPosition.GetPointSize();

                return size;
            }

            public float[] GetFloat(PointType ptype)
            {
                List<float> floatedPoints = new List<float>();
                switch (ptype)
                {
                    case PointType.Color:
                        floatedPoints.AddRange(EngColor.ToFloatArray());
                        break;

                    case PointType.Vertex:
                        floatedPoints.AddRange(EngPosition.ToFloatArray());
                        break;

                    case PointType.Normal:
                        floatedPoints.AddRange(EngNormal.ToFloatArray());

                        break;

                    case PointType.Texture:
                        floatedPoints.AddRange(EngTexture.ToFloatArray());

                        break;

                    case PointType.Position:

                        break;

                    case PointType.Tangent:
                        floatedPoints.AddRange(EngTangent.ToFloatArray());

                        break;

                    case PointType.Rotation:
                        break;

                    case PointType.All:
                        {
                            if (EngPosition != null) floatedPoints.AddRange(GetFloat(PointType.Vertex));
                            if (EngNormal != null) floatedPoints.AddRange(GetFloat(PointType.Normal));
                            if (EngTexture != null) floatedPoints.AddRange(GetFloat(PointType.Texture));
                            if (EngTangent != null) floatedPoints.AddRange(GetFloat(PointType.Tangent));
                            if (EngColor != null) floatedPoints.AddRange(GetFloat(PointType.Color));
                        }
                        break;
                }
                return floatedPoints.ToArray();
            }

            public List<Engine_Point> GetEngPoints()
            {
                return new List<Engine_Point>() { EngPosition, EngNormal, EngTexture, EngTangent, EngColor };
            }

            public Engine_ModelPoint Clone()
            {
                return new Engine_ModelPoint().JDeserializemyData(this.JSerialize());
            }
        }

        public class Engine_Triangle
        {
            public Engine_ModelPoint V1;
            public Engine_ModelPoint V2;
            public Engine_ModelPoint V3;

            public void Scale(Vector3 value)
            {
                var triPoints = GetPoints();
                foreach (var engPoint in triPoints)
                {
                    engPoint.EngPosition.Scale(value);
                    engPoint.EngTexture.Scale(value);
                }
            }

            public float[] GetFloatPoints(PointType ptype)
            {
                List<float> floatedPoints = new List<float>();
                var triPoints = GetPoints();
                foreach (var engPoint in triPoints)
                {
                    floatedPoints.AddRange(engPoint.GetFloat(ptype));
                }
                return floatedPoints.ToArray();
            }

            public List<Engine_Point> GetPointsOfType(PointType ptype)
            {
                List<Engine_Point> engPoints = new List<Engine_Point>();
                var triPoints = GetPoints();
                foreach (var engPoint in triPoints)
                {
                    switch (ptype)
                    {
                        case PointType.Color:
                            break;

                        case PointType.Vertex:
                            engPoints.Add(engPoint.EngPosition);
                            break;

                        case PointType.Normal:
                            engPoints.Add(engPoint.EngNormal);
                            break;

                        case PointType.Texture:
                            engPoints.Add(engPoint.EngTexture);
                            break;

                        case PointType.Position:

                            break;

                        case PointType.Tangent:
                            engPoints.Add(engPoint.EngTangent);
                            break;

                        case PointType.Rotation:
                            break;

                        default:
                            break;
                    }
                }
                return engPoints;
            }

            internal void GenerateNormal()
            {
                var dv1 = V3.EngPosition.ToVec3() - V1.EngPosition.ToVec3();
                var dv2 = V2.EngPosition.ToVec3() - V1.EngPosition.ToVec3();
                var normalvec = dv1.CrossProduct(dv2);
                normalvec = normalvec.Multiply(V3.EngNormal.ToVec3());
                V1.EngNormal = new Engine_Point(normalvec.ToFloatArray(), PointType.Normal);
                V2.EngNormal = new Engine_Point(normalvec.ToFloatArray(), PointType.Normal);
                V3.EngNormal = new Engine_Point(normalvec.ToFloatArray(), PointType.Normal);
            }

            public List<Engine_ModelPoint> GetPoints()
            {
                return new List<Engine_ModelPoint>() { V1, V2, V3 };
            }
        }

        public class Engine_Face
        {
            public string FaceId;
            public List<Engine_Triangle> EngTriangles;
            public Engine_Point FaceColor { get; set; }

            public Engine_Face()
            {
                FaceColor = new Engine_Point(.28f, .28f, .28f, 1, PointType.Color);
            }
            public void Scale(Vector3 value)
            {
                var trianglesCount = EngTriangles.Count;
                for (int ti = 0; ti < trianglesCount; ti++)
                {
                    var triangle = EngTriangles[ti];
                    triangle.Scale(value);
                }
            }

            public int GetEngPointSize()
            {
                return EngTriangles[0].V1.GetSize();
            }
        }

        public float[] GetFaceFloatPoints(int faceIndex)
        {
            List<float> floatedPoints = new List<float>();
            var face = Engine_Faces[faceIndex];
            var triangleCount = face.EngTriangles.Count();
            for (int i = 0; i < triangleCount; i++)
            {
                var triangle = face.EngTriangles[i];
                floatedPoints.AddRange(triangle.GetFloatPoints(PointType.All));
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
            if (Color == null) Color = new Engine_Point(1f, 1f, .28f, 1, PointType.Color);
            return new Color(Color.X, Color.Y, Color.Z, Color.L);
        }

        public short[] GetShortIndex()
        {
            var inds = Engine_Faces.SelectMany(o => o.EngTriangles).Count() * 3;
            List<short> indpoints = new List<short>();
            for (short ni = 0; ni < inds; ni++)
            {
                indpoints.Add(ni);
            }
            return indpoints.ToArray();
        }

        public int[] GetLongIndexData()
        {
            var inds = Engine_Faces.SelectMany(o => o.EngTriangles).Count() * 3;
            List<int> indpoints = new List<int>();

            for (int ni = 0; ni < inds; ni++)
            {
                indpoints.Add(ni);
            }
            return indpoints.ToArray();
        }

        public int GetVBSize()
        {
            Logger.Log("Getting VBSize");
            return Engine_Faces[0].GetEngPointSize() * sizeof(float);
        }

        private int TangentStart()
        {
            Logger.Log("Getting TangentStart");
            var f = Engine_Faces[0];
            return f.EngTriangles[0].V1.EngTexture == null ? 6 : 8;
        }

        private int IndexSize()
        {
            Logger.Log("Getting IndexSize");
            return UseLargeIndex ? sizeof(int) : sizeof(short);
        }

        public void GenerateNormals()
        {

            Logger.Log("Generating Normals");

            foreach (var engFace in Engine_Faces)
            {
                foreach (var triangle in engFace.EngTriangles)
                {
                    var trianglesCount = engFace.EngTriangles.Count();
                    triangle.GenerateNormal();
                }
            }
        }

         internal bool GenerateTangents()
        {
            Logger.Log("Generating Tangents");
            UseLargeIndex = true;
            var vbPoints = GetVbArray();
            var IndexData = GetLongIndexData();
            var tangentPosition = TangentStart();
            var vbSize = GetVBSize();
            if (IndexData == null)
            {
                Logger.Log($"Maximum Limits Reached for this Model {Name}");
                return false;
            }

            unsafe
            {
                fixed (float* vsIntPtr = vbPoints)
                {
                    fixed (int* indxPtr = IndexData)
                    {

                        Urho3D.GenerateTangents((IntPtr)vsIntPtr, (uint)vbSize, (IntPtr)indxPtr, (uint)IndexSize(), 0, (uint)IndexData.Length, 3 * sizeof(float), 6 * sizeof(float), (uint)tangentPosition * sizeof(float));
                    }
                } 
            }

            var ps = vbPoints.ToList();
            var facescount = Engine_Faces.Count;
            var pointSize = Engine_Faces[0].EngTriangles[0].V1.GetSize();
            var triangles = Engine_Faces.SelectMany(o => o.EngTriangles).ToList();

            foreach (var engTriangle in triangles)
            {
                var triIndex = triangles.IndexOf(engTriangle);
                int rangestart = triIndex * 3 * pointSize;
                var range = ps.GetRange(rangestart, 3 * pointSize);

                for (int rangeIndex = 0; rangeIndex < range.Count; rangeIndex++)
                {
                    var triPoints = engTriangle.GetPoints();
                    foreach (var engPoint in triPoints)
                    {
                        engPoint.EngTangent.X = range[tangentPosition];
                        engPoint.EngTangent.Y = range[tangentPosition + 1];
                        engPoint.EngTangent.Z = range[tangentPosition + 2];
                        engPoint.EngTangent.L = range[tangentPosition + 3];
                    }
                }
            }

            return true;
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

            public void Scale(Vector3 value)
            {
                X *= value.X;
                Y *= value.Y;
                Z *= value.Z;
                L *= 1;
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

            public override bool Equals(object obj)
            {
                if (!(obj is Engine_Point))
                    return base.Equals(obj);
                else
                {
                    var engpoint = obj as Engine_Point;
                    if (engpoint.X == X &&
                    engpoint.Y == Y &&
                    engpoint.Z == Z &&
                    engpoint.L == L) return true;
                    else return false;
                }
            }

            public int GetPointSize()
            {
                int value = 0;
                switch (EngPointType)
                {
                    case PointType.Color:
                    case PointType.Tangent:
                        value = 4;
                        break;

                    case PointType.Vertex:
                    case PointType.Normal:
                    case PointType.Position:
                    case PointType.Rotation:
                        value = 3;
                        break;

                    case PointType.Texture:
                        value = 2;
                        break;
                }
                return value;
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
                return $"{X}, {Y}, {Z}, {L}";
            }

            public Color ToColor()
            {
                return new Color(X, Y, Z, L);
            }
        }
    }
}