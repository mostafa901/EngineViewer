using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Urho3DNet;

namespace EngineViewer._3D.Utility
{
    public static class Engine_Math
    {
        public static float[] GetTangent(List<float[]> vecs, List<float[]> texcoor)        
        {


            var vec01 = new Vector3(vecs[0]);
            var vec02 = new Vector3(vecs[1]);
            var vec03 = new Vector3(vecs[2]);
            var uv01 = new Vector2(texcoor[0]);
            var uv02 = new Vector2(texcoor[1]);
            var uv03 = new Vector2(texcoor[2]);

            var edge1 = vec02 - vec01;
            var edge2 = vec03 - vec01;
            var deltaUV1 = uv02 - uv01;
            var deltaUV2 = uv03 - uv01;

            float f = 1.0f / (deltaUV1.X * deltaUV2.Y - deltaUV2.X * deltaUV1.Y);
            if (float.IsInfinity(f)) f = 1;
            Vector3 tangent = new Vector3();
            tangent.X = f * (deltaUV2.Y * edge1.X - deltaUV1.Y * edge2.X);
            tangent.Y = f * (deltaUV2.Y * edge1.Y - deltaUV1.Y * edge2.Y);
            tangent.Z = f * (deltaUV2.Y * edge1.Z - deltaUV1.Y * edge2.Z);
            tangent.Normalize();

            return new float[] { tangent.X, tangent.Y, tangent.Z, 0 };

        }
    }
}
