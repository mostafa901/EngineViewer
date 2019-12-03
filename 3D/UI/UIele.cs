using System;
using Urho;
using Urho.Gui;
using Urho.Resources;

namespace InSitU.Actions._3D.UI
{
	public static class UIele
	{ 
		public enum ButtonType
		{
			Save,
			Close
		}

		public static Text GetText(string text, string style = "")
		{
			var txt = new Text();
			if (string.IsNullOrEmpty(style)) txt.SetStyleAuto();
			else txt.SetStyle(style);
			txt.Value = text;
			txt.MinHeight = 20;
			txt.MinWidth = 100;
			return txt;
		}

		private static ToolTip SetToolTip(this UIElement ele, string tip)
		{
			// Add a tooltip to Fish button
			ToolTip toolTip = new ToolTip();

			toolTip.Position = new IntVector2(ele.Width + 5, ele.Width / 2);
			// slightly offset from close button
			BorderImage textHolder = new BorderImage();
			toolTip.AddChild(textHolder);
			textHolder.SetStyle("ToolTipBorderImage", null);
			var toolTipText = GetText(tip, "ToolTipText");
			textHolder.AddChild(toolTipText);
			ele.AddChild(toolTip);
			return toolTip;
		}

		public static Button GetButton(ResourceCache cache, ButtonType btype, string tooltip = "")
		{
			var button = new Button();
			button.Name = btype.ToString();
			button.SetStyleAuto();

			button.SetToolTip(string.IsNullOrEmpty(tooltip) ? btype.ToString() : tooltip);

			switch (btype)
			{
				case ButtonType.Save:
					{
						button.Texture = cache.GetTexture2D("Buttons/Save.png");
						button.BlendMode = BlendMode.Add;
					}
					break;

				case ButtonType.Close:
					{
						button.SetStyle("CloseButton");
					}
					break;

				default:
					break;
			}

			return button;
		}

		private static Action<object> Actions(ButtonType btype)
		{
			switch (btype)
			{
				case ButtonType.Save:
					return (x) => { };
					break;

				case ButtonType.Close:
					return (x) => { };
					break;

				default:
					break;
			}

			return null;
		}
	}
}