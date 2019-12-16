using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Urho3DNet;

namespace EngineViewer.Actions._3D.RbfxUtility
{
    public static class Material_Ext
    {
        public static Material noLitFromColor(Color color, bool unlit = true)
        {
            var material = new Material(DefaultScene.scene.Context);
            var cache = DefaultScene.scene.Context.Cache;
            float tolerance = 0.001f;
            if (unlit)
                material.SetTechnique(0, Math.Abs(color.ToVector4().W - 1f) < tolerance ? cache.GetResource<Technique>("Textures/NoTextureUnlit.xml") : cache.GetResource<Technique>("Textures/NoTextureUnlitAlpha.xml"), MaterialQuality.QualityMedium, 1);
            else
                material.SetTechnique(0, Math.Abs(color.ToVector4().W - 1) < tolerance ? cache.GetResource<Technique>("Textures/NoTexture.xml") : cache.GetResource<Technique>("Textures/NoTextureAlpha.xml"), MaterialQuality.QualityMedium, 1);

            material.SetShaderParameter("MatDiffColor", color);

            return material;
        }

        public static Material TransParentMaterial(Color color)
        {
            var mat = new Material(DefaultScene.Instance.Context);
            mat.SetTechnique(0, mat.Cache.GetResource<Technique>("Techniques/NoTextureAlpha.xml"));
            mat.SetShaderParameter("MatDiffColor", color);
            mat.SetShaderParameter("MatSpecColor", new Vector4(1, 1, 1, 32));
            mat.Name = color.ToString() + ".xml";

            return mat;
        }

        public static Material ColoredMaterial(Color color)
        {
            var mat = DefaultScene.scene.Context.Cache.GetResource<Material>("Materials/Colored.xml").Clone();
            mat.SetShaderParameter("MatDiffColor", color);
            mat.Name = color.ToString() + ".xml";
            return mat;
        }

        #region SelectedMaterial

        private static Material _SelectedMaterial;
        public static Material SelectedMaterial
        {
            get
            {
                if (_SelectedMaterial == null && DefaultScene.scene != null)
                {
                    _SelectedMaterial = DefaultScene.scene.Context.Cache.GetResource<Material>("Materials/Colored.xml");
                }
                return _SelectedMaterial;
            }
            set { _SelectedMaterial = value; }
        }

        #endregion SelectedMaterial
    }
}