using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Urho3DNet;

namespace EngineViewer.Actions._3D.Models
{
	class Engn_Plan
	{
		public Node planeNode;
		public StaticModel plane;

		public Engn_Plan(Scene scene)
		{
			// Create scene node & StaticModel component for showing a static plane
			planeNode = scene.CreateChild("Plane");
			planeNode.SetScale(new Vector3(100, 1, 100));
            planeNode.Rotate(new Quaternion(0));
			plane = planeNode.CreateComponent<StaticModel>();
			plane.SetModel(scene.Context.Cache.GetResource<Urho3DNet.Model>("Models/Plane.mdl"));
			plane.SetMaterial(scene.Context.Cache.GetResource<Material>("Materials/StoneTiled.xml"));			
		}
	}
}
