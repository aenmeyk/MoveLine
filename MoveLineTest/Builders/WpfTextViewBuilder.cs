using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Moq;
using Microsoft.VisualStudio.Text.Formatting;

namespace MoveLineTest.Builders
{
    public class WpfTextViewBuilder
    {
        private const int TEXT_LENGTH = 100;
        private readonly Mock<IWpfTextView> wpfTextView = new Mock<IWpfTextView> { DefaultValue = DefaultValue.Mock };
        private readonly Mock<ITextSelection> textSelection;
        public Mock<IWpfTextViewLine> EndLine { get; private set; }
        public SnapshotPoint SelectedLineStartPosition { get; private set; }
        public SnapshotPoint SelectedLineEndPosition { get; private set; }

        public WpfTextViewBuilder()
        {
            var textSnapshot = new Mock<ITextSnapshot> { DefaultValue = DefaultValue.Mock };
            textSnapshot.SetupGet(x => x.Length).Returns(TEXT_LENGTH);
            this.wpfTextView.SetupGet(x => x.TextSnapshot).Returns(textSnapshot.Object);
            this.textSelection = Mock.Get(this.wpfTextView.Object.Selection);
        }

        public WpfTextViewBuilder SetSelectedLineStart(int position)
        {
            this.SelectedLineStartPosition = new SnapshotPoint(this.wpfTextView.Object.TextSnapshot, position);
            var startLine = new Mock<IWpfTextViewLine>();
            startLine.SetupGet(x => x.Start).Returns(this.SelectedLineStartPosition);

            var selectionStartPoint = new SnapshotPoint(wpfTextView.Object.TextSnapshot, 0);
            this.textSelection.SetupGet(x => x.Start).Returns(new VirtualSnapshotPoint(selectionStartPoint));

            var bufferPosition = this.wpfTextView.Object.Selection.Start.Position;
            this.wpfTextView.Setup(x => x.GetTextViewLineContainingBufferPosition(bufferPosition)).Returns(startLine.Object);

            return this;
        }

        public WpfTextViewBuilder SetSelectedLineEnd(int position)
        {
            this.SelectedLineEndPosition = new SnapshotPoint(this.wpfTextView.Object.TextSnapshot, position);
            this.EndLine = new Mock<IWpfTextViewLine>();
            this.EndLine.SetupGet(x => x.EndIncludingLineBreak).Returns(this.SelectedLineEndPosition);

            var selectionEndPoint = new SnapshotPoint(wpfTextView.Object.TextSnapshot, 0);
            this.textSelection.SetupGet(x => x.End).Returns(new VirtualSnapshotPoint(selectionEndPoint));

            var bufferPosition = this.wpfTextView.Object.Selection.End.Position;
            this.wpfTextView.Setup(x => x.GetTextViewLineContainingBufferPosition(bufferPosition)).Returns(this.EndLine.Object);

            return this;
        }
 
        public Mock<IWpfTextView> BuildMock()
        {
            return this.wpfTextView;
        }
   }
}