using EngineViewer.Actions._3D.Animations;
using EngineViewer.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Urho3DNet;

namespace EngineViewer.Actions._3D.RbfxUtility
{
	public static class Node_Ext
	{

		//CameraNode.MoveTo(vector3, Easing.In);
		public static void MoveTo(this Node node, Vector3 Endpos, Easeing easeing, float duration, float easetime = 2f)
		{

			

			ValueAnimation posanim = new ValueAnimation(DefaultScene.scene.Context);
			posanim.InterpolationMethod = InterpMethod.ImLinear;

			posanim.SetKeyFrame(0f, node.Position);
			//Debug.WriteLine($"Node: {node.ID} => Before Move {node.WorldPosition}");
			var incr = 1 / (easetime * 100);
			float consumed = 0;
			for (float i = 0; i < 1; i += incr)
			{
				float time = 0;
				switch (easeing)
				{
					case Easeing.In:
						time = EaseMath.ElasticIn(i);
						break;
					default:
						break;
				}

				posanim.SetKeyFrame(i * easetime, getposition(node.Position, Endpos, time));
				consumed += easetime * i;
			}

			posanim.SetKeyFrame(duration - consumed, Endpos);
			node.RemoveAttributeAnimation("Position");
			node.SetAttributeAnimation("Position", posanim, WrapMode.WmOnce);
			//	node.SetWorldPosition(Endpos);
			//Debug.WriteLine($"Node: {node.ID} => After Move {node.WorldPosition}");

		}

		static Vector3 getposition(Vector3 A, Vector3 B, float perc)
		{
			var distance = A.DistanceToPoint(B);
			var f = (distance - ((1 - perc) * distance)) / distance;
			var x = A.X + f * (B.X - A.X);
			var y = A.Y + f * (B.Y - A.Y);
			var z = A.Z + f * (B.Z - A.Z);

			return new Vector3(x, y, z);
		}
	}
}
