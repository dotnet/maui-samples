using CommunityToolkit.Mvvm.Input;
using DeveloperBalance.Models;

namespace DeveloperBalance.PageModels;

public interface IProjectTaskPageModel
{
	IAsyncRelayCommand<ProjectTask> NavigateToTaskCommand { get; }
	bool IsBusy { get; }
}