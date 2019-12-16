using EngineViewer._3D.TCP;
using EngineViewer._3D.Test;
using EngineViewer.Actions._3D.Models;
using EngineViewer.Actions._3D.RbfxUtility;
using EngineViewer.Actions._3D.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Urho3DNet;
using Utility.Constant;
using Utility.IO;
using static EngineViewer.Actions._3D.UI.UIMenu;
using static EngineViewer.Serializable.Engine_Geometry;
using Logger = Shared_Utility.Logger.Logger;

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
        private Node lightNode;
        public static DefaultScene Instance;
        public Node RootNode;
        public UIMenu uiMenu;

        public DefaultScene(Context context) : base(context)
        {
            Instance = this;
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
            engineParameters_[Urho3D.EpWindowWidth] = 1800;
            engineParameters_[Urho3D.EpWindowHeight] = 800;
            engineParameters_[Urho3D.EpWindowTitle] = "Hello C#";
            engineParameters_[Urho3D.EpResourcePrefixPaths] = $"{currentDir};{currentDir}/..";
        }

        private Engn_Camera cam = null;
        private Engn_Selection Selection;

        public override void Start()
        {
            Context.Input.SetMouseVisible(true);

            SetupResourcePaths();
            Selection = new Engn_Selection();
            SetupScene();
            SubscribeEvents();

            InitializeTcpConnection();

            DebugHud debugHud = Engine.CreateDebugHud();
            debugHud.Mode = DebugHudMode.DebughudShowAll;
            RunOnce();
        }

        private void SubscribeEvents()
        {
            SubscribeToEvent(E.Update, args =>
            {
                float moveSpeed = 5;
                uiMenu.RenderMenu();

                //what to do if selection is nothing
                onUnSelect();

                //camera movement
                if (Input.GetKeyPress(Key.KeyShift)) moveSpeed *= .5f;
                cam.FirstPersonCamera(this, Context.Time.TimeStep, moveSpeed, Selection?.SelectedModel?.Node);

                //CheckSelection
                Drawable hoverselected = null;
                if (uiMenu.ActionMenu == menuaction.none)
                {
                    hoverselected = Selection.SelectGeometry(this, scene, cam);
                    uiMenu.Selection = Selection;
                    uiMenu.RootNode = RootNode;
                }

                if (Context.Input.GetMouseButtonPress(Urho3DNet.MouseButton.MousebLeft))
                {
                    touraroundboxes(Selection.SelectedModel);
                }

                //invoke any actions in the list
                if (Actions.Count > 0)
                {
                    var runningActions = Actions.ToList();
                    Actions.Clear();
                    for (int i = 0; i < runningActions.Count; i++)
                    {
                        runningActions[i].Invoke();
                    }
                }

                if (Context.Input.GetMouseButtonClick(Urho3DNet.MouseButton.MousebRight))
                {
                    if (Selection.SelectedModel != null)
                    {
                        uiMenu.ActionMenu = menuaction.ObjectContext;
                    }
                }

                if (ImGuiNet.ImGui.Button("Message"))
                {
                    //  string imp = system.LoadFile("Json|*.json");
                    var imp = system.LoadFiles("Json|*.json");
                    //if (string.IsNullOrEmpty(imp)) return;

                    if (imp == null) return;
                    var jsonFiles = imp.ToList();
                    DrawGeometryFromRevit(jsonFiles);
                }
                if (ImGuiNet.ImGui.Button("Generate Boxes"))
                {
                    new Rbfx_RandomBoxes(RootNode);
                }
                if (ImGuiNet.ImGui.Button("Remove All Boxes"))
                {
                    while (true)
                    {
                        var nodes = RootNode.GetChild("Boxes", true);
                        if (nodes == null) break;
                        nodes.Remove();
                    }
                }
                if (ImGuiNet.ImGui.Button("Draw"))
                {
                    new Draw().DrawRectangle();
                }
                DisplayInfoText(hoverselected);
            });
        }

        public void RunOnce()
        {
            Task.Run(async () =>
            {
                while (!Engine_Tcp.Started)
                {
                    await Task.Delay(100);
                }
                var js = new JStruct();
                js.JsMessage = "OK";
                Engine_Tcp.SendRequestToClient(js);
            });
        }

        private void InitializeTcpConnection()
        {
            if (Engine_Tcp.Started == false)
            {
                new Engine_Tcp();
            }
        }

        private void SetupScene()
        {
            // Scene
            scene = new Scene(Context);
            var octree = scene.CreateComponent<Octree>();
            scene.CreateComponent<DebugRenderer>();
            SetupCameranViewPort();

            var eng_Zone = new Engn_Zone_Test(scene);
            eng_Zone.Rbfx_Zone.FogStart = 100f;

            new Engn_WirePlane(scene);
            SetupLight();

            RootNode = scene.CreateChild("root");

            uiMenu = new UIMenu(RootNode, Selection);

            SetupInfoWindow();
        }

        private void SetupLight()
        {
            lightNode = cam.CameraNode.CreateChild("Light");
            lightNode.SetTemporary(true);
            var l = lightNode.CreateComponent<Light>();
            lightNode.Position = (new Vector3(2, 1, 0));
            l.Range = 200f;
            lightNode.LookAt(Vector3.Zero);
            l.LightType = LightType.LightPoint;
            l.CastShadows = true;            
            l.ShadowBias = new BiasParameters(0.00025f, 0.5f);
            // Set cascade splits at 10, 50 and 200 world units, fade shadows out at 80% of maximum shadow distance
            l.ShadowCascade = new CascadeParameters(10.0f, 50.0f, 200.0f, 0.0f, 0.8f);
        }

        private void SetupCameranViewPort()
        {
            //Camera-Viewport
            cam = new Engn_Camera(scene);
            cam.CameraNode.Position = (new Vector3(0, 2, -20));
            cam.LookAt(Vector3.Zero);
            viewport = new Viewport(Context);
            viewport.Scene = scene;
            viewport.Camera = cam.camera;
            Context.Renderer.SetViewport(0, viewport);

            //todo: not sure how setclipplane works
            //Graphics.SetClipPlane(true, Plane.Up, new Matrix3x4(Vector3.Zero, Quaternion.IDENTITY, 200f));
            cam.camera.ClipPlane = new Plane(Vector3.Up, new Vector3(0, 1, 0));
        }

        private void SetupResourcePaths()
        {
            //SetupResources Location

            Context.Cache.AddResourceDir(@"c:\windows\fonts");
            Context.Cache.AddResourceDir(@"D:\Revit_API\Downloaded_Library\Source\rbfx\bin\CoreData");
            Context.UI.Cursor = new Urho3DNet.Cursor(Context);
        }

        public void DrawGeometryFromRevit(List<string> jsonFiles)
        {
            foreach (var jsonFilePath in jsonFiles)
            {
                var geo = new Serializable.Engine_Geometry().JDeserializemyData(System.IO.File.ReadAllText(jsonFilePath));
                geo.FileName = jsonFilePath;
                CreateCustomShape2(geo);
            }
        }

        public void CreateCustomShape2(Serializable.Engine_Geometry geom)
        {
            Logger.Log($"Generating Geometry [{geom.Name}]");

            if (geom.Engine_Faces.Count == 0)
            {
                Logger.Log($"Geometry: [{geom.Name}] has no faces", "", Logger.ErrorType.Warrning);
                return;
            }

            var geonode = RootNode.CreateChild(geom.Name);
            geonode.Scale(geom.Flip.ToVec3());
            if (geom.Rotation != null)
                geonode.Rotate(new Quaternion(geom.Rotation.ToVec3()));
            float scaleValue = (float)DynConstants.FeettoMeter;
            geonode.Scale(new Vector3(scaleValue, scaleValue, scaleValue));

            geom.GenerateNormals();
            if (!geom.GenerateTangents())
            {
                var failChild = geonode.CreateChild($"{geom.Name} Failed");
                failChild.Position = geom.Position.ToVec3();
                var model = Cache.GetResource<Model>("Models/Box.mdl");
                var stcomp = failChild.CreateComponent<StaticModel>();
                stcomp.SetModel(model);
                return;
            }

            var faceColorGroups = geom.Engine_Faces.GroupBy(o => o.FaceColor.ToString());
            string dir = System.IO.Path.GetDirectoryName(geom.FileName);
            var files = System.IO.Directory.GetFiles(dir).ToList();
            foreach (var faceColorGroup in faceColorGroups)
            {

                var facechild = geonode.CreateChild("Face_Color");
                Material mat = null;

                var faceColor = faceColorGroup.ElementAt(0).FaceColor;

                if (faceColor.L != 1)
                {
                    mat = Material_Ext.TransParentMaterial(faceColor.ToColor());
                }
                else
                {
                    mat = Material_Ext.ColoredMaterial(faceColor.ToColor());
                }
                mat.CullMode = geom.GeoCullModel;
#if false
                var faceColor = faceColorGroup.ElementAt(0).FaceColor.ToColor();

                // mat = RootNode.Context.Cache.GetResource<Material>("Materials/Stone.xml");
                if (!files.Any(o => o.Contains(faceColor.ToString())))
                {
                    if (faceColor.ToVector4().W != 1)
                    {
                        mat = Material_Ext.TransParentMaterial(faceColor);
                    }
                    else
                    {
                        mat = Material_Ext.ColoredMaterial(faceColor);
                    }
                    mat.CullMode = geom.GeoCullModel;

                    var isSaved = mat.SaveFile(dir + "\\" + mat.Name);
                }

                mat = Cache.GetResource<Material>(dir + "\\" + faceColor.ToString() + ".xml"); 
#endif

                var cus = facechild.CreateComponent<CustomNodeComponent>();
                cus.OriginalMaterial = mat;

                var cusGeo = facechild.CreateComponent<CustomGeometry>();
                cusGeo.CastShadows = true;
                
                cusGeo.BeginGeometry(0, PrimitiveType.TriangleList);
                cusGeo.SetMaterial(mat);
                
                Logger.Log("Begin Geometry");
                foreach (var face in faceColorGroup)
                {
                    var triangles = face.EngTriangles;
                    var trianglesCount = face.EngTriangles.Count;
                    for (int triIndex = 0; triIndex < trianglesCount; triIndex++)
                    {
                        var triangle = triangles[triIndex];
                        var triPoints = triangle.GetPoints();
                        foreach (var engpoint in triPoints)
                        {
                            cusGeo.DefineVertex(engpoint.EngPosition.ToVec3());
                            cusGeo.DefineNormal(engpoint.EngNormal.ToVec3());
                            cusGeo.DefineTexCoord(engpoint.EngTexture.ToVec2());
                            cusGeo.DefineTangent(engpoint.EngTangent.ToVec4());
                        }
                    }
                }
                cusGeo.Commit();
                Logger.Log("End Geometry");
            }
        }

        private Window infowindow = null;

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

        private void DisplayInfoText(Drawable hoverselected)
        {
            if (hoverselected != null)
            {
                var infotext = infowindow.GetChild("InfoText") as Text;
                infowindow.SetVisible(true);
                infowindow.Position = Context.Input.MousePosition + new IntVector2(10, 10);
                string todisplay = hoverselected.Node.Name + "\r\n" + hoverselected.WorldBoundingBox.Center;

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

        private string objname = "wall";

        private void touraroundboxes(Drawable model)
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

        private void onUnSelect()
        {
            if (Selection.SelectedModel == null)
            {
                uiMenu.ActionMenu = menuaction.none;
            }
        }
    }
}