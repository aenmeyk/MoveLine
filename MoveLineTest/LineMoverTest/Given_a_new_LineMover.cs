using KevinAenmey.MoveLine;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.Text;
using Moq;
using MoveLineTest.Builders;
using MoveLineTest.Tools;

namespace MoveLineTest.LineMoverTest
{
    [TestClass]
    public abstract class Given_a_new_LineMover : Gwt
    {

        public override void Given()
        {
            this.WpfTextViewBuilder = new WpfTextViewBuilder();
            this.TextSnapshot = new Mock<ITextSnapshot>();
            this.TextSnapshot.SetupGet(x => x.Length).Returns(10);
            this.LineMover = new LineMover();
        }

        protected WpfTextViewBuilder WpfTextViewBuilder { get; private set; }
        protected LineMover LineMover { get; private set; }
        protected Mock<ITextSnapshot> TextSnapshot { get; private set; }
    }
}