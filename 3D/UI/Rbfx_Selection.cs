using EngineViewer.Actions._3D.Models;
using EngineViewer.Actions._3D.RbfxUtility;
using ImGuiNet;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Urho3DNet;

namespace EngineViewer.Actions._3D.UI
{
    public class Engn_Selection
    {
        private Material Originalmat;
        public Drawable SelectedModel;
        public List<Drawable> VoidableSelection;
        public Vector3 HitPosition;
        public Engn_Selection()
        {
            VoidableSelection = new List<Drawable>();
        }
        public Drawable SelectGeometry(Application app, Scene scene, Engn_Camera cam)
        {
            if (app.Context.UI.Cursor.IsVisible())
            {
                if (ImGui.IsAnyItemHovered())
                {
                    return null;
                }

                //Select element
                Drawable model;

                Rbfx_Utility.Raycast(scene, cam, float.MaxValue, out HitPosition, out model);
                if (app.Context.Input.GetMouseButtonPress(MouseButton.MousebLeft))
                {
                    HiLightSelected(model);
                    SelectedModel = model;
                    return SelectedModel;
                }
                else
                {
                    return model as Drawable;
                }
            }

            return null;
        }

        //todo:Issue Imgui doesn't accept array.
        public bool ShowListOfGeometries = false;
        int currentSelectedItem;
        IEnumerable<Drawable> drawables;
        [MarshalAs(UnmanagedType.SafeArray)]
        unsafe string[] drawableNames;
        unsafe public void ShowGeometryList(Node root, bool update = false)
        {
            if (drawables == null || update == true || drawableNames.Length == 0)
            {
                drawables = root.GetComponents<StaticModel>(true).Cast<Drawable>();
                drawableNames = drawables.Select(o => o.ID.ToString()).ToArray();
            }

            if (ShowListOfGeometries)
            {
                if (ImGui.ListBox("Geometries", ref currentSelectedItem, drawableNames, drawableNames.Length))
                {
                    HiLightSelected(drawables.ElementAt(currentSelectedItem));
                }
            }

        }

        ListView viewlist;
        public void ShowGeometryViewList(Node root, bool update = false)
        {
            if (drawables == null || update == true || drawableNames.Length == 0)
            {
                drawables = root.GetComponents<StaticModel>(true).Cast<Drawable>();
                drawableNames = drawables.Select(o => o.ID.ToString()).ToArray();
            }

            if (ShowListOfGeometries)
            {
                if (viewlist == null)
                {
                    viewlist = new ListView(root.Context);
                    viewlist.SetStyleAuto();
                    viewlist.Height = 380;
                    viewlist.Width = 180;                    
                    viewlist.SetColor(Color.Gray);
                    viewlist.HorizontalAlignment = HorizontalAlignment.HaCenter;
                    viewlist.VerticalAlignment = VerticalAlignment.VaCenter;
                    Window win = new Window(root.Context);
                    win.SetStyleAuto();
                    win.SetColor(Color.Gray);
                    win.HorizontalAlignment = HorizontalAlignment.HaCenter;
                    win.VerticalAlignment = VerticalAlignment.VaCenter;
                    win.Width = 200;
                    win.Height = 400;
                    win.AddChild(viewlist);
                    root.Context.UI.Root.AddChild(win);
                    
                }
             //   if (drawables == null || update == true || drawableNames.Length == 0)
                {
                  //  viewlist.RemoveAllItems();
                    foreach (var item in drawableNames)
                    {
                        var uiItem = new Text(root.Context);
                        uiItem.Height = 20;
                        uiItem.SetStyleAuto();
                        uiItem.SetFont("Arial.ttf", 12);
                        uiItem.SetColor(Color.Green);
                        uiItem.SetText(item);
                        viewlist.AddItem(uiItem);                        
                    }
                }
                if (viewlist.SelectedItem != null)
                {
                    var selected = drawables.ElementAt(System.Array.IndexOf(drawableNames, viewlist.SelectedItem));
                    HiLightSelected(selected);
                }
            }

        }

        public void SetOriginalMaterial(Material mat)
        {
            Originalmat = mat;
        }

        public Drawable HiLightSelected(Drawable model)
        {
            if (SelectedModel != null)
            {
                ((dynamic)SelectedModel).SetMaterial(Originalmat);
            }

            if (model != null)
            {
                Originalmat = ((dynamic)model).GetMaterial();
                var mat = Material_Ext.SelectedMaterial;
                mat.CullMode = Originalmat.CullMode;
                ((dynamic)model).SetMaterial(mat);
                return model;
            }

            return null;
        }
    }
}