


using Shared_Utility;
using Urho3DNet;

namespace EngineViewer.Actions._3D.Animations
{
	[ObjectFactory]
	class RotateObject: LogicComponent
	{
		Vector3 rotationSpeed = new Vector3(10.0f, 20.0f, 30.0f);

		public RotateObject(Context context) : base(context)
		{
			UpdateEventMask = UpdateEvent.UseUpdate;
			rotationSpeed = new Vector3(Randoms.Next(0, 3), Randoms.Next(0, 3), Randoms.Next(0, 3));
		}

		public override void Update(float timeStep)
		{

			var d = new Quaternion(10 * timeStep * rotationSpeed.X, 20 * timeStep * rotationSpeed.Y, 30 * timeStep * rotationSpeed.Z);

			Node.Rotate(d);
		}
	}
}
