using System;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace KevinAenmey.MoveLine
{
    public class LineMoverDown : ILineMover
    {
        public SnapshotPoint GetInsertPosition(SelectionHelper selectionHelper)
        {
            return selectionHelper.GetLineStartPoint();
        }

        public ITextSnapshotLine GetLineToSwap(IWpfTextView view, SelectionHelper selectionHelper)
        {
            var endPosition = selectionHelper.GetLineEndPoint();
            var endLineNumber = view.TextSnapshot.GetLineNumberFromPosition(endPosition.Position);

            return view.TextSnapshot.GetLineFromLineNumber(endLineNumber + 1);
        }

        public int PerformMove(IWpfTextView view, ITextSnapshotLine lineToSwap, int insertPosition)
        {
            var insertedText = lineToSwap.GetTextIncludingLineBreak();
            var deleteStartPosition = lineToSwap.Start.Position;

            if(lineToSwap.End == lineToSwap.EndIncludingLineBreak)
            {
                insertedText = insertedText + Environment.NewLine;
                deleteStartPosition = deleteStartPosition - Environment.NewLine.Length;
            }

            using (var edit = view.TextBuffer.CreateEdit())
            {
                edit.Delete(deleteStartPosition, insertedText.Length);
                edit.Insert(insertPosition, insertedText);
                edit.Apply();
            }

            return insertedText.Length;
        }
    }
}