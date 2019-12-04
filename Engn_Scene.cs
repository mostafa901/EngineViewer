using EngineViewer.Actions._3D.Animations;
using EngineViewer.Actions._3D.Models;
using EngineViewer.Actions._3D.RbfxUtility;
using EngineViewer.Actions._3D.UI;
using EngineViewer.Controls.TypicalControls._3D.Test;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Urho3DNet;
using static EngineViewer.Actions._3D.UI.UIMenu;

namespace EngineViewer
{
	public class Engn_Scene
	{
		 
		public Engn_Scene()
		{

		}
		public void Load(IntPtr engineHandle)
		{
			 
			Task.Run(() =>
			{
				DefaultScene.Parent = engineHandle;
				using (var context = new Context())
				{
					using (var application = new DefaultScene(context))
					{
						application.Run();
					}
				}
			});

		}
	}

	public partial class DefaultScene: Application
	{
		public static IntPtr Parent;
		public static Scene scene;
		private Viewport viewport;

		private Node light;

		public Node RootNode;
		public UIMenu uiMenu;
		public DefaultScene(Context context) : base(context)
		{
		}

		protected override void Dispose(bool disposing)
		{
			Context.Renderer.SetViewport(0, null);
			//Enable disposal of viewport by making it unreferenced by engine.
			//cam.camera.Dispose();
			//viewport.Dispose();
			//scene.Dispose();

			//light.Dispose();
			base.Dispose();
		}

		public override void Setup()
		{
			var currentDir = @"D:\Revit_API\Downloaded_Library\Source\rbfx\cmake-build\bin\Data";
			engineParameters_[Urho3D.EpFullScreen] = false;
			engineParameters_[Urho3D.EpExternalWindow] = Parent;
			engineParameters_[Urho3D.EpWindowResizable] = true;
			engineParameters_[Urho3D.EpWindowWidth] = 1200;
			engineParameters_[Urho3D.EpWindowHeight] = 800;
			engineParameters_[Urho3D.EpWindowTitle] = "Hello C#";
			engineParameters_[Urho3D.EpResourcePrefixPaths] = $"{currentDir};{currentDir}/..";
		}

		Engn_Camera cam = null;
		Engn_Selection Selection;

