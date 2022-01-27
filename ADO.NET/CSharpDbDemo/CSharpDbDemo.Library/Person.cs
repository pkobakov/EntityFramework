namespace CSharpDbDemo.Library
{
    public class Person
    {
        public Person(string name, string family, int age, string email)
        {
            Name = name;
            Family = family;
            Age = age;
            Email = email;
        }

        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string Name { get; set; }
        public string Family { get; set; }
        public int Age { get; set; }
        public string Email { get; set; }

        public override string ToString()
        {
            return $"Id: {Id}, Name: {Name}, Family: {Family}, Age: {Age}, Email: {Email}";
        }
    }
}