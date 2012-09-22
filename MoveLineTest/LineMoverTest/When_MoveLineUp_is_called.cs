using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.Text.Editor;
using Moq;

namespace MoveLineTest.LineMoverTest
{
    [TestClass]
    public class When_MoveLineUp_is_called : Given_a_new_LineMover
    {
        private Mock<IWpfTextView> view;

        public override void When()
        {
            this.view = this.WpfTextViewBuilder
                .BuildMock();

//            this.LineMover.MoveLine(this.view.Object);
        }

        [TestMethod]
        public void Then_the_line_under_the_cursor_moves_one_line_up()
        {
            var result = this.view.Object.TextSnapshot.GetText();
        }
    }
}