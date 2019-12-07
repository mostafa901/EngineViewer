using EngineViewer._3D.TCP;
using EngineViewer._3D.Test;
using EngineViewer._3D.Utility;
using EngineViewer.Actions._3D.Animations;
using EngineViewer.Actions._3D.Models;
using EngineViewer.Actions._3D.RbfxUtility;
using EngineViewer.Actions._3D.UI;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Urho3DNet;
using Utility.IO;
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
            engineParameters_[Urho3D.EpWindowWidth] = 1200;
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
        }

        private void SubscribeEvents()
        {
            SubscribeToEvent(E.Update, args =>
            {
                uiMenu.RenderMenu();

                //what to do if selection is nothing
                onUnSelect();

                //camera movement
                cam.FirstPersonCamera(this, Context.Time.TimeStep, 10, Selection?.SelectedModel?.Node);

                //CheckSelection
                StaticModel hoverselected = null;
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
                    // var msg = new MessageBox(Context, "hello", "Title message");

                    //  string imp = @"D:\Program Files\Autodesk\Revit 2018\Testvertex_trans.json";
                    //  var verte = new List<List<string>>().JDeserializemyData(System.IO.File.ReadAllText(imp));

                    string imp = @"D:\Program Files\Autodesk\Revit 2018\test.json";
                    var verte = new List<Serializable.Engine_Geometry>().JDeserializemyData(System.IO.File.ReadAllText(imp));
                    CreateCustomShape2(verte);
                }

                DisplayInfoText(hoverselected);
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
            scene.CreateComponent<Octree>();

            SetupCameranViewPort();

            var eng_Zone = new Engn_Zone_Test(scene);
            eng_Zone.Rbfx_Zone.FogStart = 100f;

            new Engn_WirePlane(scene);
            SetupLight();

            RootNode = scene.CreateChild("root");

            new Rbfx_RandomBoxes(RootNode);

            uiMenu = new UIMenu(RootNode, Selection);

            SetupInfoWindow();
        }

        private void SetupLight()
        {
            light = cam.CameraNode.CreateChild("Light");
            var l = light.CreateComponent<Light>();
            light.Position = (new Vector3(0, 50, -1));
            l.Range = 100f;
            light.LookAt(Vector3.Zero);
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

        /*
          var vs = vb.GetUnpackedData().ToList();
            List<Vector3> poss = new List<Vector3>();
            List<Vector3> tangs = new List<Vector3>();
            List<Vector2> tex = new List<Vector2>();
            for (int i = 0; i < vs.Count; i += 3)
            {
                var v = vs[i];
                var vp = new Vector3(v.X, v.Y, v.Z);
                poss.Add(vp);

                 v = vs[i+2];
                var vt = new Vector2(v.X, v.Y);
                tex.Add(vt);

                if (poss.Count == 3)
                {
                    tangs.Add(GetTangent(poss.ToArray(), tex.ToArray()));
                    tex.Clear();
                    poss.Clear();
                }
            }
            int p = 3;

            foreach (var t in tangs)
            {
                vs.Insert(p, new Vector4(t,0));
               p += 3;
            }

            vb.SetSize(numVertices, VertexMask.MaskPosition | VertexMask.MaskNormal | VertexMask.MaskTexcoord1 | VertexMask.MaskTangent, false);
            var s = vs.Select(o => o.ToString());
            var sj = string.Join(" ", s);
            var ss = sj.Split(' ');
            List<float> fls = new List<float>();
            for (int i = 0; i < ss.Length; i++)
            {
                fls.Add(float.Parse(ss[i]));
            }
             vb.SetData(fls.ToArray());

         * */

        private void SetupResourcePaths()
        {
            //SetupResources Location
            string ResDir = $@"{Path.GetDirectoryName(Assembly.GetExecutingAssembly().EscapedCodeBase)}/Resources/3D";
            ResDir.Log("CurrentPath");
            Context.Cache.AddResourceDir(ResDir);
            Context.Cache.AddResourceDir(@"c:\windows\fonts");
            Context.Cache.AddResourceDir(@"D:\Revit_API\Downloaded_Library\Source\rbfx\bin\CoreData");
            Context.UI.Cursor = new Urho3DNet.Cursor(Context);
            
        }

        public void CreateCustomShape(List<List<string>> geos)
        {
            var geonode = RootNode.CreateChild("GeoNode");
            var mat = new Material(Context);
            // mat = Context.Cache.GetResource<Material>("Materials/Stone.xml");
            mat.SetShaderParameter("MatDiffColor", Color.Red);

            for (uint g = 0; g < geos.Count; g++)
            {
                var cusGeom = new CustomGeometry(Context);
                //var geom = new CustomGeometry(Context);
                cusGeom.SetMaterial(mat);
                cusGeom.BeginGeometry(0, PrimitiveType.TriangleList);

                var verte = geos[(int)g];
                Vector3 pos = new Vector3();
                for (uint i = 0; i < verte.Count; i++)
                {
                    var v = verte[(int)i];
                    var ve = v.Replace("(", "").Replace(")", "");
                    var vs = ve.Split(',');
                    float x = float.Parse(vs[0]);
                    float y = float.Parse(vs[1]);
                    float z = float.Parse(vs[2]);
                    if (vs.Length == 3)
                    {
                        pos = new Vector3(x, y, z);
                        continue;
                    }

                    float xn = float.Parse(vs[3]);
                    float yn = float.Parse(vs[4]);
                    float zn = float.Parse(vs[5]);
                    float xt = float.Parse(vs[6]);
                    float yt = float.Parse(vs[7]);
                    cusGeom.DefineVertex(new Vector3(x, y, z));
                    cusGeom.DefineTexCoord(new Vector2(xt, yt));
                    cusGeom.DefineNormal(new Vector3(xn, yn, zn));
                }

                cusGeom.Commit();
                geonode.Rotate(new Quaternion(90, 0.0f, 0));
                var modelnode = new Node(Context);
                modelnode.Position = pos;
                modelnode.Name = "Geom";
                geonode.AddChild(modelnode);

                var geo = cusGeom.GetLodGeometry(0, 1);

                StaticModel compStaticModel = modelnode.CreateComponent<StaticModel>();
                Model model = new Model(Context);
                model.NumGeometries = 1;
                model.SetGeometry(0, 0, geo);
                var vcs = cusGeom.Vertices[0].Select(o => o.Position).ToArray();
                model.BoundingBox = new BoundingBox(vcs);

                var vbuff = geo.VertexBuffers[0].GetUnpackedData();
                VertexBuffer vbtest = new VertexBuffer(Context);
                vbtest.SetShadowed(true);

                List<short> ind = new List<short>();

                short c = 0;
                for (int i = 0; i < vbuff.Count; i += 3)
                {
                    ind.Add(c++);
                }
                IndexBuffer ib = new IndexBuffer(Context);
                ib.SetShadowed(true);
                ib.SetSize((uint)ind.Count, false);
                ib.SetData(ind.ToArray());

                VertexBufferRefList vblist = new VertexBufferRefList();
                IndexBufferRefList iblist = new IndexBufferRefList();

                vblist.Add(geo.VertexBuffers[0]);
                iblist.Add(ib);

                UIntArray morphRangeStarts = new UIntArray();
                UIntArray morphRangeCounts = new UIntArray();
                morphRangeStarts.Add(0);
                morphRangeCounts.Add(0);

                model.SetVertexBuffers(vblist, morphRangeStarts, morphRangeCounts);
                model.IndexBuffers = iblist;

                compStaticModel.SetModel(model);
                compStaticModel.SetMaterial(mat);
            }

            cam.LookAt(geonode.GetComponent<StaticModel>(true).WorldBoundingBox.Center);
        }

        public void CreateCustomShape2(List<Serializable.Engine_Geometry> geos)
        {
            var geonode = RootNode.CreateChild("GeoNode");
            geonode.Rotate(new Quaternion(-90, 0.0f, 0));

            for (uint g = 0; g < geos.Count; g++)
            {
                var cusGeom = new CustomGeometry(Context);
                //var geom = new CustomGeometry(Context);
                Material mat = null;

                cusGeom.BeginGeometry(0, PrimitiveType.TriangleList);

                var geom = geos[(int)g];
                Vector3 pos = new Vector3(geom.Position.X, geom.Position.Y, geom.Position.Z);

                foreach (var gp in geom.Engine_Points)
                {
                    switch (gp.EngPointType)
                    {
                        case Serializable.Engine_Geometry.PointType.Color:
                            {
                                var clor = new Color(gp.X, gp.Y, gp.Z, gp.L);
                                cusGeom.DefineColor(clor);
                                mat = new Material(Context);
                                mat.SetShaderParameter("MatDiffColor", clor);
                            }
                            break;

                        case Serializable.Engine_Geometry.PointType.Vertex:
                            {
                                var v = new Vector3(gp.X, gp.Y, gp.Z);
                                cusGeom.DefineVertex(v);
                            }
                            break;

                        case Serializable.Engine_Geometry.PointType.Normal:
                            cusGeom.DefineNormal(new Vector3(gp.X, gp.Y, gp.Z));
                            break;
                           
                        case Serializable.Engine_Geometry.PointType.Texture:
                            cusGeom.DefineTexCoord(new Vector2(gp.X, gp.Y));
                            break;

                        case Serializable.Engine_Geometry.PointType.Position:
                            break;
                        case Serializable.Engine_Geometry.PointType.Tangent:
                            cusGeom.DefineTangent(new Vector4(gp.X, gp.Y, gp.Z, gp.L));

                            break;
                        default:
                            break;
                    }
                }

                cusGeom.Commit();
                

                var modelnode = new Node(Context);
                geonode.AddChild(modelnode);
                modelnode.Position = pos;
                modelnode.Name = geom.Name;

                var geo = cusGeom.GetLodGeometry(0, 1);

                Model model = new Model(Context);
                model.NumGeometries = 1;
                model.SetGeometry(0, 0, geo);
                var vcs = cusGeom.Vertices[0].Select(o => o.Position).ToArray();
                model.BoundingBox = new BoundingBox(vcs);

                var vbuff = geo.VertexBuffers[0].GetUnpackedData();
                VertexBuffer vbtest = new VertexBuffer(Context);
                vbtest.SetShadowed(true);

                List<short> ind = new List<short>();

                short c = 0;
                for (int i = 0; i < vbuff.Count; i += 3)
                {
                    ind.Add(c++);
                }

                IndexBuffer ib = new IndexBuffer(Context);
                ib.SetShadowed(true);
                ib.SetSize((uint)ind.Count, false);
                ib.SetData(ind.ToArray());

                VertexBufferRefList vblist = new VertexBufferRefList();
                IndexBufferRefList iblist = new IndexBufferRefList();

                vblist.Add(geo.VertexBuffers[0]);
                iblist.Add(ib);

                UIntArray morphRangeStarts = new UIntArray();
                UIntArray morphRangeCounts = new UIntArray();
                morphRangeStarts.Add(0);
                morphRangeCounts.Add(0);

                model.SetVertexBuffers(vblist, morphRangeStarts, morphRangeCounts);
                model.IndexBuffers = iblist;

                StaticModel compStaticModel = modelnode.CreateComponent<StaticModel>();
                compStaticModel.SetModel(model);
                if (mat != null) compStaticModel.SetMaterial(mat);
            }
            cam.LookAt(geonode.GetComponent<StaticModel>(true).WorldBoundingBox.Center);
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

        private string objname = "wall";

        private void touraroundboxes(StaticModel model)
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