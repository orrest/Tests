namespace Tests
{
    [TestClass]
    public sealed class Test1
    {
        [TestMethod]
        public void TestMethod1()
        {
            var hello = "Hello, World!";

            Assert.AreEqual("Hello, World!", hello);
        }
    }
}
