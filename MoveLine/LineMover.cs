using System;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;
using Microsoft.VisualStudio.Text.Operations;

namespace KevinAenmey.MoveLine
{
    public class LineMover
    {
        public bool MoveLineUp(IWpfTextView view)
        {
            var endLine = GetEndPosition(view);
            var startPosition = GetStartPosition(view);

            var insertPosition = !view.Selection.IsEmpty && endLine.Start.Position == view.Selection.End.Position
                ? endLine.Start.Position
                : endLine.EndIncludingLineBreak.Position;

            var selectionStartLinePosition = view.TextSnapshot.GetLineNumberFromPosition(startPosition.Position);
            if (selectionStartLinePosition == 0) return false;
            var lineAbove = view.TextSnapshot.GetLineFromLineNumber(selectionStartLinePosition - 1);

            SwapLinesUp(view, insertPosition, lineAbove);

            return true;
        }

        public bool MoveLineDown(IWpfTextView view)
        {
            var insertPosition = GetStartPosition(view);
            var endLine = GetEndPosition(view);

            var endPosition = view.Selection.IsEmpty || view.Selection.End.Position != endLine.Start
                ? endLine.EndIncludingLineBreak.Position
                : endLine.End.Position;

            var selectionEndLinePosition = view.TextSnapshot.GetLineNumberFromPosition(endPosition);

            if (selectionEndLinePosition + 1 == view.TextSnapshot.LineCount) return false;

            var lineBelow = view.TextSnapshot.GetLineFromLineNumber(selectionEndLinePosition);
            SwapLines(view, insertPosition, lineBelow);
            return true;
        }

        private static SnapshotPoint GetStartPosition(IWpfTextView view)
        {
            var startPosition = view.Selection.Start.Position;
            var startLine = view.GetTextViewLineContainingBufferPosition(startPosition);
            return startLine.Start;
        }

        private static IWpfTextViewLine GetEndPosition(IWpfTextView view)
        {
            var endPosition = view.Selection.End.Position;
            var endLine = view.GetTextViewLineContainingBufferPosition(endPosition);
            return endLine;
        }

        private static void SwapLines(ITextView view, int insertPosition, ITextSnapshotLine textSnapshotLine)
        {
            var edit = view.TextBuffer.CreateEdit();
            edit.Delete(textSnapshotLine.Start, textSnapshotLine.LengthIncludingLineBreak);
            edit.Insert(insertPosition, textSnapshotLine.GetText() + Environment.NewLine);
            edit.Apply();

            view.Caret.EnsureVisible();
        }

        private static void SwapLinesUp(ITextView view, int insertPosition, ITextSnapshotLine textSnapshotLine)
        {
            if (insertPosition == view.TextSnapshot.Length)
            {
                SwapLines(view, insertPosition, textSnapshotLine);
                return;
            }

            var nextChar = view.TextSnapshot.GetText(insertPosition, 1);
            var insertText = textSnapshotLine.GetText() + Environment.NewLine;

            var edit = view.TextBuffer.CreateEdit();
            edit.Delete(textSnapshotLine.Start, textSnapshotLine.LengthIncludingLineBreak);
            edit.Insert(insertPosition + 1, insertText);
            edit.Apply();

            edit = view.TextBuffer.CreateEdit();
            edit.Delete(insertPosition - insertText.Length, 1);
            edit.Insert(insertPosition + 1, nextChar);
            edit.Apply();

            view.Caret.EnsureVisible();
        }
    }
}



//
//using System;
//using Microsoft.VisualStudio.Text;
//using Microsoft.VisualStudio.Text.Editor;
//using Microsoft.VisualStudio.Text.Formatting;
//using Microsoft.VisualStudio.Text.Operations;
//
//namespace KevinAenmey.MoveLine
//{
//    public class LineMover
//    {
//        public bool MoveLineUp(IWpfTextView view)
//        {
//            var endLine = GetEndPosition(view);
//            var startPosition = GetStartPosition(view);
//
//            var insertPosition = endLine.Start.Position == startPosition.Position
//                ? endLine.EndIncludingLineBreak.Position
//                : endLine.Start.Position;
//
//            var selectionStartLinePosition = view.TextSnapshot.GetLineNumberFromPosition(startPosition.Position);
//            if (selectionStartLinePosition == 0) return false;
//            var lineAbove = view.TextSnapshot.GetLineFromLineNumber(selectionStartLinePosition - 1);
//
//            SwapLinesUp(view, insertPosition, lineAbove);
//
//            return true;
//        }
//
//        public bool MoveLineDown(IWpfTextView view)
//        {
//            var insertPosition = GetStartPosition(view);
//            var endLine = GetEndPosition(view);
//
//            var endPosition = view.Selection.IsEmpty || view.Selection.End.Position != endLine.Start
//                ? endLine.EndIncludingLineBreak.Position
//                : endLine.End.Position;
//
//            var selectionEndLinePosition = view.TextSnapshot.GetLineNumberFromPosition(endPosition);
//
//            if (selectionEndLinePosition + 1 == view.TextSnapshot.LineCount) return false;
//
//            var lineBelow = view.TextSnapshot.GetLineFromLineNumber(selectionEndLinePosition);
//            SwapLines(view, insertPosition, lineBelow);
//            return true;
//        }
//
//        private static SnapshotPoint GetStartPosition(IWpfTextView view)
//        {
//            var startPosition = view.Selection.Start.Position;
//            var startLine = view.GetTextViewLineContainingBufferPosition(startPosition);
//            return startLine.Start;
//        }
//
//        private static IWpfTextViewLine GetEndPosition(IWpfTextView view)
//        {
//            var endPosition = view.Selection.End.Position;
//            var endLine = view.GetTextViewLineContainingBufferPosition(endPosition);
//            return endLine;
//        }
//
//        private static void SwapLines(ITextView view, int insertPosition, ITextSnapshotLine textSnapshotLine)
//        {
//            var edit = view.TextBuffer.CreateEdit();
//            edit.Delete(textSnapshotLine.Start, textSnapshotLine.LengthIncludingLineBreak);
//            edit.Insert(insertPosition, textSnapshotLine.GetText() + Environment.NewLine);
//            edit.Apply();
//
//            view.Caret.EnsureVisible();
//        }
//
//        private static void SwapLinesUp(ITextView view, int insertPosition, ITextSnapshotLine textSnapshotLine)
//        {
//            if (insertPosition == view.TextSnapshot.Length)
//            {
//                SwapLines(view, insertPosition, textSnapshotLine);
//                return;
//            }
//
//            var nextChar = view.TextSnapshot.GetText(insertPosition, 1);
//            var insertText = textSnapshotLine.GetText() + Environment.NewLine;
//
//            var edit = view.TextBuffer.CreateEdit();
//            edit.Delete(textSnapshotLine.Start, textSnapshotLine.LengthIncludingLineBreak);
//            edit.Insert(insertPosition + 1, insertText);
//            edit.Apply();
//
//            edit = view.TextBuffer.CreateEdit();
//            edit.Delete(insertPosition - insertText.Length, 1);
//            edit.Insert(insertPosition + 1, nextChar);
//            edit.Apply();
//
//            view.Caret.EnsureVisible();
//        }
//    }
//}