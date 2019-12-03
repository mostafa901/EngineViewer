

using Urho;

namespace InSitU.Controls.TypicalControls._3D
{
	class Urho_DirectionalLight
	{
		public Light light;
		public Node lightNode;
		public Urho_DirectionalLight(Scene scene)
		{
			// Create a directional light to the world. Enable cascaded shadows on it
			  lightNode = scene.CreateChild("DirectionalLight");
			lightNode.SetDirection(new Vector3(0.6f, -1.0f, 0.8f));
			light = lightNode.CreateComponent<Light>();

			light.LightType = LightType.Directional;
			light.CastShadows = true;
			light.ShadowBias = new BiasParameters(0.00025f, 0.5f);

			// Set cascade splits at 10, 50 and 200 world units, fade shadows out at 80% of maximum shadow distance
			light.ShadowCascade = new CascadeParameters(10.0f, 50.0f, 200.0f, 0.0f, 0.8f);
		}
	}
}
