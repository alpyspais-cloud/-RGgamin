
using System;

public class Book
{
    public string Title;
    public string Author;
    public int Year;
    public string Description;
    public bool IsAvailable;

    // Конструктор
    public Book(string title, string author, int year, string description = "")
    {
        Title = title;
        Author = author;
        Year = year;
        Description = description;
        IsAvailable = true;
    }

    // Задание 1. Метод DisplayInfo()
    public void DisplayInfo()
    {
        Console.WriteLine($"Название: {Title}");
        Console.WriteLine($"Автор: {Author}");
        Console.WriteLine($"Год: {Year}");
        Console.WriteLine($"Описание: {(string.IsNullOrWhiteSpace(Description) ? "не указано" : Description)}");
        Console.WriteLine($"Доступна: {(IsAvailable ? "Да" : "Нет")}");
        Console.WriteLine("-------------------------");
    }

    // Задание 3. Метод SetTitle()
    public void SetTitle(string title)
    {
        Title = title;
    }
}
