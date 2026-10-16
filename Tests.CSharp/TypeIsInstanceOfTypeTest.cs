namespace Tests.CSharp;

// Explains https://learn.microsoft.com/en-us/dotnet/api/system.type.isinstanceoftype
// 说明 Type.IsInstanceOfType 的文档行为，重点关注当前 Type 为 object 时的返回结果。
[TestClass]
public sealed class TypeIsInstanceOfTypeTest
{
    private interface IExample { }

    private class BaseClass : IExample { }

    private sealed class DerivedClass : BaseClass { }

    private struct PlainStruct { }

    [TestMethod]
    public void ObjectTypeMatchesEveryNonNullInstance()
    {
        // object is the root of every inheritance hierarchy, so every non-null instance matches.
        // object 位于一切继承层次之根，因此任意非空实例都匹配。
        var baseInstance = new BaseClass();
        var derivedInstance = new DerivedClass();
        object boxedStruct = new PlainStruct();

        Assert.IsTrue(typeof(object).IsInstanceOfType(baseInstance));
        Assert.IsTrue(typeof(object).IsInstanceOfType(derivedInstance));
        Assert.IsTrue(typeof(object).IsInstanceOfType("text"));
        Assert.IsTrue(typeof(object).IsInstanceOfType(42));
        Assert.IsTrue(typeof(object).IsInstanceOfType(new int[3]));
        Assert.IsTrue(typeof(object).IsInstanceOfType(boxedStruct));
    }

    [TestMethod]
    public void ObjectTypeDoesNotMatchNull()
    {
        // The documented null rule wins even for object: returns false when o is null.
        // 即使当前 Type 是 object，文档中 o 为 null 的规则依然优先，返回 false。
        Assert.IsFalse(typeof(object).IsInstanceOfType(null));
        Assert.IsFalse(typeof(BaseClass).IsInstanceOfType(null));
    }

    [TestMethod]
    public void InheritanceHierarchyAndInterfaceCasesReturnTrue()
    {
        // Mirrors the official example: same type, base type vs derived instance, array vs Array, interface.
        // 与官方文档示例一致：相同类型、基类判断派生实例、数组与 Array、接口实现。
        var baseInstance = new BaseClass();
        var derivedInstance = new DerivedClass();
        int[] array = [1, 2, 3];

        Assert.IsTrue(baseInstance.GetType().IsInstanceOfType(baseInstance));
        Assert.IsTrue(baseInstance.GetType().IsInstanceOfType(derivedInstance));
        Assert.IsTrue(typeof(Array).IsInstanceOfType(array));
        Assert.IsTrue(typeof(IExample).IsInstanceOfType(baseInstance));
        Assert.IsTrue(typeof(IExample).IsInstanceOfType(derivedInstance));
    }

    [TestMethod]
    public void NonMatchingCasesReturnFalse()
    {
        // False when the current Type is absent from the instance's hierarchy: unrelated type or reversed direction.
        // 当前 Type 不在实例继承层次中时返回 false：无关类型，或方向相反（派生类判断基类实例）。
        var baseInstance = new BaseClass();
        var derivedInstance = new DerivedClass();

        Assert.IsFalse(typeof(IExample).IsInstanceOfType("text"));
        Assert.IsFalse(derivedInstance.GetType().IsInstanceOfType(baseInstance));
    }

    [TestMethod]
    public void OpenGenericTypeNeverMatches()
    {
        // Returns false when the current Type is an open generic (ContainsGenericParameters is true);
        // a constructed type is not an instance of its generic type definition.
        // 当前 Type 是开放泛型（ContainsGenericParameters 为 true）时返回 false；构造类型不是其泛型定义的实例。
        Assert.IsFalse(typeof(List<>).IsInstanceOfType(new List<int>()));
        Assert.IsTrue(typeof(List<int>).IsInstanceOfType(new List<int>()));
    }
}
