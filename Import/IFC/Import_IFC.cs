using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GeometryGym;
using GeometryGym.Ifc;
using MahApps.Metro.Controls.Dialogs;
using InSitU.DataBase;
using InSitU.Model;
using Utility.IO;

namespace InSitU.Import
{
	public partial class IFC
	{
		public static void Import_IFC(string ifcpath = "")
		{
			if (ifcpath == "") ifcpath = IO.LoadFile("IFC File|*.ifc");
			if (string.IsNullOrEmpty(ifcpath)) return;
			var database = new GeometryGym.Ifc.DatabaseIfc(ifcpath);
			ImportProject(database);
		}

		static void ImportProject(DatabaseIfc db)
		{
#if IFCTest
			if (SQ.sql != null)
			{
				if (db.Project.GlobalId == ProjectModel.Set.Projectmodel.Code)
				{
					//the current model is the same as the IFC 
					//TODO: ASk User if he want to replace the currently stored Data with the IFC One
				}

				var projectmodel = ProjectModel.Set.Projectmodel;
#else
			var projectmodel = new ProjectModel();

#endif
			projectmodel.Code = db.Project.GlobalId;
			projectmodel.Name = db.Project.Name;

			IfcBuilding.ExtractBuilding(db);

			var walls = db.Where(o => o is IfcWall).Cast<IfcWall>();
			foreach (var w in walls)
			{
				var finishmodel = new FinishModel();
				IfcObjectType.ExtractTypicalProperties(w, finishmodel);

				finishmodel.FinishLib = IfcObjectType.ExtractType(w) as FinishLib;
				finishmodel.FinishLib.InsertOrReplace();
				IfcData.InsertOrReplaceAll(finishmodel.FinishLib.Additional_Info);
				IfcData.InsertOrReplaceAll(finishmodel.Additional_Info);

				IfcMaterial.ExtractMaterial(w).ForEach(m=>
				{
					m.SetFkeyParent(finishmodel.FinishLib);
					m.InsertOrReplace();
				});


				finishmodel.InsertOrReplace();

			}

#if IFCTest
			}
			else
			{
				Utility.Constants.MainWindow.ShowModalMessageExternal("Please create a project first", "");
			} 
#endif
		}


	}
}
