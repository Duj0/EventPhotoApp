using CommunityToolkit.Mvvm.Input;
using EventPhotoApp.Models;

namespace EventPhotoApp.PageModels
{
    public interface IProjectTaskPageModel
    {
        IAsyncRelayCommand<ProjectTask> NavigateToTaskCommand { get; }
        bool IsBusy { get; }
    }
}