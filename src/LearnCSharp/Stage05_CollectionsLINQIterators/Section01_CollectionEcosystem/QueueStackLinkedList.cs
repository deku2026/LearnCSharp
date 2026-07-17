// LearnCSharp example (filled)
// Doc      : CSharp-阶段5-集合LINQ迭代器-第1部分-集合体系.md
// Stage    : Stage05_CollectionsLINQIterators
// Section  : Section01_CollectionEcosystem
// Item     : QueueStackLinkedList
// Topic id : stage05/section01/queue_stack_linkedlist
//
// 步骤 5：Queue FIFO / Stack LIFO / LinkedList 双链表。

using System.Diagnostics;
using LearnCSharp.Topics;

namespace LearnCSharp.Stage05.Section01;

internal static class QueueStackLinkedList
{
    [LearnTopic("stage05/section01/queue_stack_linkedlist")]
    internal static int Run(string[] args)
    {
        _ = args;
        Console.WriteLine("=== QueueStackLinkedList ===");
        DemoQueue();
        DemoStack();
        DemoLinkedList();
        DemoBracketMatch();
        return 0;
    }

    private static void DemoQueue()
    {
        Console.WriteLine("-- Queue：Enqueue / Dequeue / Peek (FIFO) --");
        Queue<int> q = new();
        q.Enqueue(1);
        q.Enqueue(2);
        q.Enqueue(3);
        Debug.Assert(q.Peek() == 1);
        Debug.Assert(q.Dequeue() == 1);
        Debug.Assert(q.Dequeue() == 2);
        Debug.Assert(q.Count == 1);
        Console.WriteLine($"  after two Dequeue, Peek={q.Peek()}");
    }

    private static void DemoStack()
    {
        Console.WriteLine("-- Stack：Push / Pop / Peek (LIFO) --");
        Stack<int> s = new();
        s.Push(1);
        s.Push(2);
        s.Push(3);
        Debug.Assert(s.Peek() == 3);
        Debug.Assert(s.Pop() == 3);
        Debug.Assert(s.Pop() == 2);
        Console.WriteLine($"  after two Pop, Peek={s.Peek()}");
    }

    private static void DemoLinkedList()
    {
        Console.WriteLine("-- LinkedList：已知节点 O(1) 插删 --");
        LinkedList<int> ll = new();
        LinkedListNode<int> node = ll.AddLast(1);
        ll.AddFirst(0);
        ll.AddAfter(node, 2);
        Debug.Assert(ll.SequenceEqual([0, 1, 2]));
        ll.Remove(node);
        Debug.Assert(ll.SequenceEqual([0, 2]));
        Console.WriteLine($"  list → [{string.Join(", ", ll)}]");
    }

    private static void DemoBracketMatch()
    {
        Console.WriteLine("-- Stack 括号匹配 --");
        Debug.Assert(IsBalanced("()[]{}"));
        Debug.Assert(IsBalanced("({[]})"));
        Debug.Assert(!IsBalanced("(]"));
        Debug.Assert(!IsBalanced("((("));
        Console.WriteLine("  ()[]{} / ({[]}) ok; (] / ((( fail");
    }

    private static bool IsBalanced(string s)
    {
        Stack<char> st = new();
        foreach (char c in s)
        {
            if (c is '(' or '[' or '{')
            {
                st.Push(c);
                continue;
            }
            if (st.Count == 0)
                return false;
            char open = st.Pop();
            if ((c == ')' && open != '(') || (c == ']' && open != '[') || (c == '}' && open != '{'))
                return false;
        }
        return st.Count == 0;
    }
}
