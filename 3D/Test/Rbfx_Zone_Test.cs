

using EngineViewer.Actions._3D.Animations;
using EngineViewer.Actions._3D.RbfxUtility;
using Urho3DNet;

namespace EngineViewer.Controls.TypicalControls._3D.Test
{
	class Engn_Zone_Test
	{
		 
		public Engn_Zone_Test(Node RootNode)
		{

			//zone is a region that has enviromental effects including light, any object within this zone will get affected.
			var zoneNode = RootNode.CreateChild("Zone");
			var zone = zoneNode.CreateComponent<Zone>();

			// Set same volume as the Octree, set a close bluish fog and some ambient light
			zone.SetBoundingBox(new BoundingBox(-10000.0f, 10000.0f));
			zone.AmbientColor = new Color(0.05f, 0.1f, 0.15f);
			zone.FogColor = new Color(0.1f, 0.2f, 0.3f);
			zone.FogStart = 50;
			zone.FogEnd = 100;
			 
		}


	}
}
