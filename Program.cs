using System;
using System.Linq;

var library = new Library();
library.AddBook(
    "Чистый код",
    "Роберт Мартин",
    2008,
    "Практическое руководство по написанию понятного, поддерживаемого и качественного кода.");
library.AddBook(
    "Гарри Поттер и философский камень",
    "Дж. К. Роулинг",
    1997,
    "Первая книга о юном волшебнике Гарри Поттере и его жизни в Хогвартсе.");
library.AddBook(
    "Властелин колец",
    "Дж. Р. Р. Толкин",
    1954,
    "Эпическое фэнтези о путешествии Фродо и борьбе за судьбу Средиземья.");

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://0.0.0.0:5080");
var app = builder.Build();

app.MapGet("/", () => Results.Content(
    """
    <!doctype html>
    <html lang="ru">
    <head>
      <meta charset="utf-8">
      <meta name="viewport" content="width=device-width, initial-scale=1">
      <title>Каталог библиотеки</title>
      <style>
        :root { color-scheme: light; font-family: system-ui, sans-serif; }
        body { margin: 0; background: #f3f5f8; color: #202735; }
        main { max-width: 900px; margin: 0 auto; padding: 32px 20px; }
        h1 { margin-bottom: 8px; }
        .intro { color: #596579; margin-top: 0; }
        input { box-sizing: border-box; width: 100%; padding: 12px 14px; margin: 16px 0 20px; border: 1px solid #cbd2dc; border-radius: 8px; font: inherit; }
        #books { display: grid; grid-template-columns: repeat(auto-fit, minmax(250px, 1fr)); gap: 16px; }
        article { background: white; border: 1px solid #e0e5ec; border-radius: 10px; padding: 18px; box-shadow: 0 2px 8px #1522380a; }
        article h2 { font-size: 1.15rem; margin: 0 0 8px; }
        .meta, .status { color: #596579; font-size: .92rem; }
        .description { line-height: 1.5; }
        .empty { color: #596579; }
      </style>
    </head>
    <body>
      <main>
        <h1>Каталог библиотеки</h1>
        <p class="intro">Книги, авторы, описания и доступность.</p>
        <label for="search">Поиск по названию или автору</label>
        <input id="search" type="search" placeholder="Например, Толкин">
        <section id="books" aria-live="polite"></section>
      </main>
      <script>
        const booksContainer = document.querySelector("#books");
        const searchInput = document.querySelector("#search");
        let books = [];

        function renderBooks() {
          const query = searchInput.value.trim().toLocaleLowerCase("ru");
          const matches = books.filter(book =>
            book.title.toLocaleLowerCase("ru").includes(query) ||
            book.author.toLocaleLowerCase("ru").includes(query));

          booksContainer.replaceChildren();
          if (matches.length === 0) {
            const message = document.createElement("p");
            message.className = "empty";
            message.textContent = "Книги не найдены.";
            booksContainer.append(message);
            return;
          }

          for (const book of matches) {
            const card = document.createElement("article");
            const title = document.createElement("h2");
            title.textContent = book.title;
            const meta = document.createElement("p");
            meta.className = "meta";
            meta.textContent = `${book.author} · ${book.year} год`;
            const description = document.createElement("p");
            description.className = "description";
            description.textContent = book.description || "Описание пока не добавлено.";
            const status = document.createElement("p");
            status.className = "status";
            status.textContent = book.isAvailable ? "В наличии" : "Выдана";
            card.append(title, meta, description, status);
            booksContainer.append(card);
          }
        }

        searchInput.addEventListener("input", renderBooks);
        fetch("/api/books")
          .then(response => {
            if (!response.ok) throw new Error("Не удалось загрузить каталог.");
            return response.json();
          })
          .then(data => {
            books = data;
            renderBooks();
          })
          .catch(error => {
            booksContainer.textContent = error.message;
          });
      </script>
    </body>
    </html>
    """,
    "text/html; charset=utf-8"));

app.MapGet("/api/books", () => library.Books.Select(book => new
{
    book.Title,
    book.Author,
    book.Year,
    book.Description,
    book.IsAvailable
}).ToArray());

await app.StartAsync();
Console.WriteLine("Веб-каталог запущен: http://localhost:5080");
Console.WriteLine("В локальной сети откройте http://<IP-адрес-этого-компьютера>:5080");

try
{
    var running = true;
    while (running)
    {
        library.ShowMenu();
        var choice = Console.ReadLine();

        if (choice is null)
        {
            break;
        }

        switch (choice)
        {
            case "1":
            {
                var title = ReadRequired("Название: ");
                var author = ReadRequired("Автор: ");
                var year = ReadInteger("Год издания: ", 1, DateTime.Now.Year);
                Console.Write("Описание (можно оставить пустым): ");
                var description = Console.ReadLine() ?? string.Empty;
                library.AddBook(title, author, year, description);
                break;
            }
            case "2":
                library.ShowBooks();
                break;
            case "3":
            {
                var searchTitle = ReadRequired("Введите название книги: ");
                var found = library.SearchBook(searchTitle);
                if (found is not null)
                {
                    found.DisplayInfo();
                }
                else
                {
                    Console.WriteLine("Книга не найдена!");
                }
                break;
            }
            case "4":
            {
                var name = ReadRequired("Имя читателя: ");
                var surname = ReadRequired("Фамилия: ");
                Console.Write("Телефон: ");
                var phone = Console.ReadLine() ?? string.Empty;
                library.AddReader(name, surname, phone);
                Console.WriteLine("Читатель успешно добавлен!");
                break;
            }
            case "5":
                if (library.Readers.Count == 0)
                {
                    Console.WriteLine("Читателей пока нет.");
                }
                else
                {
                    foreach (var reader in library.Readers)
                    {
                        reader.DisplayInfo();
                    }
                }
                break;
            case "6":
            {
                var borrowTitle = ReadRequired("Название книги: ");
                var readerName = ReadRequired("Имя читателя: ");
                library.BorrowBook(borrowTitle, readerName);
                break;
            }
            case "7":
            {
                var returnTitle = ReadRequired("Название книги: ");
                var overdueDays = ReadInteger("Дней просрочки: ", 0, int.MaxValue);
                library.ReturnBook(returnTitle, overdueDays);
                break;
            }
            case "8":
                library.ShowStatistics();
                break;
            case "0":
                running = false;
                Console.WriteLine("Программа завершена. До свидания!");
                break;
            default:
                Console.WriteLine("Неверный выбор. Попробуйте снова.");
                break;
        }
    }
}
catch (EndOfStreamException)
{
    Console.WriteLine("\nВвод завершён. Останавливаю каталог.");
}
finally
{
    await app.StopAsync();
}

static string ReadRequired(string prompt)
{
    while (true)
    {
        Console.Write(prompt);
        var value = Console.ReadLine();
        if (value is null)
        {
            throw new EndOfStreamException("Ввод завершён.");
        }

        if (!string.IsNullOrWhiteSpace(value))
        {
            return value.Trim();
        }

        Console.WriteLine("Значение не должно быть пустым.");
    }
}

static int ReadInteger(string prompt, int minimum, int maximum)
{
    while (true)
    {
        Console.Write(prompt);
        var value = Console.ReadLine();
        if (value is null)
        {
            throw new EndOfStreamException("Ввод завершён.");
        }

        if (int.TryParse(value, out var number) && number >= minimum && number <= maximum)
        {
            return number;
        }

        Console.WriteLine($"Введите целое число от {minimum} до {maximum}.");
    }
}
