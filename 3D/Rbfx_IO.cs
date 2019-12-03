using EngineViewer.Actions._3D.RbfxUtility;
using EngineViewer.Controls;
using EngineViewer.Controls.TypicalControls._3D;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Threading;
using Urho3DNet;

namespace EngineViewer.Actions._3D
{
	public static class Rbfx_IO
	{
		public static Node LoadAsset(Node RootNode, bool open)
		{
			string path = "";
			Dispatcher.CurrentDispatcher.Invoke(() =>
			{
				path = Utility.IO.system.LoadFile("xml|*.xml|ifc|*.ifc|Collada DAE|*.dae|mdl|*.mdl|FBX|*.fbx|DXF|*.dxf|3DS|*.3ds");

				if (path.Contains("}") || path.Contains("{"))
				{
					DefaultScene.Actions.Add(() =>
					{
						new MessageBox(DefaultScene.scene.Context, "File name must not contain { or }.", "File Name Mismatch");
					});
					path = "";

					return;
				}
				if (!string.IsNullOrEmpty(path))
				{
					if (!path.ToLower().EndsWith("xml"))
					{
						string assetport = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
						Process p = null;
						if (path.ToLower().EndsWith("ifc"))
						{
							//http://ifcopenshell.org/ifcconvert.html
							p = Process.Start($"{assetport}\\Import\\ifcConvert.exe", $"\"{path}\" \"{path + ".xml"}\"");
							p = Process.Start($"{assetport}\\Import\\ifcConvert.exe", $"\"{path}\" \"{path += ".dae"}\"");
							p.WaitForExit();
						}

						//TODO:extract information from IFc and add to DAE  
						p = Process.Start($"{assetport}\\Import\\AssetImporter_Win64.exe", $"node \"{path}\" \"{path}.xml\"");

						p.WaitForExit();
						path += ".xml";
					}
				}
			});
			if (string.IsNullOrEmpty(path)) return null;

			RootNode.Cache.AddResourceDir(Path.GetDirectoryName(path), 1);
			
			if (open)
			{
				RootNode.RemoveAllChildren();
			}

			Node modelnode = RootNode.CreateChild("");
			modelnode.LoadXML(path);
			modelnode.Name = Path.GetFileNameWithoutExtension(path);
						 
			return modelnode;
		}

		internal static void SaveAsset(Scene scene)
		{
			string path = "";
			Dispatcher.CurrentDispatcher.Invoke(() =>
			{
				var dlg = new Ookii.Dialogs.Wpf.VistaSaveFileDialog();
				dlg.Filter = "xml|*.xml";
				if (dlg.ShowDialog() == true)
				{
					path = dlg.FileName;

					if (path.Contains("}") || path.Contains("{"))
					{
						DefaultScene.Actions.Add(() =>
						{
							new MessageBox(DefaultScene.scene.Context, "File name must not contain { or }.", "File Name Mismatch");
						});
						path = "";

						return;
					}
				}
			});

			if (string.IsNullOrEmpty(path)) return;
			var xmlFile = new XMLFile(scene.Context);
			XMLElement rootElem = xmlFile.CreateRoot("Root");
			if (!scene.SaveXML(rootElem))
			{
				new MessageBox(scene.Context, "Couldn't Save File", "Save Error");
				return;
			}

			xmlFile.SaveFile(path);
			//	Task.Run(async () => await cam.MoveToSelected(max, modelnode.Position, 50));
		}

	}


}