using MiniProjectManager.API.DTOs;
using MiniProjectManager.API.Entities;

namespace MiniProjectManager.API.Services
{

    public interface IWorkspaceService
    {
        Task<IEnumerable<Workspace>> GetAllWorkspace(int userId);

        Task<WorkspaceResponseDto> CreateWorkspace(string name, int userId);

        Task<bool> Addmember(int workspaceId, AddMemberRequestDto request, int CurrentUserId);

        Task<bool> IsLeaderAsync(int workspaceId, int userId);

        Task<IEnumerable<WorkspaceResponseDto>> GetUserWorkspacesAsync(int userId);

        Task<WorkspaceResponseDto?> UpdateWorkspaceAsync(int id, string name, int userId);
        Task<bool> DeleteWorkspaceAsync(int id, int userId);
        Task<bool> AddMemberByUsernameAsync(int workspaceId, string username);
    }
}