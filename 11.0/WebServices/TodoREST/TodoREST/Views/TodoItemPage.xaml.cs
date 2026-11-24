using TodoREST.Models;
using TodoREST.Services;

namespace TodoREST.Views
{
    [QueryProperty(nameof(TodoItem), "TodoItem")]
    public partial class TodoItemPage : ContentPage
    {
        ITodoService _todoService;
        TodoItem _todoItem;
        bool _isNewItem;

        public TodoItem TodoItem
        {
            get => _todoItem;
            set
            {
                _isNewItem = IsNewItem(value);
                _todoItem = value;
                OnPropertyChanged();
            }
        }

        public TodoItemPage(ITodoService service)
        {
            InitializeComponent();
            _todoService = service;
            BindingContext = this;
        }

        bool IsNewItem(TodoItem todoItem)
        {
            if (string.IsNullOrWhiteSpace(todoItem.Name) && string.IsNullOrWhiteSpace(todoItem.Notes))
                return true;
            return false;
        }

        async void OnSaveButtonClicked(object sender, EventArgs e)
        {
            await _todoService.SaveTaskAsync(TodoItem, _isNewItem);
            await Shell.Current.GoToAsync("..");
        }

        async void OnDeleteButtonClicked(object sender, EventArgs e)
        {
            await _todoService.DeleteTaskAsync(TodoItem);
            await Shell.Current.GoToAsync("..");
        }

        async void OnCancelButtonClicked(object sender, EventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
