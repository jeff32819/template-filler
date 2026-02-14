using ConsoleApp;
using Jeff32819DLL.TemplateFiller20;

var parser = new TagParser("Hello {{ user.name }}, your code is {{ code }}. {{ hello_world }} {{ user.name }}");
var result = parser.Apply(new DemoModel
{
    Code = "ABC123",
    HelloWorld = "hello world 123",
    User = new DemoModel.UserModel
    {
        Name = "bob smith"
    }
});

Console.WriteLine();

if (result.AllTagsReplaced)
{
    Console.WriteLine("all tags replaced");
}
else
{
    Console.WriteLine("Tags not found");
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine();
    foreach (var tag in result.TagsNotReplaced)
    {
        Console.WriteLine($" - {tag}");
    }

    Console.ResetColor();
}

Console.WriteLine();
Console.WriteLine("output:");
Console.WriteLine();
Console.WriteLine(result.Text);

Console.WriteLine();
Console.WriteLine("------------------------------------------------------------------------------------------------------");
Console.WriteLine();
var result2 = parser.Apply(new DemoModel // test another model with different values, but same tags, to show that the parser can be reused
{
    Code = "my code",
    HelloWorld = "first text",
    User = new DemoModel.UserModel
    {
        Name = "jeff m"
    }
});

Console.WriteLine(result2);
Console.ReadKey();