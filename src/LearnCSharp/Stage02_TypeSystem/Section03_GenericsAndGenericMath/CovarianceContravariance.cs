// LearnCSharp example (filled)
// Doc      : CSharp-阶段2-类型系统-第3部分-泛型与泛型数学.md
// Stage    : Stage02_TypeSystem
// Section  : Section03_GenericsAndGenericMath
// Item     : CovarianceContravariance
// Topic id : stage02/section03/covariance_contravariance
//
// 步骤 4：默认 invariant、out 协变、in 逆变。

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage02.Section03;

internal static class CovarianceContravariance
{
    [LearnTopic("stage02/section03/covariance_contravariance")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== CovarianceContravariance ===");
        DemoInvariantList();
        DemoCovarianceOut();
        DemoContravarianceIn();
        DemoArrayCovariancePitfall();
        DemoDelegates();
        return 0;
    }

    private static void DemoInvariantList()
    {
        Console.WriteLine("-- List<T> 默认不变 --");
        List<Dog> dogs = [new Dog("Rex")];
        // List<Animal> animals = dogs; // 编译错误
        Debug.Assert(dogs[0].Name == "Rex");
        Console.WriteLine("  若允许 List<Dog>→List<Animal>，可 animals.Add(new Cat()) 破坏类型");
    }

    private static void DemoCovarianceOut()
    {
        Console.WriteLine("-- out 协变：IEnumerable<out T> --");
        IEnumerable<Dog> dogs = [new Dog("Rex"), new Dog("Max")];
        IEnumerable<Animal> animals = dogs; // 合法：只产出，不消费
        int count = 0;
        foreach (Animal a in animals)
        {
            count++;
            Debug.Assert(a is Dog);
        }
        Debug.Assert(count == 2);

        IProducer<Dog> prod = new Producer<Dog>(new Dog("P"));
        IProducer<Animal> asAnimal = prod; // out T
        Debug.Assert(asAnimal.Produce().Name == "P");
        Console.WriteLine("  IEnumerable<Dog> → IEnumerable<Animal> OK（只读产出）");
    }

    private static void DemoContravarianceIn()
    {
        Console.WriteLine("-- in 逆变：Action<in T> / IComparer --");
        IComparer<Animal> byName = Comparer<Animal>.Create((a, b) =>
            string.Compare(a.Name, b.Name, StringComparison.Ordinal));
        IComparer<Dog> dogComparer = byName; // Animal 比较器可用于 Dog
        Dog d1 = new Dog("A");
        Dog d2 = new Dog("B");
        Debug.Assert(dogComparer.Compare(d1, d2) < 0);

        IConsumer<Animal> consumer = new Consumer<Animal>();
        IConsumer<Dog> dogConsumer = consumer; // in T：能吃 Animal 就能吃 Dog
        dogConsumer.Consume(new Dog("Z"));
        Console.WriteLine("  IComparer<Animal> → IComparer<Dog> OK（更宽输入）");
    }

    private static void DemoArrayCovariancePitfall()
    {
        Console.WriteLine("-- ⚠ 数组协变运行时不安全 --");
        string[] strings = ["a", "b"];
        object[] objects = strings; // 数组协变允许
        try
        {
            objects[0] = 42; // 运行时 ArrayTypeMismatchException
            Debug.Assert(false);
        }
        catch (ArrayTypeMismatchException)
        {
            Console.WriteLine("  object[] o = string[]; o[0]=42 → ArrayTypeMismatchException ✓");
        }
        Debug.Assert(strings[0] == "a");
    }

    private static void DemoDelegates()
    {
        Console.WriteLine("-- 委托协变/逆变 --");
        Func<Dog> makeDog = () => new Dog("F");
        Func<Animal> makeAnimal = makeDog; // 协变返回值
        Debug.Assert(makeAnimal().Name == "F");

        Action<Animal> eatAnimal = a => Debug.Assert(a is not null);
        Action<Dog> eatDog = eatAnimal; // 逆变参数
        eatDog(new Dog("E"));
        Console.WriteLine("  Func 返回协变；Action 参数逆变");
    }

    private abstract class Animal(string name)
    {
        public string Name { get; } = name;
    }

    private sealed class Dog(string name) : Animal(name);
    private sealed class Cat(string name) : Animal(name);

    private interface IProducer<out T>
    {
        T Produce();
    }

    private sealed class Producer<T>(T value) : IProducer<T>
    {
        public T Produce() => value;
    }

    private interface IConsumer<in T>
    {
        void Consume(T item);
    }

    private sealed class Consumer<T> : IConsumer<T>
    {
        public void Consume(T item) => Debug.Assert(item is not null);
    }
}
