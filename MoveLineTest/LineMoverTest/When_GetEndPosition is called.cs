using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.Text.Formatting;

namespace MoveLineTest.LineMoverTest
{
    [TestClass]
    public class When_GetEndPosition_is_called : Given_a_new_LineMover
    {
        private IWpfTextViewLine endPosition;

        public override void When()
        {
            var wpfTextView = this.WpfTextViewBuilder
                .SetSelectedLineEnd(position: 5)
                .BuildMock();

//            this.endPosition = this.LineMover.GetEndPosition(wpfTextView.Object);
        }

        [TestMethod]
        public void Then_the_end_position_is_returned_from_the_view()
        {
            this.endPosition.Should().Be(this.WpfTextViewBuilder.EndLine);
        }
    }
}