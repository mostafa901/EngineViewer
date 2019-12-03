using ImGuiNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Urho3DNet;

namespace EngineViewer.Actions._3D.UI
{
	public class UIMenu
	{
		public UIMenu(Node RootNode, Engn_Selection Selection)
		{ 
			this.RootNode = RootNode;
			this.Selection = Selection;
		}
		public enum menuaction
		{
			none,
			ObjectContext,

		}
		public menuaction ActionMenu = menuaction.none;

		public Node RootNode { get; set; }
		public Engn_Selection Selection { get; set; }

		public void SetupMenu()
		{
			if (ImGui.BeginMainMenuBar())
			{
				if (ImGui.BeginMenu("File"))
				{
					if (ImGui.MenuItem("Import", ""))
					{
						var node = Rbfx_IO.LoadAsset(RootNode, false);

					}
					if (ImGui.MenuItem("Open", ""))
					{
						var node = Rbfx_IO.LoadAsset(RootNode, true);
					}
					if (ImGui.MenuItem("Save", ""))
					{
						Rbfx_IO.SaveAsset(RootNode.Scene);
					}

					ImGui.EndMenu();
				}

				if (ImGui.BeginMenu("Scene"))
				{
					if (ImGui.BeginMenu("Rotate"))
					{
						if (ImGui.MenuItem("X:90", ""))
						{
							RootNode.Rotate(new Quaternion(90, 0, 0));
						}
						ImGui.EndMenu();
					}

					ImGui.EndMenu();
				}
				ImGui.EndMainMenuBar();
			}


			switch (ActionMenu)
			{
				case menuaction.ObjectContext:
					{
						ImGui.OpenPopup("ObjectContext");

						if (ImGui.BeginPopup("ObjectContext"))
						{
							if (ImGui.Button("Rotate 90"))
							{
								Selection.SelectedModel.Node.Rotate(new Quaternion(90, 0, 0));
								ActionMenu = menuaction.none;
							}
							ImGui.EndPopup();
						}
						break;
					}
				case menuaction.none:
				default:
					break;
			}
		}
	}
}

