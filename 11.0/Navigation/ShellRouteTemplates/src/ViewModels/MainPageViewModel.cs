using CommunityToolkit.Mvvm.Input;
using ShellRouteTemplates.Models;

namespace ShellRouteTemplates.ViewModels;

public partial class MainPageViewModel
{
	public IReadOnlyList<RouteExample> Examples => RouteCatalog.Examples;

	[RelayCommand]
	private static Task NavigateAsync(RouteExample example) =>
		Shell.Current.GoToAsync(example.NavigationUri);
}
