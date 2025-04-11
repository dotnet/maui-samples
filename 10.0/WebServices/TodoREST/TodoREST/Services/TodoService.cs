using TodoREST.Models;

namespace TodoREST.Services
{
    public class TodoService : ITodoService
    {
        IRestService _restService;

        public TodoService(IRestService service)
        {
            _restService = service;
        }

        public Task<List<TodoItem>> GetTasksAsync()
        {
            return _restService.RefreshDataAsync();
        }

        public Task SaveTaskAsync(TodoItem item, bool isNewItem = false)
        {
            return _restService.SaveTodoItemAsync(item, isNewItem);
        }

        public Task DeleteTaskAsync(TodoItem item)
        {
            return _restService.DeleteTodoItemAsync(item.ID);
        }
    }
}
