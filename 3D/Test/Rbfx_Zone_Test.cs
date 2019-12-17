

using EngineViewer.Actions._3D.Animations;
using EngineViewer.Actions._3D.RbfxUtility;
using Urho3DNet;

namespace EngineViewer._3D.Test
{
    class Engn_Zone_Test
    {
        public Zone Rbfx_Zone;
        public Engn_Zone_Test(Node RootNode)
        {

            //zone is a region that has enviromental effects including light, any object within this zone will get affected.
            var zoneNode = RootNode.CreateChild("Zone");
            Rbfx_Zone = zoneNode.CreateComponent<Zone>();
            zoneNode.SetTemporary(true);

            // Set same volume as the Octree, set a close bluish fog and some ambient light
            Rbfx_Zone.SetBoundingBox(new BoundingBox(-1000f, 1000f));
            Rbfx_Zone.AmbientColor = new Color(0.05f, 0.1f, 0.15f);
            Rbfx_Zone.FogColor = new Color(0.1f, 0.2f, 0.3f);
            Rbfx_Zone.FogStart = 0;
            Rbfx_Zone.FogEnd = 100;


        }


    }
}
