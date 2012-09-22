using System;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;
using System.Linq;

namespace KevinAenmey.MoveLine
{
    public class LineMover
    {
        public bool MoveLineUp(IWpfTextView view)
        {
            try
            {

                /* GET SWAP LINE
                 * ------------------------------
                 * get start of line to swap
                 * get end of line to swap
                 * get ITextSnapshotLine of text to swap:
                 *  - if moving up: include end linebreak.
                 *  - if moving down: include start linebreak.
                 *  
                 * GET INSERT POSITION
                 * -----------------------------------
                 *  - if moving up: start of text to move
                 *  - if moving down: end of text to move
                 *  
                 * GET SELECTION
                 * -------------------------------
                 * Get current selection start
                 * Get current selection end
                 * After move (reset selection): 
                 *  - if moving up: new selection start = old - len of swap text
                 *  - if moving down: new selection start = old + len of swap text
                 *  
                 * PERFORM MOVE
                 * ----------------------------------
                 * Delete ITextSnapshotLine
                 * Insert text snapshot at insert position
                 * Reset selection
                */

                var selectionHelper = new SelectionHelper(view);
                selectionHelper.TakeSelectionSnapshot();

                var lineToSwap = this.GetLineToSwap(view, selectionHelper);
                var insertPosition = selectionHelper.GetLineEndIncludingLineBreak();
                this.PerformMove(view, lineToSwap, insertPosition);

                selectionHelper.ApplySelection(-lineToSwap.LengthIncludingLineBreak);
                view.Caret.EnsureVisible();
            }
            catch(Exception e)
            {
                return false;
            }

            return true;
        }

        public ITextSnapshotLine GetLineToSwap(IWpfTextView view, SelectionHelper selectionHelper)
        {
            var startPosition = selectionHelper.GetLineStart();
            var startLineNumber = view.TextSnapshot.GetLineNumberFromPosition(startPosition.Position);

            return view.TextSnapshot.GetLineFromLineNumber(startLineNumber - 1);
        }

        private void PerformMove(IWpfTextView view, ITextSnapshotLine lineToSwap, int insertPosition)
        {
            var insertedText = lineToSwap.GetTextIncludingLineBreak();

            if(insertPosition == view.TextSnapshot.Length)
            {
                // We don't want ot move the line break if the insert position is the last character of the 
                // document but also the first character of the line (i.e. an empty line at the end of the document)
                var lineUnderInsertPosition = view.TextSnapshot.GetLineFromPosition(insertPosition);
                if (lineUnderInsertPosition.Length > 0)
                {
                    // Move the line break to the start of the text to insert
                    insertedText = (Environment.NewLine + insertedText).Substring(0, insertedText.Length);
                }
            }

            using (var edit = view.TextBuffer.CreateEdit())
            {
                edit.Delete(lineToSwap.Start, lineToSwap.LengthIncludingLineBreak);
                edit.Insert(insertPosition, insertedText);
                edit.Apply();
            }
        }

    }

    public class SelectionHelper
    {
        private readonly IWpfTextView view;
        private SnapshotSpan selection;

        public SelectionHelper(IWpfTextView view)
        {
            this.view = view;
        }

        public void TakeSelectionSnapshot()
        {
            this.selection = this.view.Selection.SelectedSpans.FirstOrDefault();
        }

        public void ApplySelection(int offset)
        {
            if (selection.Length > 0)
            {
                var updatedSelectionSnapshot = new SnapshotSpan(view.TextSnapshot, selection.Start + offset, selection.Length);
                view.Selection.Select(updatedSelectionSnapshot, false);
                view.Caret.MoveTo(new SnapshotPoint(view.TextSnapshot, updatedSelectionSnapshot.End));
            }
        }

        public SnapshotPoint GetLineStart()
        {
            var startPosition = view.Selection.Start.Position;
            var startLine = view.GetTextViewLineContainingBufferPosition(startPosition);
            return startLine.Start;
        }

        public SnapshotPoint GetLineEndIncludingLineBreak()
        {
            var endPosition = view.Selection.End.Position;
            var endLine = view.GetTextViewLineContainingBufferPosition(endPosition);

            return view.Selection.IsEmpty || view.Selection.End.Position != endLine.Start
                ? endLine.EndIncludingLineBreak
                : new SnapshotPoint(view.TextSnapshot, endLine.Start);
        }
    }
}