using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Urho3DNet;

namespace EngineViewer.Actions._3D.Animations
{
	
	class Base_CustomLogicComponent :LogicComponent
	{
		public Base_CustomLogicComponent(Context context) : base(context)
		{
		}

		public int Duration { get; set; } = 1;
		public Action PostUpdate { get; set; } = () => { };
	}
}
