using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Formatting;
using Moq;
using MoveLineTest.Builders;

namespace MoveLineTest.LineMoverTest
{
    [TestClass]
    public class When_GetStartPosition_is_called : Given_a_new_LineMover
    {
        private SnapshotPoint expectedStartPosition;
        private SnapshotPoint actualStartPosition;

        public override void When()
        {
            var textSnapshot = new Mock<ITextSnapshot>();
            textSnapshot.SetupGet(x => x.Length).Returns(10);
            this.expectedStartPosition = new SnapshotPoint(textSnapshot.Object, 5);
            var startLine = new Mock<IWpfTextViewLine>();
            startLine.SetupGet(x => x.Start).Returns(this.expectedStartPosition);

            var wpfTextView = new WpfTextViewBuilder()
            .SetLineContainingSelectionStart(startLine.Object)
                .BuildMock();

            this.actualStartPosition = this.LineMover.GetStartPosition(wpfTextView.Object);
        }

        [TestMethod]
        public void Then_the_start_position_is_returned_from_the_view()
        {
            this.actualStartPosition.Should().Be(this.expectedStartPosition);
        }
    }

    public class LineMover
    {
        public SnapshotPoint GetStartPosition(IWpfTextView view)
        {
            var startPosition = view.Selection.Start.Position;
            var startLine = view.GetTextViewLineContainingBufferPosition(startPosition);
            return startLine.Start;
        }
    }
}