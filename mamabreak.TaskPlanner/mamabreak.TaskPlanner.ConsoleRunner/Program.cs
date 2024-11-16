using AVladyslav.TaskPlanner.Domain.Logic;
using AVladyslav.TaskPlanner.Domain.Models.enums;
using AVladyslav.TaskPlanner.Domain.Models;

class Program
{
    private static readonly SimpleTaskPlanner TaskPlanner = new SimpleTaskPlanner(new FileWorkItemsRepository());


    static void Main(string[] args)
    {
        Console.OutputEncoding = System.Text.Encoding.Default;
        Console.WriteLine("Вітаємо у Task Planner!");

        while (true)
        {
            PrintMenu();
            var choice = Console.ReadLine()?.ToUpper();

            switch (choice)
            {
                case "A":
                    AddWorkItem();
                    break;
                case "B":
                    BuildPlan();
                    break;
                case "M":
                    MarkAsCompleted();
                    break;
                case "R":
                    RemoveWorkItem();
                    break;
                case "Q":
                    Console.WriteLine("Вихiд з програми. Бувайте!");
                    return;
                default:
                    Console.WriteLine("Невалiдний вибiр. Спробуйте ще.");
                    break;
            }
        }
    }

    private static void PrintMenu()
    {
        Console.WriteLine("\nChoose an operation:");
        Console.WriteLine("[A]dd work item");
        Console.WriteLine("[B]uild a plan");
        Console.WriteLine("[M]ark work item as completed");
        Console.WriteLine("[R]emove a work item");
        Console.WriteLine("[Q]uit the app");
        Console.Write("Введiть свій вибiр: ");
    }

    private static void AddWorkItem()
    {
        Console.WriteLine("Додавання нової справи...");

        // Gather information for creating a new work item
        var title = ReadNonEmptyInput("Введiть заголовок справи: ");
        var description = ReadNonEmptyInput("Введiть опис справи: ");
        var dueDate = ReadDateTimeInput("Введiть термiн виконання (дд.мм.рррр): ");
        var priority = ReadEnumInput<Priority>("Введiть прiорiтет (None, Low, Medium, High, Urgent): ");
        var complexity = ReadEnumInput<Complexity>("Введiть складнiсть (None, Minutes, Hours, Days, Weeks): ");

        var newWorkItem = new WorkItem
        {
            Title = title,
            Description = description,
            DueDate = dueDate,
            Priority = priority,
            Complexity = complexity,
            CreationDate = DateTime.Now,
            IsCompleted = false
        };

        TaskPlanner.AddWorkItem(newWorkItem);

        Console.WriteLine("Справа успiшно додана!");
    }

    private static void BuildPlan()
    {
        Console.WriteLine("Створення плану...");

        var plannedItems = TaskPlanner.CreatePlan();

        if (plannedItems.Any())
        {
            Console.WriteLine("Запланованi справи:");
            foreach (var item in plannedItems)
            {
                Console.WriteLine(item);
            }
        }
        else
        {
            Console.WriteLine("Нема справ.");
        }
    }

    private static void MarkAsCompleted()
    {
        Console.WriteLine("Позначення справи виконаною...");

        var id = ReadGuidInput("Введiть ID справи: ");

        if (TaskPlanner.MarkAsCompleted(id))
        {
            Console.WriteLine("Справа позначена виконаною!");
        }
        else
        {
            Console.WriteLine("Справу не знайдено. Будь ласка, перевiрте ID.");
        }
    }

    private static void RemoveWorkItem()
    {
        Console.WriteLine("Видалення справи...");

        var id = ReadGuidInput("Введіь ID: ");

        if (TaskPlanner.RemoveWorkItem(id))
        {
            Console.WriteLine("Справа успішно видалена!");
        }
        else
        {
            Console.WriteLine("Справу не знайдено. Будь ласка, перевiрте ID.");
        }
    }


    private static string ReadNonEmptyInput(string prompt)
    {
        string input;
        do
        {
            Console.Write(prompt);
            input = Console.ReadLine()?.Trim();
        } while (string.IsNullOrEmpty(input));
        return input;
    }

    private static DateTime ReadDateTimeInput(string prompt)
    {
        DateTime dueDate;
        while (true)
        {
            if (DateTime.TryParseExact(ReadNonEmptyInput(prompt), "dd.MM.yyyy", null, System.Globalization.DateTimeStyles.None, out dueDate))
            {
                return dueDate;
            }
            Console.WriteLine("Невалідний формат дати. Будь ласка, використовуйте дд.мм.рррр.");
        }
    }

    private static TEnum ReadEnumInput<TEnum>(string prompt) where TEnum : struct
    {
        TEnum result;
        while (true)
        {
            if (Enum.TryParse(ReadNonEmptyInput(prompt), true, out result) && Enum.IsDefined(typeof(TEnum), result))
            {
                return result;
            }
            Console.WriteLine($"Невалідний ввід. Будь ласка, введіть валідний {typeof(TEnum).Name}.");
        }
    }

    private static Guid ReadGuidInput(string prompt)
    {
        Guid id;
        while (true)
        {
            if (Guid.TryParse(ReadNonEmptyInput(prompt), out id))
            {
                return id;
            }
            Console.WriteLine("Невалідний ID формат. Будь ласка, введіть валідний GUID.");
        }
    }
}