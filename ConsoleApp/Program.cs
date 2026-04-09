using ConsoleApp;
using Jeff32819DLL.TemplateFiller20;

var p = new Parser()
    .AddText("Here is a sentence for customer {{customer_name}}, saying {{hello-world}} for move on date {{datetime}} and another tag {{another}}")
    .SetValues(new
    {
        CustomerName = "bob smith",
        Another = "another value",
        Code = "ABC123",
        DateTime = DateTime.Now.ToString("yyyy-MM-dd"),
        HelloWorld = "hello world 123",
        User = new DemoModel.UserModel
        {
            Name = "bob smith"
        }
    });
Console.WriteLine(p.ParseTemplate());

foreach (var item in p.Tags.KeyList)
{
    Console.WriteLine($" - {item}");
}

Console.WriteLine();
Console.WriteLine("------------------------------------------------------------------------------------------------------");
Console.WriteLine();
Console.ReadKey();