		public override void Start()
		{
			Context.Input.SetMouseVisible(true);

			//SetupResources Location
			string ResDir = @"./Resources/3D";
			Context.Cache.AddResourceDir(ResDir);
			Context.Cache.AddResourceDir(@"c:\windows\fonts");
			Context.UI.Cursor = new Urho3DNet.Cursor(Context);

			// Scene
			scene = new Scene(Context);
			scene.CreateComponent<Octree>();

			Selection = new Engn_Selection();

			//Camera-Viewport
			cam = new Engn_Camera(scene);
			cam.CameraNode.Position = (new Vector3(0, 2, -20));
			cam.LookAt(Vector3.Zero);
			viewport = new Viewport(Context);
			viewport.Scene = scene;
			viewport.Camera = cam.camera;
			Context.Renderer.SetViewport(0, viewport);

			// Background
			Context.Renderer.DefaultZone.FogColor = new Color(.1f, .1f, .1f, .0f);

			var rp = viewport.RenderPath;
			for (uint i = 0; i < rp.Commands.Count; i++)
			{
				var cmd = rp.GetCommand(i);
				if (cmd.Type == RenderCommandType.CmdClear)
				{
					cmd.UseFogColor = true;
					cmd.ClearColor = new Color(.2f, 0.25f, 0.3f, .0f);
				}
			}


			//Plan
			//we don't to set wireplan to rootnode, as we need it for any root node that can be created later
			new Engn_WirePlane(scene);

			//Add Zone
			new Engn_Zone_Test(scene);

			//AddPlan
			new Engn_Plan(scene);

			// Light
			light = scene.CreateChild("Light");
			var l = light.CreateComponent<Light>();
			light.Position = (new Vector3(0, 50, -1));
			l.Range = 100f;
			light.LookAt(Vector3.Zero);
			l.LightType = LightType.LightPoint;
			l.CastShadows = true;
			l.ShadowBias = new BiasParameters(0.00025f, 0.5f);
			// Set cascade splits at 10, 50 and 200 world units, fade shadows out at 80% of maximum shadow distance
			l.ShadowCascade = new CascadeParameters(10.0f, 50.0f, 200.0f, 0.0f, 0.8f);


			//Create Root Components for all models
			RootNode = scene.CreateChild("RootNode");

			//setup Menu			
			uiMenu = new UIMenu(RootNode, Selection);

			//info window shows name by hovered
			SetupInfoWindow();

			SubscribeToEvent(E.Update, args =>
			{
				uiMenu.SetupMenu();
				//what to do if selection is nothing
				onUnSelect();

				//camera movement
				cam.FirstPersonCamera(this, Context.Time.TimeStep, 10, Selection?.SelectedModel?.Node);

				//CheckSelection
				var hoverselected = Selection.SelectGeometry(this, scene, cam);
				uiMenu.Selection = Selection;
				uiMenu.RootNode = RootNode;

				if (Context.Input.GetMouseButtonPress(Urho3DNet.MouseButton.MousebLeft))
				{
					touraroundboxes(Selection.SelectedModel);
				}

				//invoke any actions in the list
				if (Actions.Count > 0)
				{
					var runningactions = Actions.ToList();
					Actions.Clear();
					for (int i = 0; i < runningactions.Count; i++)
					{
						runningactions[i].Invoke();
					}
				}

				if (Context.Input.GetMouseButtonClick(Urho3DNet.MouseButton.MousebRight))
				{
					if (Selection.SelectedModel != null)
					{
						uiMenu.ActionMenu = menuaction.ObjectContext;
					}
				}

				DisplayInfoText(hoverselected);
			});
		}

#if false
		private void CreateCustomShape()
		{
			var geonode = RootNode.CreateChild("GeoNode");
			var geom = geonode.CreateComponent<CustomGeometry>();
			var mat = Context.Cache.GetResource<Material>("Materials/Stone.xml");
			//	mat.SetShaderParameter("MatDiffColor", Color.Gray);
			//	mat.Occlusion = true;

			geom.SetMaterial(mat);

			string imp = @"D:\Program Files\Autodesk\Revit 2018\Testvertex.json";
			var verte = new List<string>().JDeserializemyData(System.IO.File.ReadAllText(imp));

			geom.BeginGeometry(0, PrimitiveType.TriangleList);
			for (int i = 0; i < verte.Count; i++)
			{
				var v = verte[i];
				var ve = v.Replace("(", "").Replace(")", "");
				var vs = ve.Split(',');
				float x = float.Parse(vs[0]);
				float y = float.Parse(vs[1]);
				float z = float.Parse(vs[2]);
				float xn = float.Parse(vs[3]);
				float yn = float.Parse(vs[4]);
				float zn = float.Parse(vs[5]);
				geom.DefineVertex(new Vector3(x, y, z));
				geom.DefineColor(Color.Black);
				geom.DefineTexCoord(new Vector2());
				geom.DefineNormal(new Vector3(xn, yn, zn));
			}
			geom.Commit();

			geonode.Rotate(new Quaternion(90, 0.0f, 0));
		}

#endif
		Window infowindow = null;

		private void SetupInfoWindow()
		{
			//info text
			infowindow = Context.UI.Root.CreateChild(nameof(Window)) as Window;
			// Set Window size and layout settings
			infowindow.MinSize = new IntVector2(50, 20);
			infowindow.SetLayout(LayoutMode.LmVertical, 6, new IntRect(6, 6, 6, 6));
			infowindow.SetAlignment(Urho3DNet.HorizontalAlignment.HaLeft, Urho3DNet.VerticalAlignment.VaTop);
			infowindow.SetColor(new Color(.28f, .28f, .28f, .28f));
			infowindow.Name = "Window";
			infowindow.SetVisible(false);

			var infotext = infowindow.CreateChild(nameof(Text)) as Text;
			infotext.Name = "InfoText";
			infotext.SetColor(Color.Red);
			infotext.SetFont("Arial.ttf", 12);
		}

