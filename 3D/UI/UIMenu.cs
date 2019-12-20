using EngineViewer.Actions._3D.RbfxUtility;
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

        public void RenderMenu()
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

                    if (ImGui.MenuItem("UnHideAll"))
                    {
                        RootNode.SetEnabledRecursive(true);
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

                            if (ImGui.Button("Hide"))
                            {
                                Selection.SelectedModel.Node.SetEnabled(false);
                                Selection.SelectedModel = null;
                                ActionMenu = menuaction.none;
                            }

                            if (ImGui.BeginMenu("CullMode"))
                            {
                                var cusnode = Selection.SelectedModel.Node.GetComponent<CustomNodeComponent>(true);
                                var mat = cusnode.OriginalMaterial;
                                if (ImGui.MenuItem("Ccw"))
                                {
                                    if (mat != null)
                                        mat.CullMode = CullMode.CullCcw;
                                    ActionMenu = menuaction.none;
                                }
                                if (ImGui.MenuItem("Cw"))
                                {
                                    if (mat != null)
                                        mat.CullMode = CullMode.CullCw;
                                    ActionMenu = menuaction.none;
                                }
                                if (ImGui.MenuItem("None"))
                                {
                                    if (mat != null)
                                        mat.CullMode = CullMode.CullNone;
                                    ActionMenu = menuaction.none;
                                }
                                ImGui.EndMenu();
                            }

                            if (ImGui.BeginMenu("Transparency"))
                            {
                                if (ImGui.MenuItem("Set Transparent"))
                                {
                                    var comp = Selection.SelectedModel.Node.GetComponent<CustomNodeComponent>(true);

                                    Material transmap = new Material(Selection.SelectedModel.Node.Context);
                                    //transmap.SetTechnique(0, transmap.Context.Cache.GetResource<Technique>("Techniques/DiffVCol.xml"));
                                    transmap = RootNode.Context.Cache.GetResource<Material>("Materials/Stone.xml");
                                    transmap.SetShaderParameter("MatDiffColor", new Color(.6f, .6f, .6f, .4f));
                                    ((dynamic)Selection.SelectedModel).SetMaterial(transmap);
                                    Selection.SetOriginalMaterial(transmap);
                                    ActionMenu = menuaction.none;
                                }

                                if (ImGui.MenuItem("Restore Material"))
                                {
                                    var comp = Selection.SelectedModel.Node.GetComponent<CustomNodeComponent>(true);
                                    if (comp != null)
                                    {
                                        if (comp.OriginalMaterial != null)
                                        {
                                            ((dynamic)Selection.SelectedModel).SetMaterial(comp.OriginalMaterial);
                                            Selection.SetOriginalMaterial(comp.OriginalMaterial);
                                        }
                                    }
                                    ActionMenu = menuaction.none;
                                }
                                ImGui.EndMenu();
                            }

                            if (ImGui.Button("Delete"))
                            {
                                Selection.SelectedModel.Remove();
                                Selection.SelectedModel = null;
                                ActionMenu = menuaction.none;
                            }
                            if (ImGui.Button("Debug Renderer"))
                            {
                                var debugrend = Selection.SelectedModel.Scene.GetComponent<DebugRenderer>();
                                Selection.SelectedModel.Node.Context. Renderer.DrawDebugGeometry(true);
                                Selection.SelectedModel = null;
                                ActionMenu = menuaction.none;
                            }
                            if (!ImGui.IsAnyItemHovered() && Selection.SelectedModel.Context.Input.GetMouseButtonPress(MouseButton.MousebLeft))
                            {
                                Selection.SelectedModel = null;
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