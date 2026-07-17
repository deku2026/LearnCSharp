// LearnCSharp example (filled)
// Doc      : CSharp-阶段6-异常错误处理诊断-第2部分-异常实践模式与诊断.md
// Stage    : Stage06_ExceptionsAndDiagnostics
// Section  : Section02_ExceptionPracticeAndDiagnostics
// Item     : CustomExceptionTypes
// Topic id : stage06/section02/custom_exception_types
//
// 步骤 1：自定义异常 — 继承 Exception、标准构造、领域属性、类型安全捕获

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage06.Section02;

internal static class CustomExceptionTypes
{
    [LearnTopic("stage06/section02/custom_exception_types")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== CustomExceptionTypes ===");
        DemoThrowAndCatchDomainException();
        DemoStandardConstructors();
        DemoPreferBuiltInWhenPossible();
        return 0;
    }

    private static void DemoThrowAndCatchDomainException()
    {
        Console.WriteLine("-- domain exception with structured data --");
        Account account = new Account(balance: 100m);
        try
        {
            account.Withdraw(1000m);
            Debug.Assert(false);
        }
        catch (InsufficientFundsException ex)
        {
            decimal shortfall = ex.RequestedAmount - ex.AvailableBalance;
            Debug.Assert(ex.RequestedAmount == 1000m);
            Debug.Assert(ex.AvailableBalance == 100m);
            Debug.Assert(shortfall == 900m);
            Console.WriteLine($"  shortfall = {shortfall} (req={ex.RequestedAmount}, avail={ex.AvailableBalance})");
        }
    }

    private static void DemoStandardConstructors()
    {
        Console.WriteLine("-- three standard constructors + domain ctor --");
        InsufficientFundsException a = new InsufficientFundsException();
        InsufficientFundsException b = new InsufficientFundsException("funds low");
        InsufficientFundsException c = new InsufficientFundsException("funds low", new IOException("ledger io"));
        InsufficientFundsException d = new InsufficientFundsException(50m, 10m);

        Debug.Assert(a.Message.Length >= 0);
        Debug.Assert(b.Message == "funds low");
        Debug.Assert(c.InnerException is IOException);
        Debug.Assert(d.RequestedAmount == 50m && d.AvailableBalance == 10m);
        Console.WriteLine($"  domain message: {d.Message}");
    }

    private static void DemoPreferBuiltInWhenPossible()
    {
        Console.WriteLine("-- prefer built-in types when no domain data --");
        try
        {
            ArgumentNullException.ThrowIfNull((string?)null, "userId");
        }
        catch (ArgumentNullException ex)
        {
            Debug.Assert(ex.ParamName == "userId");
            Console.WriteLine($"  use ArgumentNullException, not custom: {ex.ParamName}");
        }

        // Custom when: domain data + precise catch (InsufficientFundsException above).
        Debug.Assert(typeof(InsufficientFundsException).IsSubclassOf(typeof(Exception)));
        Debug.Assert(!typeof(InsufficientFundsException).IsSubclassOf(typeof(ApplicationException)));
        Console.WriteLine("  custom exceptions inherit Exception (not ApplicationException)");
    }

    private sealed class Account
    {
        private decimal _balance;

        public Account(decimal balance) => _balance = balance;

        public void Withdraw(decimal amount)
        {
            if (amount > _balance)
                throw new InsufficientFundsException(amount, _balance);
            _balance -= amount;
        }
    }

    /// <summary>Domain exception: inherit Exception, name ends with Exception, standard ctors + properties.</summary>
    private sealed class InsufficientFundsException : Exception
    {
        public decimal RequestedAmount { get; }
        public decimal AvailableBalance { get; }

        public InsufficientFundsException()
        {
        }

        public InsufficientFundsException(string message)
            : base(message)
        {
        }

        public InsufficientFundsException(string message, Exception inner)
            : base(message, inner)
        {
        }

        public InsufficientFundsException(decimal requested, decimal available)
            : base($"need {requested} but only {available} available")
        {
            RequestedAmount = requested;
            AvailableBalance = available;
        }
    }
}
