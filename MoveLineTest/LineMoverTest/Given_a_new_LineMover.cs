using Microsoft.VisualStudio.TestTools.UnitTesting;
using MoveLineTest.Tools;

namespace MoveLineTest.LineMoverTest
{
    [TestClass]
    public abstract class Given_a_new_LineMover : Gwt
    {
        public override void Given()
        {
            this.LineMover = new LineMover();
        }

        protected LineMover LineMover { get; private set; }
    }
}