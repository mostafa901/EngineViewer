


using Shared_Utility;
using Urho3DNet;

namespace EngineViewer.Actions._3D.Animations
{
	[ObjectFactory]
	class MoveObject : Base_CustomLogicComponent
	{

		public MoveObject(Context context) : base(context)
		{
			UpdateEventMask = UpdateEvent.UseUpdate;

		}
		float consumed = 0f;
		public Vector3 TargetPos { get; set; }
		 
		public override void Update(float timeStep)
		{
			timeStep /= Duration;
			//if (TargetPos == null) TargetPos = node_.Position;
			consumed = consumed >= 1 ? 0 : consumed += timeStep;

			if (TargetPos != Node.Position)
			{
				var ease = EaseMath.ElasticIn(consumed);
				Node.Position = getposition(Node.Position, TargetPos, ease);

			}
			else
			{
				consumed = 0;
				PostUpdate();

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
