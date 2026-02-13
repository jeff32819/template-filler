using Jeff32819DLL.TemplateFiller20;

var parser = new TagParser("Hello {{ name }}, your code is {{ code }}. {{ CamelCase }} 22222222222 {{ test-not-found }} {{ hello_world }} {{ my-var }}");
var result = parser.Apply(new { Name = "Jeff", Code = "ABC123", HelloWorld = "hello world 123", MyVar = "my var value", CamelCase = "my camel case" });

Console.WriteLine();

if (parser.AllTagsReplaced)
{
    Console.WriteLine("all tags replaced");
}
else
{
    Console.WriteLine("Tags not found");
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine();
    foreach (var tag in parser.TagsNotReplaced)
    {
        Console.WriteLine($" - {tag}");
    }
    Console.ResetColor();
}

Console.WriteLine();
Console.WriteLine("output:");
Console.WriteLine();
Console.WriteLine(result);
Console.ReadKey();
