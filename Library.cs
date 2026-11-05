
using System;
using System.Collections.Generic;

public class Library
{
    private readonly object syncRoot = new();
    private readonly List<Book> books = new();
    private readonly List<Reader> readers = new();

    public IReadOnlyList<Book> Books
    {
        get
        {
            lock (syncRoot)
            {
                return books.ToArray();
            }
        }
    }

    public IReadOnlyList<Reader> Readers
    {
        get
        {
            lock (syncRoot)
            {
                return readers.ToArray();
            }
        }
    }

    public int TotalBooks
    {
        get
        {
            lock (syncRoot)
            {
                return books.Count;
            }
        }
    }

    // Задание 2. Метод приветствия
    public void ShowWelcomeMessage()
    {
        Console.WriteLine("Добро пожаловать в библиотеку!");
    }

    // Задание 4. Добавление книги
    public void AddBook(string title, string author, int year, string description = "")
    {
        lock (syncRoot)
        {
            books.Add(new Book(title, author, year, description));
        }

        Console.WriteLine("Книга успешно добавлена!");
    }

    // Задание 5. Показать все книги
    public void ShowBooks()
    {
        foreach (Book book in Books)
        {
            book.DisplayInfo();
        }
    }
    // Задание 6. Количество книг
    public int GetBookCount()
    {
        return TotalBooks;
    }

    // Задание 7. Проверка доступности книги
    public bool IsBookAvailable(string title)
    {
        lock (syncRoot)
        {
            Book? book = FindBook(title);
            return book?.IsAvailable ?? false;
        }
    }

    // Задание 8. Расчёт возраста книги
    public int CalculateBookAge(int year)
    {
        return DateTime.Now.Year - year;
    }

    // Задание 9. Поиск книги
    public List<Book> SearchBook(string value, bool searchByAuthor)
    {
        lock (syncRoot)
        {
            return books.FindAll(book =>
                string.Equals(
                    searchByAuthor ? book.Author : book.Title,
                    value,
                    StringComparison.OrdinalIgnoreCase));
        }
    }

    // Задание 10. Проверка читателя
    
    public bool IsReaderRegistered(string name)
    {
        lock (syncRoot)
        {
            return readers.Exists(reader =>
                string.Equals(reader.Name, name, StringComparison.OrdinalIgnoreCase));
        }
    }

    // Задание 11. Статический метод
    public static void ShowLibraryName()
    {
        Console.WriteLine("Название библиотеки: Центральная библиотека");
    }

    // Задание 13. Расчёт штрафа
    public static double CalculateFine(int overdueDays)
    {
        if (overdueDays < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(overdueDays), "Просрочка не может быть отрицательной.");
        }

