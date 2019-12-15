using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Urho3DNet;

namespace EngineViewer._3D.Extention
{
    public static class Vertex_EX
    {
        public static float[] ToFloatArray(this Vector4 vec4)
        {
            float[] pointInFloatArr = new float[4];
            pointInFloatArr[0] = vec4.X;
            pointInFloatArr[1] = vec4.Y;
            pointInFloatArr[2] = vec4.Z;
            pointInFloatArr[3] = vec4.W;
            return pointInFloatArr;
        }

        public static float[] ToFloatArray(this Vector3 vec3)
        {
            float[] pointInFloatArr = new float[3];
            pointInFloatArr[0] = vec3.X;
            pointInFloatArr[1] = vec3.Y;
            pointInFloatArr[2] = vec3.Z;
            return pointInFloatArr;
        }

        public static float[] ToFloatArray(this Vector2 vec2)
        {
            float[] pointInFloatArr = new float[2];
            pointInFloatArr[0] = vec2.X;
            pointInFloatArr[1] = vec2.Y;
            return pointInFloatArr;
        }

        public static Vector3 Multiply(this Vector3 source,Vector3 value)
        {
            var multipliedVector = new Vector3(
            source.X * value.X,
            source.Y * value.Y,
            source.Z * value.Z
            );
            return multipliedVector;
        }
    }
}
