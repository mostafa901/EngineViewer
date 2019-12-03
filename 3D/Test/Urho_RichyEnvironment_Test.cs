

using System;
using Urho;
using Urho.Resources;

namespace InSitU.Controls.TypicalControls._3D.Test
{
	class Urho_RichyEnvironment_Test
	{
		public Urho_RichyEnvironment_Test(Scene scene,ResourceCache cache,Urho_DirectionalLight light)
		{

			for (int y = -5; y <= 5; ++y)
			{
				for (int x = -5; x <= 5; ++x)
				{
					var floorNode = scene.CreateChild("FloorTile");
					floorNode.Position = new Vector3(x * 20.5f, -0.5f, y * 20.5f);
					floorNode.Scale = new Vector3(20.0f, 1.0f, 20.0f);
					var floorObject = floorNode.CreateComponent<StaticModel>();
					floorObject.Model = cache.GetModel("Models/Box.mdl");
					floorObject.SetMaterial(cache.GetMaterial("Materials/Stone.xml"));
				}
			}

			// Create groups of mushrooms, which act as shadow casters
			const uint numMushroomgroups = 25;
			const uint numMushrooms = 25;

			for (uint i = 0; i < numMushroomgroups; ++i)
			{
				var groupNode = scene.CreateChild("MushroomGroup");
				groupNode.Position = new Vector3(Randoms.Next(0, 190.0f) - 95.0f, 0.0f, Randoms.Next(0, 190.0f) - 95.0f);

				for (uint j = 0; j < numMushrooms; ++j)
				{
					var mushroomNode = groupNode.CreateChild("Mushroom");
					mushroomNode.Position = new Vector3(Randoms.Next(0, 25.0f) - 12.5f, 0.0f, Randoms.Next(0, 25.0f) - 12.5f);
					mushroomNode.Rotation = new Quaternion(0.0f, Randoms.Next(0, 1f) * 360.0f, 0.0f);
					mushroomNode.SetScale(1.0f + Randoms.Next(0, 1f) * 4.0f);
					var mushroomObject = mushroomNode.CreateComponent<StaticModel>();
					mushroomObject.Model = cache.GetModel("Models/Mushroom.mdl");
					mushroomObject.SetMaterial(cache.GetMaterial("Materials/Mushroom.xml"));
					mushroomObject.CastShadows = true;
				}
			}

			// Create billboard sets (floating smoke)
			const uint numBillboardnodes = 25;
			const uint numBillboards = 10;

			for (uint i = 0; i < numBillboardnodes; ++i)
			{
				var smokeNode = scene.CreateChild("Smoke");
				smokeNode.Position = new Vector3(Randoms.Next(0, 200.0f) - 100.0f, Randoms.Next(0, 20.0f) + 10.0f, Randoms.Next(0, 200.0f) - 100.0f);

				var billboardObject = smokeNode.CreateComponent<BillboardSet>();
				billboardObject.NumBillboards = numBillboards;
				billboardObject.Material = cache.GetMaterial("Materials/LitSmoke.xml");
				billboardObject.Sorted = true;

				for (uint j = 0; j < numBillboards; ++j)
				{
					var bb = billboardObject.GetBillboardSafe(j);
					bb.Position = new Vector3(Randoms.Next(0, 12.0f) - 6.0f, Randoms.Next(0, 8.0f) - 4.0f, Randoms.Next(0, 12.0f) - 6.0f);
					bb.Size = new Vector2(Randoms.Next(0, 2.0f) + 3.0f, Randoms.Next(0, 2.0f) + 3.0f);
					bb.Rotation = Randoms.Next(0, 1f) * 360.0f;
					bb.Enabled = true;
				}

				// After modifying the billboards, they need to be "commited" so that the BillboardSet updates its internals
				billboardObject.Commit();
			}

			// Create shadow casting spotlights
			const uint numLights = 9;

			for (uint i = 0; i < numLights; ++i)
			{
				light.lightNode = scene.CreateChild("SpotLight");
				light.light = light.lightNode.CreateComponent<Light>();

				float angle = 0.0f;

				Vector3 position = new Vector3((i % 3) * 60.0f - 60.0f, 45.0f, (i / 3) * 60.0f - 60.0f);
				Color color = new Color(((i + 1) & 1) * 0.5f + 0.5f, (((i + 1) >> 1) & 1) * 0.5f + 0.5f, (((i + 1) >> 2) & 1) * 0.5f + 0.5f);

				light.lightNode.Position = position;
				light.lightNode.SetDirection(new Vector3((float)Math.Sin(angle), -1.5f, (float)Math.Cos(angle)));

				light.light.LightType = LightType.Spot;
				light.light.Range = 90.0f;
				light.light.RampTexture = cache.GetTexture2D("Textures/RampExtreme.png");
				light.light.Fov = 45.0f;
				light.light.Color = color;
				light.light.SpecularIntensity = 1.0f;
				light.light.CastShadows = true;
				light.light.ShadowBias = new BiasParameters(0.00002f, 0.0f);

				// Configure shadow fading for the lights. When they are far away enough, the lights eventually become unshadowed for
				// better GPU performance. Note that we could also set the maximum distance for each object to cast shadows
				light.light.ShadowFadeDistance = 100.0f; // Fade start distance
				light.light.ShadowDistance = 125.0f; // Fade end distance, shadows are disabled
													 // Set half resolution for the shadow maps for increased performance
				light.light.ShadowResolution = 0.5f;
				// The spot lights will not have anything near them, so move the near plane of the shadow camera farther
				// for better shadow depth resolution
				light.light.ShadowNearFarRatio = 0.01f;
			}
		}
	}
}