		private void DisplayInfoText(StaticModel hoverselected)
		{
			if (hoverselected != null)
			{
				var infotext = infowindow.GetChild("InfoText") as Text;
				infowindow.SetVisible(true);
				infowindow.Position = Context.Input.MousePosition + new IntVector2(10, 10);
				string todisplay = hoverselected.Node.Name;

				var cusComponent = hoverselected.Node.GetComponent<CustomNodeComponent>();
				if (cusComponent == null)
				{
					cusComponent = hoverselected.Node.CreateComponent<CustomNodeComponent>();
				}
				if (!string.IsNullOrEmpty(cusComponent.Vmap))
				{
					if (cusComponent.Info == null || cusComponent.Info.Count == 0)
					{
#if false
						cusComponent.Info = new List<data>().JDeserializemyData(cusComponent.Vmap); 
#endif
					}
				}
				if (cusComponent.Info != null)
				{
					foreach (var item in cusComponent.Info)
					{
						todisplay += $"\n{item.Key}: {item.value}";
					}
					infotext.SetText(todisplay);
				}
				else
					infotext.SetText(todisplay);
			}
			else
			{
				infowindow.SetVisible(false);
			}
		}

		string objname = "wall";
		void touraroundboxes(StaticModel model)
		{
			//	var model = scene.GetChild("Boxes").GetChildren()[(int)Randoms.Next(1, 2000)].GetComponent<StaticModel>();
			if (model == null) return;
			var offset = model.WorldBoundingBox.Max.DistanceToPoint(model.WorldBoundingBox.Min);
			cam.MoveToSelected(model.WorldBoundingBox.Center, model.WorldBoundingBox.Center, offset + 5);

#if false
			var l = model.Node.CreateComponent<Light>();
			l.LightType = LightType.LightPoint;
			l.SetIntensitySortValue(100f);
			l.Range = 20f;
			l.Color = Color.Yellow;
			l.SetZone(scene.GetComponent<Zone>(true)); 
#endif
		}
		public static List<Action> Actions = new List<Action>();
#if false
		private async void DisplaceAll(Node selected, string name)
		{
			var pos = selected.Position;
			var childs = selected.GetChildren();


			foreach (var node in childs)
			{
				if (!node.Name.ToLower().Contains(name)) continue;
				var cuscmp = node.GetComponent<CustomNodeComponent>();
				if (cuscmp == null)
				{
					cuscmp = new CustomNodeComponent(Context);
					cuscmp.OriginalPosition = node.Position;
					node.AddComponent(cuscmp, 1, CreateMode.Local);
				}

				var chpos = node.Position;
				var dir = selected.Position - chpos;
				dir.Normalize();
				var dis = chpos.DistanceToPoint(selected.Position);
				var rpos = new Vector3(Randoms.Next(0, 200f) - 100f, Randoms.Next(0, 200f) - 100f, Randoms.Next(0, 200f) - 100f);
				// Orient using random pitch, yaw and roll Euler angles
				//var rrot = new Quaternion(Randoms.Next(0, 360.0f), Randoms.Next(0, 360.0f), Randoms.Next(0, 360.0f));

				var move = node.GetComponent<MoveObject>();
				if (move == null) move = node.CreateComponent<MoveObject>();

				if (cuscmp.OriginalPosition == node.Position)
				{
					move.TargetPos = rpos;
					if (cuscmp.OriginalMaterial == null) cuscmp.OriginalMaterial = node.GetComponents().Where(o => o is StaticModel).Cast<StaticModel>()?.FirstOrDefault().GetMaterial();
					node.GetComponents().Where(o => o is StaticModel).Cast<StaticModel>().ForEach(o => o.SetMaterial(Material_Ext.SetMaterialFromColor(Color.Red, true)));
				}
				else
				{
					move.TargetPos = cuscmp.OriginalPosition;
					node.GetComponents().Where(o => o is StaticModel).Cast<StaticModel>().ForEach(o => o.SetMaterial(cuscmp.OriginalMaterial));
				}

			}
		}


