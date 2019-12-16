using System;
using Urho3DNet;
using EngineViewer.Actions._3D.RbfxUtility;

namespace EngineViewer.Actions._3D.Models
{
    internal class Engn_WirePlan
    {
        public WirePlan wireplan;

        public Engn_WirePlan(Scene scene)
        {
            wireplan = new WirePlan(scene.Context);
            wireplan.Color = Color.Gray;
            wireplan.SetTemporary(true);
            scene.AddComponent(wireplan, wireplan.ID, CreateMode.Local);

        }
    }

    public class WirePlan: Component
    {
        private CustomGeometry geom;
        private int size = 50;
        private float scale = 1f;
        private Color color = new Color(1f, 0.0f, 0.7f);

        public WirePlan(Context context) : base(context)
        {
            SetTemporary(true);
        }

        public int Size
        {
            get { return size; }
            set
            {
                size = value;
                Reload();
            }
        }

        public float Scale
        {
            get { return scale; }
            set
            {
                scale = value;
                Reload();
            }
        }

        public Color Color
        {
            get { return color; }
            set
            {
                color = value;
                Reload();
            }
        }

        protected override void OnNodeSet(Node node)
        {
            base.OnNodeSet(node);
            Reload();
        }

        private void Reload()
        {
            if (geom != null && !geom.IsZoneDirty())
                geom.Remove();

            if (Node == null || Node.IsDirty())
                return;

            geom = Node.CreateComponent<CustomGeometry>();
            geom.SetTemporary(true);
            geom.BeginGeometry(0, PrimitiveType.LineList);

            geom.SetMaterial(Material_Ext.noLitFromColor(color, true));

            var halfSize = Size / 2;
            for (int i = -halfSize; i <= halfSize; i++)
            {
                if (i % 5 == 0)
                {
                    continue;
                }

                //x
                geom.DefineVertex(new Vector3(i, 0, -halfSize) * Scale);
                geom.DefineColor(Color);
                geom.DefineVertex(new Vector3(i, 0, halfSize) * Scale);
                geom.DefineColor(Color);

                //z
                geom.DefineVertex(new Vector3(-halfSize, 0, i) * Scale);
                geom.DefineColor(Color);
                geom.DefineVertex(new Vector3(halfSize, 0, i) * Scale);
                geom.DefineColor(Color);
            }

            geom.Commit();

            geom = Node.CreateComponent<CustomGeometry>();
            geom.SetTemporary(true);

            geom.BeginGeometry(0, PrimitiveType.LineList);

            geom.SetMaterial(0, Material_Ext.noLitFromColor(Color.White, true));

            for (int i = -halfSize; i <= halfSize; i++)
            {
                if (i % 5 != 0)
                {
                    continue;
                }

                //x
                geom.DefineVertex(new Vector3(i, 0, -halfSize) * Scale);
                geom.DefineColor(Color);
                geom.DefineVertex(new Vector3(i, 0, halfSize) * Scale);
                geom.DefineColor(Color);

                //z
                geom.DefineVertex(new Vector3(-halfSize, 0, i) * Scale);
                geom.DefineColor(Color);
                geom.DefineVertex(new Vector3(halfSize, 0, i) * Scale);
                geom.DefineColor(Color);
            }

            geom.Commit();
        }
    }
}