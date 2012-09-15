using System;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace MoveLine
{
    internal class MoveLineCommands
    {
        internal static bool MoveLineUp(IWpfTextView view)
        {
            if (view == null) return false;
            var currentLine = view.Caret.ContainingTextViewLine;
            if (currentLine.Start.Position == 0) return false;

            if (view.Caret.Position.BufferPosition.Position == currentLine.Snapshot.Length)
            {
                view.Caret.MoveTo(new SnapshotPoint(currentLine.Snapshot, currentLine.Snapshot.Length - 1));
            }

            var currentLineSpanLength = new SnapshotSpan(currentLine.Start, currentLine.EndIncludingLineBreak).Length;
            var insertPosition = currentLine.Start.Position + currentLineSpanLength;
            var otherLine = new SnapshotPoint(currentLine.Snapshot, currentLine.Start.Position - 1).GetContainingLine();
            var otherLineSpan = new SnapshotSpan(otherLine.Start, otherLine.EndIncludingLineBreak);
            var otherLinetext = currentLine.End.Position == currentLine.EndIncludingLineBreak.Position
                ? Environment.NewLine + otherLineSpan.GetText()
                : otherLineSpan.GetText();

            SwapLines(view, insertPosition, otherLineSpan, otherLinetext);

            return true;
        }

        internal static bool MoveLineDown(IWpfTextView view)
        {
            if (view == null) return false;
            var currentLine = view.Caret.ContainingTextViewLine;
            if (currentLine.EndIncludingLineBreak.Position >= currentLine.Snapshot.Length) return false;

            var insertPosition = currentLine.Start.Position;
            var otherLine = new SnapshotPoint(currentLine.Snapshot, currentLine.EndIncludingLineBreak.Position + 1).GetContainingLine();
            var otherLineSpan = new SnapshotSpan(otherLine.Start, otherLine.EndIncludingLineBreak);
            var otherLinetext = otherLine.End.Position == otherLine.EndIncludingLineBreak.Position
                ? otherLineSpan.GetText() + Environment.NewLine
                : otherLineSpan.GetText();

            SwapLines(view, insertPosition, otherLineSpan, otherLinetext);

            return true;
        }

        private static void SwapLines(IWpfTextView view, int insertPosition, SnapshotSpan otherLineSpan, string otherLinetext)
        {
            var edit = view.TextBuffer.CreateEdit();
            edit.Delete(otherLineSpan);
            edit.Insert(insertPosition, otherLinetext);
            edit.Apply();

            view.Caret.EnsureVisible();
        }
    }
}