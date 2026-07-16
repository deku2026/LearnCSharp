// LearnCSharp example (filled)
// Doc      : CSharp-阶段3-成员与OOP-第3部分-继承多态接口.md
// Stage    : Stage03_MembersAndOOP
// Section  : Section03_InheritancePolymorphismInterfaces
// Item     : AbstractClasses
// Topic id : stage03/section03/abstract_classes
//
// 步骤 4：抽象类、模板方法、字段/构造/具体+抽象成员。

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage03.Section03;

internal static class AbstractClasses
{
    [LearnTopic("stage03/section03/abstract_classes")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== AbstractClasses ===");
        DemoCannotInstantiate();
        DemoTemplateMethod();
        DemoMultipleConcrete();
        return 0;
    }

    private static void DemoCannotInstantiate()
    {
        Console.WriteLine("-- 不能 new 抽象类 --");
        Vehicle v = new Car("ABC123");
        Debug.Assert(v.Plate == "ABC123");
        Debug.Assert(v.Wheels == 4);
        Debug.Assert(v.Drive() == "Driving car ABC123");
        Debug.Assert(v.Honk() == "Beep");
        Console.WriteLine($"  {v.Drive()}, wheels={v.Wheels}");
        // var x = new Vehicle("X"); // ❌
    }

    private static void DemoTemplateMethod()
    {
        Console.WriteLine("-- 模板方法：具体流程调抽象步骤 --");
        DataImporter csv = new CsvImporter();
        DataImporter json = new JsonImporter();
        Debug.Assert(csv.Import() == "parse-csv|store");
        Debug.Assert(json.Import() == "parse-json|store");
        Console.WriteLine($"  csv={csv.Import()}, json={json.Import()}");
    }

    private static void DemoMultipleConcrete()
    {
        Console.WriteLine("-- 多个具体派生共享状态与行为 --");
        Vehicle[] fleet = [new Car("C1"), new Bike("B1")];
        Debug.Assert(fleet[0].Wheels == 4);
        Debug.Assert(fleet[1].Wheels == 2);
        Console.WriteLine($"  fleet: {string.Join(", ", fleet.Select(v => $"{v.Plate}/{v.Wheels}"))}");
    }

    private abstract class Vehicle
    {
        public string Plate { get; }
        protected Vehicle(string plate) => Plate = plate;
        public abstract int Wheels { get; }
        public abstract string Drive();
        public string Honk() => "Beep";
    }

    private sealed class Car : Vehicle
    {
        public Car(string plate) : base(plate) { }
        public override int Wheels => 4;
        public override string Drive() => $"Driving car {Plate}";
    }

    private sealed class Bike : Vehicle
    {
        public Bike(string plate) : base(plate) { }
        public override int Wheels => 2;
        public override string Drive() => $"Riding bike {Plate}";
    }

    private abstract class DataImporter
    {
        public string Import()
        {
            string data = Parse();
            return $"{data}|store";
        }

        protected abstract string Parse();
    }

    private sealed class CsvImporter : DataImporter
    {
        protected override string Parse() => "parse-csv";
    }

    private sealed class JsonImporter : DataImporter
    {
        protected override string Parse() => "parse-json";
    }
}
