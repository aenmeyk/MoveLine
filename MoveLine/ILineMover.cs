using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace KevinAenmey.MoveLine
{
    public interface ILineMover 
    {
        SnapshotPoint GetInsertPosition(SelectionHelper selectionHelper);
        ITextSnapshotLine GetLineToSwap(IWpfTextView view, SelectionHelper selectionHelper);
        int PerformMove(IWpfTextView view, ITextSnapshotLine lineToSwap, int insertPosition);
    }
}