		void CreateModelfromScratch()
		{
			const int numVertices = 18;
			float[] vertexData =
			{
					// Position             Normal				Texture
					0.0f, 0.5f, 0.0f,       0.5f, 0.5f, 0.5f,   0.0f,0.0f,
					-0.5f, -0.5f, 0.5f,     0.5f, 0.5f, 0.5f,   0.5f,0.5f,
					0.5f, -0.5f, 0.5f,      0.5f, 0.0f, 0.5f,   1.0f,1.0f,

					0.0f, 0.5f, 0.0f,      -0.5f, 0.5f, 0.0f,   0.0f,0.0f,
					-0.5f, -0.5f, -0.5f,   -0.5f, 0.5f, 0.0f,   0.5f,0.5f,
					-0.5f, -0.5f, 0.5f,    -0.5f, 0.5f, 0.0f,   1.0f,1.0f,

					0.0f, 0.5f, 0.0f,      -0.5f, -0.5f, 0.5f,   0.0f,0.0f,
					0.5f, -0.5f, -0.5f,    -0.5f, -0.5f, 0.5f,   0.5f,0.5f,
					-0.5f, -0.5f, -0.5f,   -0.5f, -0.5f, 0.5f,   1.0f,1.0f,

					0.0f, 0.5f, 0.0f,      -0.5f, -0.5f, 0.5f,   0.0f,0.0f,
					0.5f, -0.5f, 0.5f,     -0.5f, -0.5f, 0.5f,   1.0f,1.0f,
					0.5f, -0.5f, -0.5f,    -0.5f, -0.5f, 0.5f,   0.5f,0.5f,

					0.5f, -0.5f, -0.5f,     0.0f, 0.0f, 0.0f,   0.0f,0.0f,
					0.5f, -0.5f, 0.5f,      0.0f, 0.0f, 0.0f,   0.5f,0.5f,
					-0.5f, -0.5f, 0.5f,     0.0f, 0.0f, 0.0f,   1.0f,1.0f,

					0.5f, -0.5f, -0.5f,     0.0f, 0.0f, 0.0f,   0.0f,0.0f,
					-0.5f, -0.5f, 0.5f,     0.0f, 0.0f, 0.0f,   0.5f,0.5f,
					-0.5f, -0.5f, -0.5f,    0.0f, 0.0f, 0.0f,   1.0f,1.0f
				};

			short[] indexData =
			{
					0, 1, 2,
					3, 4, 5,
					6, 7, 8,
					9, 10, 11,
					12, 13, 14,
					15, 16, 17
			};

			Urho3DNet.Model fromScratchModel = new Urho3DNet.Model(Context);
			VertexBuffer vb = new VertexBuffer(Context, false);
			IndexBuffer ib = new IndexBuffer(Context, false);
			Geometry geom = new Geometry(Context);

			// Shadowed buffer needed for raycasts to work, and so that data can be automatically restored on device loss
			vb.SetShadowed(true);
			vb.SetSize(numVertices, VertexMask.MaskPosition | VertexMask.MaskNormal | VertexMask.MaskTexcoord1, false);

			var res = vb.SetData(vertexData);

			ib.SetShadowed(true);
			ib.SetSize(numVertices, false, false);

			res = ib.SetData(indexData);

			geom.SetVertexBuffer(0, vb);
			geom.IndexBuffer = ib;
			geom.SetDrawRange(PrimitiveType.TriangleList, 0, numVertices, true);

			fromScratchModel.NumGeometries = 1;
			fromScratchModel.SetGeometry(0, 0, geom);
			fromScratchModel.BoundingBox = new BoundingBox(new Vector3(-0.5f, -0.5f, -0.5f), new Vector3(0.5f, 0.5f, 0.5f));

			Node node = RootNode.CreateChild("FromScratchObject");
			node.Position = (new Vector3(0.0f, 3.0f, 0.0f));
			StaticModel sm = node.CreateComponent<StaticModel>();
			sm.SetModel(fromScratchModel);
			sm.CastShadows = true;

			//var mat = RootNode.Cache.GetResource<Material>("Materials/Stone.xml");
			//sm.SetMaterial(mat);

			sm.SetMaterial(Material_Ext.SelectedMaterial);

			sm.SetModel(fromScratchModel);
			node.CreateComponent<RotateObject>();
		}
#endif
		void onUnSelect()
		{
			if (Selection.SelectedModel == null)
			{
				uiMenu.ActionMenu = menuaction.none;
			}
		}
	}
}
