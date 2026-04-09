using ConsoleApp;
using Jeff32819DLL.TemplateFiller20;



var p = new Parser()
    .AddText("Here is a sentence for customer {{customer_name}} for move on date {{datetime}} and another tag {{another}}")
    .SetValues(new
    {
        CustomerName = "bob smith",
        Another = "another value",
        Code = "ABC123",
        DateTime = DateTime.Now.ToString("yyyy-MM-dd"),
        HelloWorld = "hello world 123",
        Hello_World = "hello world 456",
        User = new DemoModel.UserModel
        {
            Name = "bob smith"
        }
    });
    p.Apply();
Console.WriteLine(p);

foreach (var item in p.Tags.KeyList)
{
    Console.WriteLine($" - {item}");
}


Console.ReadLine();



var parser = new TagParser();
parser.Parse("text1", "Here is another sentence for customer {{customer_name}} for move on date {{datetime}} and another tag {{another}}");
//parser.Parse("text2", "Code: {{code}}, Hello_World: {{hello_world}}");
//parser.SetValue("customer_name", "bob smith");
parser.SetValues(new
{
    CustomerName = "bob smith",
    Another = "another value",
    Code = "ABC123",
    DateTime = DateTime.Now.ToString("yyyy-MM-dd"),
    HelloWorld = "hello world 123",
    Hello_World = "hello world 456",
    User = new DemoModel.UserModel
    {
        Name = "bob smith"
    }
});
var result = parser.Apply();

parser.Debug();

Console.WriteLine();
Console.WriteLine("--- start TagList ---");
foreach (var tag in parser.TagDictionary.KeyList)
{
    Console.WriteLine($" - {tag}");
}

Console.WriteLine("--- end TagLiist ---");
Console.WriteLine();

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