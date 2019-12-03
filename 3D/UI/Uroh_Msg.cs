using Urho3DNet;
using ImGuiNet;

namespace InSitU.Actions._3D.UI
{
	internal class Uroh_Msg
	{
		private Window window;

		public Uroh_Msg(UIElement rootui, string msg, string title)
		{
			// Create the Window and add it to the UI's root node
			window = new Window();
			rootui.AddChild(window);

			// Set Window size and layout settings
			window.SetMinSize(384, 192);
			window.SetLayout(LayoutMode.Vertical, 6, new IntRect(6, 6, 6, 6));
			window.SetAlignment(HorizontalAlignment.Center, VerticalAlignment.Center);
			window.Name = "Window";

			// Create Window 'titlebar' container
			UIElement titleBar = new UIElement();
			titleBar.MaxHeight = 20;
			var brd = titleBar.CreateBorderImage();
			brd.SetColor(new Color(0.22f, 0.22f, 0.22f,1));
			brd.LayoutMode = LayoutMode.Horizontal;

			titleBar.VerticalAlignment = Urho.Gui.VerticalAlignment.Top;
			titleBar.LayoutMode = LayoutMode.Horizontal;

			// Create the Window title Text
			var windowTitle = new Text();
			windowTitle.Name = "WindowTitle";
			windowTitle.Value = title;

			var windowmsg = new Text();
			windowmsg.Value = msg;
			windowmsg.VerticalAlignment = VerticalAlignment.Top;

			// Create the Window's close button
			Button buttonClose = new Button();
			buttonClose.Name = "CloseButton";

			// Add the controls to the title bar
			brd.AddChild(windowTitle);
			brd.AddChild(buttonClose);

			// Add the title bar to the Window
			window.AddChild(titleBar);
			window.AddChild(windowmsg);

			// Apply styles
			window.SetStyleAuto();
			
			windowmsg.SetStyleAuto();
			windowTitle.SetStyleAuto();
			
			buttonClose.SetStyle("CloseButton", null);

			buttonClose.SubscribeToReleased(_ => window.Remove());

			// Subscribe also to all UI mouse clicks just to see where we have clicked
			//	UI.SubscribeToUIMouseClick(HandleControlClicked);
		}
	}
}