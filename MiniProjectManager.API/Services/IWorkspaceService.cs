using MiniProjectManager.API.Entities;

namespace MiniProjectManager.API.Services
{
    
    public interface IWorkspaceService
    {
        Task<IEnumerable<Workspace>> GetAllWorkspace(int userId);

        Task<Workspace> CreateWorkspace(string name, int userId);
    }
}