using EngineViewer.Actions._3D.RbfxUtility;
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
                //Select element
                Vector3 hitposition;
                Drawable model;

                Rbfx_Utility.Raycast(scene, cam, 10000, out hitposition, out model);
                if (app.Context.Input.GetMouseButtonPress(MouseButton.MousebLeft))
                {
                    SelectedModel = model;
                    return HiLightSelected(model);
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
            if (model != null)
            {
                ((dynamic)SelectedModel).SetMaterial(Originalmat);
                Originalmat = null;
                model = null;
            }

            if (model != null)
            {

                Originalmat = ((dynamic)SelectedModel).GetMaterial();
                ((dynamic)model).SetMaterial(Material_Ext.SelectedMaterial);
                return model;

            }

            return null;
        }
    }
}