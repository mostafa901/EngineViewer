﻿using ImGuiNet;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Urho3DNet;
using Application = Urho3DNet.Application;
using Color = Urho3DNet.Color;

namespace EngineViewer.Actions._3D.RbfxUtility
{

	[ObjectFactory]

	public class CustomNodeComponent : Component
	{
		public CustomNodeComponent(Context context) : base(context)
		{
			Notes = "testing";
            SetTemporary(true);
		}


        protected override void OnNodeSet(Node node)
        {
            base.OnNodeSet(node);
            Reload();
        }
       public CustomGeometry geom;
        public Action ReloadAction = ()=> { };
        public virtual void Reload()
        {
            if (geom != null && !geom.IsZoneDirty())
                geom.Remove();

            geom = Node.CreateComponent<CustomGeometry>();
            ReloadAction();
            Debug.WriteLine("Reloaded");
        }

        [SerializeField]
		public Vector3 OriginalPosition { get; set; } = new Vector3();

		[SerializeField]
		public string Notes { get; set; } = "";


		[SerializeField]
		public string Vmap { get; set; } = "";

		public Material OriginalMaterial { get; set; }
		public List<data> Info { get; set; }

	}

	public class data
	{
		public string Key { get; set; }
		public string value { get; set; }
	}
}
