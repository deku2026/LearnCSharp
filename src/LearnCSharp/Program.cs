using LearnCSharp.Topics;

if (args.Length == 0)
{
#if DEBUG
    return TopicRegistry.RunAll(args);
#else
    TopicRegistry.List();
    return 0;
#endif
}

string first = args[0];
if (first is "--help" or "-h" or "help")
{
    Console.WriteLine("usage: LearnCSharp [topic_id [extra_args...]]");
    Console.WriteLine("  (no args, release)  list every registered topic");
    Console.WriteLine("  (no args, debug)    iterate every topic; useful for F5-from-IDE");
    Console.WriteLine("  topic_id            run that topic; extra args reach its Run()");
    Console.WriteLine("  --help / -h         show this message");
    return 0;
}

string[] forwarded = args.Length > 1 ? args[1..] : [];
return TopicRegistry.Run(first, forwarded);
