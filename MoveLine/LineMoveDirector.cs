using Microsoft.VisualStudio.Text.Editor;

namespace KevinAenmey.MoveLine
{
    public class LineMoveDirector
    {
        private readonly ILineMover lineMover;

        public LineMoveDirector(ILineMover lineMover)
        {
            this.lineMover = lineMover;
        }

        public void MoveLine(IWpfTextView view)
        {
            try
            {
                var selectionHelper = new SelectionHelper(view);
                selectionHelper.TakeSelectionSnapshot();

                var lineToSwap = this.lineMover.GetLineToSwap(view, selectionHelper);
                var insertPosition = this.lineMover.GetInsertPosition(selectionHelper);
                var offSet = this.lineMover.PerformMove(view, lineToSwap, insertPosition);

                selectionHelper.ApplySelection(offSet);
                view.Caret.EnsureVisible();
            }
            catch
            {
                // Not critical. Swallow the exception.
            }
        }
    }
}