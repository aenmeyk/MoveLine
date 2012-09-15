using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MoveLineTest.Tools
{
    [TestClass]
    public abstract class Gwt
    {
        private readonly TestExceptionList testExceptionList = new TestExceptionList();

        [TestInitialize]
        public void SubInitialize()
        {
            try
            {
                this.Given();
                this.When();
            }
            catch (Exception exception)
            {
                this.testExceptionList.Add(exception);
            }
        }

        public abstract void Given();
        public abstract void When();

        [TestCleanup]
        public void SubCleanup()
        {
            this.ThrowUnhandledExceptions();
        }

        protected void ThrowUnhandledExceptions()
        {
            this.testExceptionList.ThrowUnhandled();
        }
    }
}