        return overdueDays * 100.0;
    }

    // Задание 14. Сравнение количества книг
    public static int GetMaxBooks(int first, int second)
    {
        return Math.Max(first, second);
    }

    public int GetAvailableBooks()
    {
        lock (syncRoot)
        {
            int count = 0;
            foreach (Book book in books)
            {
                if (book.IsAvailable)
                {
                    count++;
                }
            }

            return count;
        }
    }

    // Задание 16. Поиск по названию
    public Book? SearchBook(string title)
    {
        return FindBook(title);
    }

    private Book? FindBook(string title)
    {
        lock (syncRoot)
        {
            return books.Find(book =>
                string.Equals(book.Title, title, StringComparison.OrdinalIgnoreCase));
        }
    }

    // Поиск по автору
    public List<Book> SearchBookByAuthor(string author)
    {
        lock (syncRoot)
        {
            return books.FindAll(book =>
                string.Equals(book.Author, author, StringComparison.OrdinalIgnoreCase));
        }
    }

    // Поиск по году
    public List<Book> SearchBook(int year)
    {
        lock (syncRoot)
        {
            return books.FindAll(book => book.Year == year);
        }
    }

    // Задание 17. Добавление читателя только по имени
    public void AddReader(string name)
    {
        lock (syncRoot)
        {
            readers.Add(new Reader(name));
        }
    }

    // По имени и фамилии
    public void AddReader(string name, string surname)
    {
        lock (syncRoot)
        {
            readers.Add(new Reader(name, surname));
        }
    }

    // По имени и телефону
    public void AddReader(string name, int phone)
    {
        lock (syncRoot)
        {
            readers.Add(new Reader(name, "", phone.ToString()));
        }
    }

    // Задание 18. Возвращение книги
   
    // Задание 22. Возврат с расчётом штрафа
    public void ReturnBook(string title, int overdueDays)
    {
        if (overdueDays < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(overdueDays), "Просрочка не может быть отрицательной.");
        }

        lock (syncRoot)
        {
            Book? book = FindBook(title);
            if (book is null || book.IsAvailable)
            {
                Console.WriteLine("Книга не найдена или не была выдана!");
                return;
            }

            book.IsAvailable = true;

            double fine = CalculateFine(overdueDays);

            Console.WriteLine("Книга успешно возвращена!");

            if (overdueDays > 0)
            {
                Console.WriteLine("Штраф за просрочку: " + fine);
            }
            else
            {
                Console.WriteLine("Просрочки нет. Штраф отсутствует.");
            }
        }
    }


    // Задание 19. Рекурсивный подсчёт книг
    public int CountBooksRecursive(int index = 0)
    {
        if (index >= TotalBooks)
        {
            return 0;
        }

        return 1 + CountBooksRecursive(index + 1);
    }

    // Задание 20. Сортировка по году
    public void SortBooksByYear()
    {
        lock (syncRoot)
        {
            books.Sort((a, b) => a.Year.CompareTo(b.Year));
            Console.WriteLine("Книги отсортированы по году издания!");
        }
    }

    // Задание 21. Выдача книги
    public void BorrowBook(string title, string readerName)
    {
        lock (syncRoot)
        {
            Book? book = FindBook(title);
            if (book is null)
            {
                Console.WriteLine("Книга не найдена!");
                return;
            }

            if (!IsReaderRegistered(readerName))
            {
                Console.WriteLine("Читатель не зарегистрирован!");
                return;
            }

            if (!book.IsAvailable)
            {
                Console.WriteLine("Книга уже выдана!");
                return;
            }

            book.IsAvailable = false;
            Console.WriteLine("Книга успешно выдана читателю " + readerName);
        }
    }

    // Задание 23. Статистика
    public void ShowStatistics()
    {
        int available = GetAvailableBooks();
        int borrowed = Books.Count - available;

        Console.WriteLine("\n===== СТАТИСТИКА =====");
        Console.WriteLine("Всего книг: " + Books.Count);
        Console.WriteLine("Доступных книг: " + available);
        Console.WriteLine("Выданных книг: " + borrowed);
        Console.WriteLine("Зарегистрированных читателей: " + Readers.Count);
        Console.WriteLine("======================");
    }

    // Задание 24. Главное меню
    public void ShowMenu()
    {
        Console.WriteLine("\n==============================");
        Console.WriteLine("      УПРАВЛЕНИЕ БИБЛИОТЕКОЙ");
        Console.WriteLine("==============================");
        Console.WriteLine("1. Добавить книгу");
        Console.WriteLine("2. Показать все книги");
        Console.WriteLine("3. Найти книгу");
        Console.WriteLine("4. Добавить читателя");
        Console.WriteLine("5. Показать читателей");
        Console.WriteLine("6. Выдать книгу");
        Console.WriteLine("7. Вернуть книгу");
        Console.WriteLine("8. Статистика библиотеки");
        Console.WriteLine("0. Выход");
        Console.WriteLine("==============================");
        Console.Write("Выберите действие: ");
    }

    public void AddReader(string name, string surname, string phone)
    {
        lock (syncRoot)
        {
            readers.Add(new Reader(name, surname, phone));
        }
    }
}
 