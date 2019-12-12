
using EngineViewer.Actions._3D.RbfxUtility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Urho3DNet;

namespace EngineViewer.Actions._3D.UI
{
public	class Engn_Selection
	{
		Material Originalmat;
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
					return HiLightSelected(model);
				}
				else
				{
					return model as Drawable;
				}
			}

			return null;
		}

        public void  SetOriginalMaterial(Material mat)
        {
            Originalmat = mat;
        }

		Drawable HiLightSelected(Drawable model)
		{
			if (SelectedModel != null)
			{

				((dynamic)SelectedModel).SetMaterial(Originalmat);
				Originalmat = null;
				SelectedModel = null;
			}

			if (model != null)
			{
				SelectedModel = model as StaticModel;
				if (SelectedModel != null)
				{
					Originalmat = ((dynamic)SelectedModel).GetMaterial();

                    ((dynamic)SelectedModel).SetMaterial(Material_Ext.SelectedMaterial);

					return SelectedModel;
				}
			}

			return null;
		}


	}
}
