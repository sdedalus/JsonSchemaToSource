using System;
//using JsonClass;

namespace TestSourceGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var payload = new Payload();
            var dict = payload.Dict;
            payload.Name = "bob";

            Console.WriteLine("Types in this assembly:");
            foreach (Type t in typeof(Program).Assembly.GetTypes())
            {
                Console.WriteLine(t.FullName);
            }
        }
    }
}
