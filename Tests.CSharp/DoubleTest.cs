namespace Tests.CSharp;

[TestClass]
public sealed class DoubleTest
{
    private const double doubleVal =  10.789738975897389458937485738947857349;
    private const string doubleStr = "10.789738975897389458937485738947857349";

    [TestMethod]
    public void DoubleToString()
    {
        var s = doubleVal.ToString();

        // NOT EQUAL!
        Assert.AreNotEqual(doubleStr, s);
    }

    [TestMethod]
    public void BitConverterToBytes()
    {
        byte[] doubleBytes = BitConverter.GetBytes(doubleVal);

        double convertedDouble = BitConverter.ToDouble(doubleBytes, 0);

        Assert.AreEqual(doubleVal, convertedDouble);
    }

    [TestMethod]
    public void ZeroPointOne()
    {
        double a = 0.1;
        double b = 0.2;

        // 0.30000000000000004
        Assert.AreNotEqual(0.3, a + b);
    }
}
