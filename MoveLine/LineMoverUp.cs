using System;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace KevinAenmey.MoveLine
{
    public class LineMoverUp : ILineMover
    {
        public SnapshotPoint GetInsertPosition(SelectionHelper selectionHelper)
        {
            return selectionHelper.GetLineEndIncludingLineBreak();
        }

        public ITextSnapshotLine GetLineToSwap(IWpfTextView view, SelectionHelper selectionHelper)
        {
            var startPosition = selectionHelper.GetLineStartPoint();
            var startLineNumber = view.TextSnapshot.GetLineNumberFromPosition(startPosition.Position);

            return view.TextSnapshot.GetLineFromLineNumber(startLineNumber - 1);
        }

        public int PerformMove(IWpfTextView view, ITextSnapshotLine lineToSwap, int insertPosition)
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

            return -lineToSwap.LengthIncludingLineBreak;
        }
    }
}