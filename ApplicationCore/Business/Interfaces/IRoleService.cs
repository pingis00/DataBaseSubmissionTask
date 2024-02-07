using ApplicationCore.Business.Dtos;
using ApplicationCore.Business.Helpers;

namespace ApplicationCore.Business.Interfaces;

public interface IRoleService
{
    Task<OperationResult<RoleDto>> CreateRoleAsync(RoleDto role);
    Task<OperationResult<RoleDto>> GetRolesByIdAsync(int roleId);
    Task<OperationResult<IEnumerable<RoleDto>>> GetAllRolesAsync();
    Task<OperationResult<RoleDto>> UpdateRoleAsync(RoleDto addressDto);
    Task<OperationResult<bool>> DeleteRoleAsync(int roleId);
}
