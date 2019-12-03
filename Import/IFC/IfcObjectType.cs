using GeometryGym.Ifc;
using InSitU.DataBase;
using InSitU.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utility.IO;

namespace InSitU.Import
{

	partial class IFC
	{
		class IfcObjectType
		{
			public static Core ExtractType(IfcElement ifcele)
			{
				IfcTypeObject ifctype = ifcele.IsTypedBy.RelatingType;
				FinishLib flib = new FinishLib();
				flib.FKey_Category = Category.GetorAddCategory(ifctype.Name.Split(':')[0]).GuId;
				ExtractTypicalProperties(ifctype, flib);
				flib.Name = ifctype.Name.Split(':')[1];

				var d = new Data("PreDefined ObjectType", ifctype.GetPredefinedType());
				flib.Additional_Info.Add(d);

				flib.Additional_Info.ForEach(dx =>
				{
					dx.SetFkeyParent(flib);
				});

				return flib;
			}

			public static void ExtractTypicalProperties(IfcObjectDefinition ifcele, Core Model)
			{
				Model.IfcId = ifcele.Guid;
				Model.Name = ifcele.Name;

				try
				{
					//get element ID
					Model.ElementId = int.Parse(((IfcElement) ifcele).Tag);
				}
				catch (Exception)
				{
					try
					{
						Model.ElementId = int.Parse(((IfcWallType)ifcele).Tag);

					}
					catch (Exception)
					{


						//object doesn't have Tag Property
						Debug.WriteLine(ifcele.Name + "Doesn't have Tag Property\r\n" + ifcele.StringSTEP());
					}
				}

			}
		}
	}
}
