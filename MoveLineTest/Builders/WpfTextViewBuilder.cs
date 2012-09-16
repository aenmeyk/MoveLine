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
        private readonly Mock<ITextSnapshot> textSnapshot = new Mock<ITextSnapshot> { DefaultValue = DefaultValue.Mock };

        public WpfTextViewBuilder()
        {
            this.textSnapshot.SetupGet(x => x.Length).Returns(TEXT_LENGTH);
            this.wpfTextView.SetupGet(x => x.TextSnapshot).Returns(this.textSnapshot.Object);
        }

        public WpfTextViewBuilder SetLineContainingSelectionStart(IWpfTextViewLine wpfTextViewLine)
        {
            var snapshotPoint = new SnapshotPoint(wpfTextView.Object.TextSnapshot, 0);
            var textSelection =  this.wpfTextView.Object.Selection;
            Mock.Get(textSelection).SetupGet(x => x.Start).Returns(new VirtualSnapshotPoint(snapshotPoint));

            var bufferPosition = this.wpfTextView.Object.Selection.Start.Position;
            this.wpfTextView.Setup(x => x.GetTextViewLineContainingBufferPosition(bufferPosition)).Returns(wpfTextViewLine);

            return this;
        }

        public Mock<IWpfTextView> BuildMock()
        {
            return this.wpfTextView;
        }
    }
}