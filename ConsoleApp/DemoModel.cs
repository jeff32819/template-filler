namespace ConsoleApp
{
    public class DemoModel
    {
        public class UserModel
        {
            public string Name { get; set; } = "";
        }
        public UserModel User { get; set; } = new();
        public string Code { get; set; } = "";
        public string HelloWorld { get; set; } = "";
    }
}
