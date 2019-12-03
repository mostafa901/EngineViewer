

using InSitU.Controls.TypicalControls._3D.Animations;
using Urho;
using Urho.Resources;

namespace InSitU.Controls.TypicalControls._3D.Test
{
	class Urho_AnimateChar
	{
		public Urho_AnimateChar(Scene scene,ResourceCache cache)
		{
			// Create animated models
			const int numModels = 100;
			const float modelMoveSpeed = 2.0f;
			const float modelRotateSpeed = 100.0f;
			var bounds = new BoundingBox(new Vector3(-47.0f, 0.0f, -47.0f), new Vector3(47.0f, 0.0f, 47.0f));

			for (var i = 0; i < numModels; ++i)
			{
				var modelNode = scene.CreateChild("Jack");
				modelNode.Position = new Vector3(Randoms.Next(-45, 45), 0.0f, Randoms.Next(-45, 45));
				modelNode.Rotation = new Quaternion(0, Randoms.Next(0, 360), 0);
				//var modelObject = modelNode.CreateComponent<AnimatedModel>();
				var modelObject = new AnimatedModel();
				modelNode.AddComponent(modelObject);
				modelObject.Model = cache.GetModel("Models/Jack.mdl");
				//modelObject.Material = cache.GetMaterial("Materials/Jack.xml");
				modelObject.CastShadows = true;

				// Create an AnimationState for a walk animation. Its time position will need to be manually updated to advance the
				// animation, The alternative would be to use an AnimationController component which updates the animation automatically,
				// but we need to update the model's position manually in any case
				var walkAnimation = cache.GetAnimation("Models/Jack_Walk.ani");
				var state = modelObject.AddAnimationState(walkAnimation);
				// The state would fail to create (return null) if the animation was not found
				if (state != null)
				{
					// Enable full blending weight and looping
					state.Weight = 1;
					state.Looped = true;
				}

				// Create our custom Mover component that will move & animate the model during each frame's update
				var mover = new Mover(modelMoveSpeed, modelRotateSpeed, bounds);
				modelNode.AddComponent(mover);
			}
		}
	}
}
