using System;
using System.Linq;
using Urho3DNet;
using EngineViewer.Actions._3D.RbfxUtility;
using System.Diagnostics;
using EngineViewer.Actions._3D.Models;

namespace EngineViewer.Actions._3D.RbfxUtility
{
    internal static class Rbfx_Utility
    {
        public static bool Raycast(Scene scene, Engn_Camera cam, float maxDistance, out Vector3 hitPos, out Drawable hitDrawable)
        {
            hitDrawable = null;
            hitPos = Vector3.Zero;

            var oct = scene.GetComponent<Octree>();
            RayQueryResultList results = new RayQueryResultList();
            var ray = cam.GetRay();

            var query = new RayOctreeQuery(results, ray, RayQueryLevel.RayTriangle, maxDistance, DrawableFlags.DrawableGeometry);
            oct.Raycast(query);

            for (int i = 0; i < results.Count; i++)
            {
                var result = results[i];
                hitDrawable = result.Drawable;
                if (hitDrawable != null)
                {
                    if (hitDrawable.Node.GetComponent<WirePlan>() != null)
                    {
                        hitDrawable = null;
                        continue;
                    }
                } 
                hitPos = result.Position;
                return true;
            }

            return false;
        }

        public static void PaintDecal(Application app, Scene scene, Engn_Camera cam)
        {
            Vector3 hitPos;
            Drawable hitDrawable;

            if (Rbfx_Utility.Raycast(scene, cam, 250.0f, out hitPos, out hitDrawable))
            {
                var targetNode = hitDrawable.Node;
                var decal = targetNode.GetComponent<DecalSet>();

                if (decal == null)
                {
                    var cache = app.Context.Cache;
                    decal = targetNode.CreateComponent<DecalSet>();
                    var decalse = new DecalSet(app.Context);

                    decal.Material = Material_Ext.noLitFromColor(new Color(1, 0, 0, .5f), false);

                    //cache.GetMaterial("Materials/UrhoDecal.xml");
                }

                // Add a square decal to the decal set using the geometry of the drawable that was hit, orient it to face the camera,
                // use full texture UV's (0,0) to (1,1). Note that if we create several decals to a large object (such as the ground
                // plane) over a large area using just one DecalSet component, the decals will all be culled as one unit. If that is
                // undesirable, it may be necessary to create more than one DecalSet based on the distance

                decal.AddDecal(
                        hitDrawable, //drawable triangle
                        hitPos, //location
                        cam.CameraNode.Rotation, //Rotation
                        0.5f, //size
                        1.0f, //aspect ratio
                        1.0f, //depth
                        Vector2.ZERO, //uv top left coor
                        Vector2.ONE, //uv bottomright coor
                        0.0f, //timetolive
                        0.1f, //normalCutoff
                        uint.MaxValue);
            }
        }
    }
}