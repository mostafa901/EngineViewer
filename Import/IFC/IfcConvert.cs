using System.Collections.Generic;
using GeometryGym.Ifc;
using Utility.IO;

namespace InSitU.Import
{ 
	public partial class IFC 
	{
		public class IfcConvert
		{
			public static Dictionary<string, IfcProperty> ToIfcPropertyDic(System.Collections.ObjectModel.ReadOnlyDictionary<string, IfcProperty> properties)
			{
				Dictionary<string, IfcProperty> dic = new Dictionary<string, IfcProperty>();
				properties.Keys.ForEach(k =>
				{
					dic.Add(k, properties[k]);
				});
				return dic;

			}
		}

	}

}
