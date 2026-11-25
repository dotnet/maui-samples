using TodoREST.Views;

namespace TodoREST;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		Routing.RegisterRoute(nameof(TodoItemPage), typeof(TodoItemPage));
	}
}
