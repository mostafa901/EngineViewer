using System;
using Urho3DNet;
using EngineViewer.Actions._3D.RbfxUtility;

namespace EngineViewer.Actions._3D.Models
{
    internal class Engn_WirePlan
    {
        public Node wirePlanNode;
        public CustomNodeComponent WireNodeProperties;

        public CustomGeometry geom;
        private int size = 50;
        private float scale = 1f;
        private Color color = new Color(1f, 0.0f, 0.7f);

        public Engn_WirePlan(Scene scene)
        {
            var wireNode = scene.CreateChild("WirePlan");
            wireNode.SetTemporary(true);

            WireNodeProperties = wireNode.CreateComponent<CustomNodeComponent>();
            WireNodeProperties.CanBeSelected = false;

            CreateWirePlan(wireNode);
        }

        public void CreateWirePlan(Node Parent)
        {
            geom = new CustomGeometry(Parent.Context);
            geom.NumGeometries = 2;
            geom.SetTemporary(true);
            geom.BeginGeometry(0, PrimitiveType.LineList);


            var halfSize = Size / 2;
            for (int i = -halfSize; i <= halfSize; i++)
            {
                if (i % 5 == 0)
                {
                    continue;
                }

                //x
                geom.DefineVertex(new Vector3(i, 0, -halfSize) * Scale);
                geom.DefineVertex(new Vector3(i, 0, halfSize) * Scale);

                //z
                geom.DefineVertex(new Vector3(-halfSize, 0, i) * Scale);
                geom.DefineVertex(new Vector3(halfSize, 0, i) * Scale);
            }

            geom.Commit();

            geom.BeginGeometry(1, PrimitiveType.LineList);


            for (int i = -halfSize; i <= halfSize; i++)
            {
                if (i % 5 != 0)
                {
                    continue;
                }

                //x
                geom.DefineVertex(new Vector3(i, 0, -halfSize) * Scale);
                geom.DefineVertex(new Vector3(i, 0, halfSize) * Scale);

                //z
                geom.DefineVertex(new Vector3(-halfSize, 0, i) * Scale);
                geom.DefineVertex(new Vector3(halfSize, 0, i) * Scale);
            }

            geom.Commit();

            var model = new Model(Parent.Context);
            model.NumGeometries = 2;
            model.SetGeometry(0, 0, geom.GetLodGeometry(0, 0));
            model.SetGeometry(1, 0, geom.GetLodGeometry(1, 0));



            var stwire = Parent.CreateComponent<StaticModel>();
            stwire.SetModel(model);
            stwire.SetMaterial(0,Material_Ext.noLitFromColor(Color.Gray, true));
            stwire.SetMaterial(1,Material_Ext.noLitFromColor(Color.White, true));

        }

        public int Size
        {
            get { return size; }
            set
            {
                size = value;
            }
        }

        public float Scale
        {
            get { return scale; }
            set
            {
                scale = value;
            }
        }

        
    }
}