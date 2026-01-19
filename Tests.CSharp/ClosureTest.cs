namespace Tests.CSharp;

[TestClass]
public class ClosureTest
{
    [TestMethod]
    public void Var_Should_Be_Final_Value_Of_Loop()
    {
        var actions = new List<Func<int>>();

        for (int i = 0; i < 3; i++)
        {
            actions.Add(() => 
            {
                Console.WriteLine(i);
                return i;
            });
        }

        foreach (var action in actions)
        {
            var res = action();

            Assert.AreEqual(3, res);
        }
    }

    [TestMethod]
    public void Var_Should_Capture_Current_Value()
    {
        var actions = new List<Func<int>>();

        int i = 0;
        for (i = 0; i < 3; i++)
        {
            // use a temp var to store the current value
            int a = i;

            actions.Add(() =>
            {
                Console.WriteLine(a);
                return a;
            });
        }

        i = 0;
        foreach (var action in actions)
        {
            var res = action();

            Assert.AreEqual(i++, res);
        }
    }
}
