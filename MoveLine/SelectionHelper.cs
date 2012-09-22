using System.Linq;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;

namespace KevinAenmey.MoveLine
{
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
            if (this.selection.Length > 0)
            {
                var updatedSelectionSnapshot = new SnapshotSpan(this.view.TextSnapshot, this.selection.Start + offset, this.selection.Length);
                this.view.Selection.Select(updatedSelectionSnapshot, false);
                this.view.Caret.MoveTo(new SnapshotPoint(this.view.TextSnapshot, updatedSelectionSnapshot.End));
            }
        }

        public SnapshotPoint GetLineStart()
        {
            var startPosition = this.view.Selection.Start.Position;
            var startLine = this.view.GetTextViewLineContainingBufferPosition(startPosition);
            return startLine.Start;
        }

        public SnapshotPoint GetLineEndIncludingLineBreak()
        {
            var endLine = this.GetEndLine();

            return this.view.Selection.IsEmpty || this.view.Selection.End.Position != endLine.Start
                ? endLine.EndIncludingLineBreak
                : new SnapshotPoint(this.view.TextSnapshot, endLine.Start);
        }

        public SnapshotPoint GetLineEnd()
        {
            var endLine = this.GetEndLine();
            return this.view.Selection.IsEmpty || this.view.Selection.End.Position != endLine.Start
                ? endLine.End
                : new SnapshotPoint(this.view.TextSnapshot, endLine.Start - 1);
        }

        private IWpfTextViewLine GetEndLine()
        {
            var endPosition = this.view.Selection.End.Position;
            var endLine = this.view.GetTextViewLineContainingBufferPosition(endPosition);
            return endLine;
        }
    }
}