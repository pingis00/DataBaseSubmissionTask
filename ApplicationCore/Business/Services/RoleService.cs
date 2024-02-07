using ApplicationCore.Business.Dtos;
using ApplicationCore.Business.Helpers;
using ApplicationCore.Business.Interfaces;
using ApplicationCore.Infrastructure.Entities;
using ApplicationCore.Infrastructure.Interfaces;
using ApplicationCore.Infrastructure.Repositories;
using System.Diagnostics;
using System.Linq.Expressions;
using System.Net;

namespace ApplicationCore.Business.Services;

public class RoleService(IRoleRepository roleRepository) : IRoleService
{
    private readonly IRoleRepository _roleRepository = roleRepository;

    public async Task<OperationResult<RoleDto>> CreateRoleAsync(RoleDto role)
    {
        try
        {
            var existingRole = await _roleRepository.GetOneAsync(a =>
            a.Id == role.Id &&
            a.RoleName == role.RoleName);

            if (existingRole != null)
            {
                return OperationResult<RoleDto>.Success("Rollen finns redan i systemet.", new RoleDto
                { Id = existingRole.Id, RoleName = existingRole.RoleName });
            }
            else
            {
                var newRoleEntity = await _roleRepository.CreateAsync(new RoleEntity
                {
                    Id = role.Id,
                    RoleName = role.RoleName
                });

                var newRoleDto = new RoleDto
                {
                    Id = newRoleEntity.Id,
                    RoleName= newRoleEntity.RoleName
                };

                return OperationResult<RoleDto>.Success("Rollen skapades framgångrikt", newRoleDto);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine("ERROR :: " + ex.Message);
            return OperationResult<RoleDto>.Failure("Ett internt fel inträffade när rollen skulle skapas.");
        }
    }

    public Task<OperationResult<bool>> DeleteRoleAsync(int roleId)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<IEnumerable<RoleDto>>> GetAllRolesAsync()
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<RoleDto>> GetRolesByIdAsync(int roleId)
    {
        throw new NotImplementedException();
    }

    public Task<OperationResult<RoleDto>> UpdateRoleAsync(RoleDto addressDto)
    {
        throw new NotImplementedException();
    }
}
