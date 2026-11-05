
public class Reader
{
    public string Name;
    public string Surname;
    public string Phone;

    public Reader(string name, string surname = "", string phone = "")
    {
        Name = name;
        Surname = surname;
        Phone = phone;
    }

    public void DisplayInfo()
    {
        Console.WriteLine($"Имя: {Name} {Surname}");
        Console.WriteLine($"Телефон: {Phone}");
        Console.WriteLine("--------------------");
    }
}
