using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeometryGym.Ifc;
namespace InSitU.Import
{
	partial class IFC
	{

		class IfcBuilding
		{
			public static void ExtractBuilding(DatabaseIfc db)
			{
				var builds = db.Where(o =>
				//o is IfcBuildingElement
				//||
				//o is IfcBuildingElementPartType 
				//||
				o is IfcBuildingElementProxy
				);


			}

		}
	}
}
