using mamabreak.TaskPlanner.Domain.Models;
using mamabreak.TaskPlanner.DataAccess.Abstractions;

namespace mamabreak.TaskPlanner.Domain.Logic
{
    public class SimpleTaskPlanner
    {
        private readonly IWorkItemsRepository _workItemsRepository;

        public SimpleTaskPlanner(IWorkItemsRepository workItemsRepository)
        {
            _workItemsRepository = workItemsRepository ?? throw new ArgumentNullException(nameof(workItemsRepository));
        }

        public WorkItem[] CreatePlan()
        {
            // Get all tasks from the repository
            var allTasks = _workItemsRepository.GetAll();

            // Filter out completed tasks
            var relevantTasks = allTasks.Where(task => !task.IsCompleted).ToArray();

            // Sort the relevant tasks
            var sortedTasks = relevantTasks.OrderBy(task => task.Priority)
                                           .ThenBy(task => task.DueDate)
                                           .ThenBy(task => task.Title)
                                           .ToArray();

            return sortedTasks;
        }

        public Guid AddWorkItem(WorkItem workItem)
        {
            if (workItem == null)
            {
                throw new ArgumentNullException(nameof(workItem));
            }

            // Add the work item to the repository and return the assigned ID
            return _workItemsRepository.Add(workItem);
        }

        public bool MarkAsCompleted(Guid id)
        {
            var workItem = _workItemsRepository.Get(id);

            if (workItem != null)
            {
                // Mark the work item as completed
                workItem.IsCompleted = true;
                // Update the work item in the repository
                return _workItemsRepository.Update(workItem);
            }

            return false;
        }

        public bool RemoveWorkItem(Guid id)
        {
            // Remove the work item from the repository
            return _workItemsRepository.Remove(id);
        }
    }
}