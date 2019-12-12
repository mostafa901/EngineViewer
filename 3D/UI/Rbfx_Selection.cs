using EngineViewer.Actions._3D.Models;
using EngineViewer.Actions._3D.RbfxUtility;
using ImGuiNet;
using Urho3DNet;

namespace EngineViewer.Actions._3D.UI
{
    public class Engn_Selection
    {
        private Material Originalmat;
        public Drawable SelectedModel;

        public Drawable SelectGeometry(Application app, Scene scene, Engn_Camera cam)
        {
            if (app.Context.UI.Cursor.IsVisible())
            {
                if (ImGui.IsAnyItemHovered()) return null;
                //Select element
                Vector3 hitposition;
                Drawable model;

                Rbfx_Utility.Raycast(scene, cam, 10000, out hitposition, out model);
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

        private Drawable HiLightSelected(Drawable model)
        {
            if (SelectedModel != null)
            {
                ((dynamic)SelectedModel).SetMaterial(Originalmat);
            }

            if (model != null)
            {
                Originalmat = ((dynamic)model).GetMaterial();
                ((dynamic)model).SetMaterial(Material_Ext.SelectedMaterial);
                return model;
            }

            return null;
        }
    }
}