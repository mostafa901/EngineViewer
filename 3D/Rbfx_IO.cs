using EngineViewer.Actions._3D.RbfxUtility;
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
                path = Utility.IO.system.LoadFiles("bin|*.bin|xml|*.xml|ifc|*.ifc|Collada DAE|*.dae|mdl|*.mdl|FBX|*.fbx|DXF|*.dxf|3DS|*.3ds").FirstOrDefault();
                if (path == null) return;

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
                    if (!path.ToLower().EndsWith("xml") && !path.ToLower().EndsWith("bin"))
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

            RootNode.Context.Cache.AddResourceDir(Path.GetDirectoryName(path), 1);

            if (open)
            {
                RootNode.RemoveAllChildren();
            }

            Node modelnode = RootNode.CreateChild("");
            var isloaded = modelnode.LoadFile(path);
            if (!isloaded)
            {
                Shared_Utility.Logger.Logger.Log($"Error Loading File {path}", "Load File", Shared_Utility.Logger.Logger.ErrorType.Error);
            }
#if false
            modelnode.Name = Path.GetFileNameWithoutExtension(path);
            var stmodels = modelnode.GetComponents<StaticModel>(true).Cast<StaticModel>();
            foreach (var stm in stmodels)
            {
                var comp = stm.Node.CreateComponent<CustomNodeComponent>();
                comp.OriginalMaterial = stm.GetMaterial();
            }
#endif
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

#if false
            Urho3DNet.File file = new Urho3DNet.File(scene.Context, path, Urho3DNet.FileMode.FileWrite);
            file.sa
            if (!scene.Save(file))
            {
                new MessageBox(scene.Context, "Couldn't Save File", "Save Error");
                return;
            }
#endif
#if false
            var jsonFile = new JSONFile(scene.Context);
            var rootElem = jsonFile.GetRoot();
            if (!scene.SaveJSON(rootElem))
            {
                new MessageBox(scene.Context, "Couldn't Save File", "Save Error");
                return;
            }

            jsonFile.SaveFile(path);
#endif
#if true
            var cusgeoms = scene.GetComponents<CustomGeometry>(true).Cast<CustomGeometry>();
            var dir = System.IO.Path.GetDirectoryName(path);

            Directory.CreateDirectory(dir + "/Materials");

            foreach (var cusgeo in cusgeoms)
            {
                if (cusgeo.IsTemporary()) continue;
                var mat = cusgeo.GetMaterial();
                if (mat != null)
                {
                    mat.SaveFile($"{dir}/{mat.Name}");
                }
            }
            var xmlFile = new XMLFile(scene.Context);
            XMLElement rootElem = xmlFile.CreateRoot("Root");
            if (!scene.SaveXML(rootElem))
            {
                new MessageBox(scene.Context, "Couldn't Save File", "Save Error");
                return;
            }

            xmlFile.SaveFile(path);
#endif
            //Task.Run(async () => await cam.MoveToSelected(max, modelnode.Position, 50));
        }
    }
}