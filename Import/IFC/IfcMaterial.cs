using System.Collections.Generic;
using System.Linq;
using GeometryGym.Ifc;
using InSitU.DataBase;
using Utility.IO;

namespace InSitU.Import
{
	public partial class IFC
	{
		class IfcMaterial
		{
			public static List<MaterialLib> ExtractMaterial(IfcElement ifcele)
			{
				var ifcmats = ifcele.HasAssociations.Where(o => o is IfcRelAssociatesMaterial);
				List<MaterialLib> mats = new List<MaterialLib>();
				foreach (var ifcmat in ifcmats)
				{
					ifcele.Database.Where(o => o == ifcmat).Cast<IfcRelAssociatesMaterial>().ForEach(x =>
					{
						if (x.RelatingMaterial is IfcMaterialLayerSetUsage)
						{
							var matusage = x.RelatingMaterial as IfcMaterialLayerSetUsage;
							var c = matusage.ForLayerSet.MaterialLayers.Count;
							mats.AddRange(ExtractMaterialData(matusage));
						}
					});
				}
				return mats;
			}

			public static List<MaterialLib> ExtractMaterialData(IfcMaterialLayerSetUsage matusage)
			{
				List<MaterialLib> mats = new List<MaterialLib>();
				var layers = matusage.ForLayerSet.MaterialLayers;
				for (int i = 0; i < layers.Count; i++)
				{
					var layer = layers[i];
					var fmat = new MaterialLib();
					fmat.Name = layer.Material.Name;
					fmat.Additional_Info.Add(new Data()
					{
						Name = "Thickness",
						Value = layer.LayerThickness.ToString()
					});
					List<Data> datas = new List<Data>();
					layer.Material.HasProperties.ForEach(p =>
					{
						var dic = IfcConvert.ToIfcPropertyDic(p.Properties);
						datas.AddRange(IfcData.ExtractProps(dic, fmat));

					});
				}
				return mats;
			}
		}



	}

}
