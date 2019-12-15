using EngineViewer.Actions._3D.Animations;
using EngineViewer.Actions._3D.RbfxUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Urho3DNet;

namespace EngineViewer._3D.Test
{
	class Rbfx_RandomBoxes
	{
		public Rbfx_RandomBoxes(Node RootNode)
		{
			var boxesNode = RootNode.CreateChild("Boxes");
            boxesNode.SetTemporary(true);
			const int numObjects = 2000;
			var boxModel = RootNode.Context.Cache.GetResource<Urho3DNet.Model>("Models/Box.mdl");
			//var boxMaterial = RootNode.Context.Cache.GetResource<Material>("Materials/Stone.xml");
			var boxMaterial = Material_Ext.TransParentMaterial(new Color(1,0,0,.5f));
	 

			for (var i = 0; i < numObjects; ++i)
			{
				Node boxNode = new Node(RootNode.Context);				
				boxNode.Name = "Box" + i.ToString("00");
				boxesNode.AddChild(boxNode, 0);
				boxNode.Position = new Vector3(Shared_Utility.Randoms.Next(0, 200f) - 100f, Shared_Utility.Randoms.Next(0, 200f) - 100f, Shared_Utility.Randoms.Next(0, 200f) - 100f);
				//Orient using random pitch, yaw and roll Euler angles
				boxNode.Rotation = new Quaternion(Shared_Utility.Randoms.Next(0, 360.0f), Shared_Utility.Randoms.Next(0, 360.0f), Shared_Utility.Randoms.Next(0, 360.0f));

				var boxObject = boxNode.CreateComponent<StaticModel>();
				boxObject.SetModel(boxModel);
				boxObject.SetMaterial(boxMaterial);
				boxObject.CastShadows = true;
				boxNode.CreateComponent<RotateObject>();
				var basenode = boxNode.CreateComponent<CustomNodeComponent>();
				basenode.OriginalPosition = new Vector3(5, 10, 5);
                basenode.OriginalMaterial = boxMaterial;
			}
		}
	}
}
