using System.Collections.Generic;
using System.Linq;
using GeometryGym.Ifc;
using InSitU.DataBase;
using InSitU.Model;
using SQLiteNetExtensions.Extensions;
using Utility.IO;

namespace InSitU.Import
{

	public partial class IFC
	{
		class IfcData
		{
			public static List<Data> ExtractProps(Dictionary<string, IfcProperty> props, Core model)
			{
				List<Data> datas = new List<Data>();
				foreach (var key in props.Keys)
				{
					Data d = new Data();

					if (props[key] is IfcPropertySingleValue)
					{
						var p = props[key] as IfcPropertySingleValue;
						d.Name = p.Name;
						d.Value = p.NominalValue.ValueString;
					}

					if (props[key] is IfcPropertyBoundedValue)
					{
						//todo: IfcPropertyBoundedValue
					}

					if (props[key] is IfcPropertyEnumeratedValue)
					{
						//todo: IfcPropertyEnumeratedValue
					}
					if (props[key] is IfcPropertyListValue)
					{
						//todo: IfcPropertyListValue
					}
					if (props[key] is IfcPropertyTableValue)
					{
						//todo: IfcPropertyTableValue
					}
					d.SetFkeyParent(model);
					datas.Add(d);
				}

				return datas;
			}

			

			public static void InsertOrReplaceAll(IEnumerable<Data> data)
			{
#if !IFCTest
				data.ForEach(d =>
				{
					d.InsertOrReplace();
				});
#endif
			}

			static List<Data> ExtractData(IfcElement ifcele, Core model)
			{
				var props = ((IfcPropertySet)ifcele.IsDefinedBy.First().RelatingPropertyDefinition).HasProperties;
				return IfcData.ExtractProps(props, model);
			}

		}



	}

}
