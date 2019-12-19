using EngineViewer.Actions._3D.Models;
using EngineViewer.Actions._3D.RbfxUtility;
using ImGuiNet;
using System.Collections.Generic;
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