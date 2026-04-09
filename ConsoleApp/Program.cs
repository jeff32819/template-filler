using ConsoleApp;
using Jeff32819DLL.TemplateFiller20;

var parser = new TagParser();
parser.Parse("Here is another sentence for customer {{customer_name}} for move on date {{datetime}}");
parser.Parse("Code: {{code}}, Hello_World: {{hello_world}}, User.Name: {{user.name}}, User.Name with brackets: {{user.name}}");

parser.TagDictionary.SetValue("customer_name", "bob smith");
parser.TagDictionary.SetValue(new
{
    Code = "ABC123",
    HelloWorld = "hello world 123",
    Hello_World = "hello world 456",
    User = new DemoModel.UserModel
    {
        Name = "bob smith"
    }
});


var result = parser.Apply();

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
Console.ReadKey();