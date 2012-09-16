using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.Text;

namespace MoveLineTest.LineMoverTest
{
    [TestClass]
    public class When_GetStartPosition_is_called : Given_a_new_LineMover
    {
        private SnapshotPoint startPosition;

        public override void When()
        {
            var wpfTextView = this.WpfTextViewBuilder
                .SetSelectedLineStart(5)
                .BuildMock();

            this.startPosition = this.LineMover.GetStartPosition(wpfTextView.Object);
        }

        [TestMethod]
        public void Then_the_start_position_is_returned_from_the_view()
        {
            this.startPosition.Should().Be(this.WpfTextViewBuilder.SelectedLineStartPosition);
        }
    }
}