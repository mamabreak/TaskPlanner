using mamabreak.TaskPlanner.Domain.Models;
using mamabreak.TaskPlanner.DataAccess.Abstractions;
using Newtonsoft.Json;

public class FileWorkItemsRepository :IWorkItemsRepository
{
    private static string FilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "work-items.json");
    private Dictionary<Guid, WorkItem> tasksDictionary;

    public FileWorkItemsRepository()
    {
        // Initialize the dictionary
        tasksDictionary = new Dictionary<Guid, WorkItem>();

        // Read and deserialize JSON content from the file
        if (File.Exists(FilePath))
        {
            var jsonContent = File.ReadAllText(FilePath);
            if (!string.IsNullOrEmpty(jsonContent))
            {
                var workItems = JsonConvert.DeserializeObject<WorkItem[]>(jsonContent);
                tasksDictionary = workItems.ToDictionary(item => item.Id);
            }
        }
    }

    public Guid Add(WorkItem workItem)
    {
        // Create a copy of the provided work item
        var clone = workItem.Clone();

        // Generate a new Guid and assign it to the clone
        clone.Id = Guid.NewGuid();

        // Add the clone to the dictionary
        tasksDictionary.Add(clone.Id, clone);

        // Return the created ID
        return clone.Id;
    }

    public WorkItem Get(Guid id)
    {
        return tasksDictionary.TryGetValue(id, out var task) ? task : null;
    }

    public WorkItem[] GetAll()
    {
        return tasksDictionary.Values.ToArray();
    }

    public bool Update(WorkItem workItem)
    {
        if (tasksDictionary.ContainsKey(workItem.Id))
        {
            // Update the work item in the dictionary
            tasksDictionary[workItem.Id] = workItem.Clone();
            return true;
        }
        return false;
    }

    public bool Remove(Guid id)
    {
        return tasksDictionary.Remove(id);
    }

    public void SaveChanges()
    {
        try
        {
            // Convert the dictionary to an array
            var workItemsArray = tasksDictionary.Values.ToArray();

            // Check if the array is empty
            if (!workItemsArray.Any())
            {
                Console.WriteLine("No tasks to save.");
                return;
            }

            // Serialize the array to JSON
            var jsonContent = JsonConvert.SerializeObject(workItemsArray, Formatting.Indented);

            // Check if the file path is valid
            if (string.IsNullOrEmpty(FilePath) || !Path.IsPathRooted(FilePath))
            {
                Console.WriteLine("Invalid file path.");
            }

            // Write the JSON content to the file, overwriting the existing content
            File.WriteAllText(FilePath, jsonContent);
            Console.WriteLine("Tasks saved successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
    }

}