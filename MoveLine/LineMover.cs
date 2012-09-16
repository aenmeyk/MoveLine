using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace KevinAenmey.MoveLine
{
    public class LineMover
    {
        public SnapshotPoint GetStartPosition(IWpfTextView view)
        {
            var startPosition = view.Selection.Start.Position;
            var startLine = view.GetTextViewLineContainingBufferPosition(startPosition);
            return startLine.Start;
        }

        public SnapshotPoint GetEndPosition(IWpfTextView view)
        {
            var endPosition = view.Selection.End.Position;
            var endLine = view.GetTextViewLineContainingBufferPosition(endPosition);
            return endLine.EndIncludingLineBreak;
        }
    }
}