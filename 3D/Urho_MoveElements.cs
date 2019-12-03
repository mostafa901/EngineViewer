using Urho;
using Urho.Gui;

namespace InSitU.Controls.TypicalControls._3D
{
	internal class Urho_MoveElements
	{
		private IntVector2 dragBeginPosition;

		public void SubscribeToMove(UIElement ele)
		{
			// Subscribe button to Drag Events (in order to make it draggable)
			ele.SubscribeToDragBegin(HandleDragBegin);
			ele.SubscribeToDragMove(HandleDragMove);
			ele.SubscribeToDragEnd(HandleDragEnd);
		}

		private void HandleDragBegin(DragBeginEventArgs args)
		{
			// Get UIElement relative position where input (touch or click) occured (top-left = IntVector2(0,0))
			dragBeginPosition = new IntVector2(args.ElementX, args.ElementY);
		}

		private void HandleDragMove(DragMoveEventArgs args)
		{
			IntVector2 dragCurrentPosition = new IntVector2(args.X, args.Y);
			args.Element.Position = dragCurrentPosition - dragBeginPosition;
		}

		private void HandleDragEnd(DragEndEventArgs args) // For reference (not used here)
		{
		}
	}
}