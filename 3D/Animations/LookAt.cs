


using System;
using System.Threading.Tasks;
using Urho3DNet;

namespace EngineViewer.Actions._3D.Animations
{
	[ObjectFactory]
	class LookAtObject : Base_CustomLogicComponent
	{

		public LookAtObject(Context context) : base(context)
		{
			UpdateEventMask = UpdateEvent.UseUpdate;

		}
		float consumed = 0f;
		public Vector3 TargetPos { get; set; }

	 
		public override void Update(float timeStep)
		{
			timeStep /= Duration;
			consumed = consumed >= 1 ? 0 : consumed += timeStep;
			var currentpos = Node.Position + (Node.Direction * TargetPos.DistanceToPoint(Node.Position));
			if (TargetPos != currentpos)
			{
				var ease = EaseMath.ElasticIn(consumed);
				Node.LookAt(getposition(currentpos, TargetPos, ease));

			}
			else
			{
				PostUpdate();
				consumed = 0;				 
			}
		}

		static Vector3 getposition(Vector3 A, Vector3 B, float perc)
		{
			if (perc >= 1) return B;
			var distance = A.DistanceToPoint(B);

			if (distance == 0 || perc == 0) return A;
			var f = (distance - ((1 - perc) * distance)) / distance;
			var x = A.X + f * (B.X - A.X);
			var y = A.Y + f * (B.Y - A.Y);
			var z = A.Z + f * (B.Z - A.Z);

			return new Vector3(x, y, z);
		}
	}
}
