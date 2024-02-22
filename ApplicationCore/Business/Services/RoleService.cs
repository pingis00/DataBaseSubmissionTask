using ApplicationCore.Business.Dtos;
using ApplicationCore.Business.Helpers;
using ApplicationCore.Business.Interfaces;
using ApplicationCore.Infrastructure.Contexts;
using ApplicationCore.Infrastructure.Entities;
using ApplicationCore.Infrastructure.Interfaces;
using System.Diagnostics;

namespace ApplicationCore.Business.Services;

public class RoleService(IRoleRepository roleRepository) : IRoleService
{
    private readonly IRoleRepository _roleRepository = roleRepository;

    public async Task<OperationResult<RoleDto>> CreateRoleAsync(RoleDto role)
    {
        try
        {
            var normalizedRoleName = TextNormalizationHelper.NormalizeText(role.RoleName).Data;
            var existingRoleResult = await _roleRepository.GetOneAsync(r => r.RoleName == normalizedRoleName);

            if (existingRoleResult.IsSuccess && existingRoleResult.Data != null)
            {
                var roleDto = new RoleDto
                {
                    Id = existingRoleResult.Data.Id,
                    RoleName = existingRoleResult.Data.RoleName

                };

                return OperationResult<RoleDto>.Success("Rollen finns redan i systemet.", roleDto);
            }
            else
            {
                var newRoleEntityResult = await _roleRepository.CreateAsync(new RoleEntity
                {
                    RoleName = normalizedRoleName
                });

                if (!newRoleEntityResult.IsSuccess)
                {
                    return OperationResult<RoleDto>.Failure("Det gick inte att skapa rollen.");
                }
                var newRoleDto = ConvertToDto(newRoleEntityResult.Data);

                return OperationResult<RoleDto>.Success("Rollen skapades framgångrikt", newRoleDto);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine("ERROR :: " + ex.Message);
            return OperationResult<RoleDto>.Failure("Ett internt fel inträffade när rollen skulle skapas.");
        }
    }

    public async Task<OperationResult<bool>> DeleteRoleAsync(int roleId)
    {
        try
        {
            var roleToDeleteResult = await GetRolesByIdAsync(roleId);

            if (!roleToDeleteResult.IsSuccess)
            {
                return OperationResult<bool>.Failure("Rollen kunde inte hittas.");
            }

            OperationResult<bool> hasCustomersResult = await RoleHasCustomersAsync(roleId);
            if (!hasCustomersResult.IsSuccess)
            {
                return OperationResult<bool>.Failure("Rollen kan inte raderas eftersom den är kopplad till en eller flera kunder.");
            }
            var roleToDelete = roleToDeleteResult.Data;


            var result = await _roleRepository.DeleteAsync(r => r.Id == roleToDelete.Id);
            if (result.IsSuccess)
            {
                return OperationResult<bool>.Success("Rollen raderades framgångsrikt.", true);
            }
            else
            {
                return OperationResult<bool>.Failure("Det uppstod ett problem vid radering av rollen.");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine("ERROR :: " + ex.Message);
            return OperationResult<bool>.Failure("Ett internt fel inträffade när rollen skulle raderas.");
        }
    }

    public async Task<OperationResult<IEnumerable<RoleDto>>> GetAllRolesAsync()
    {
        try
        {
            var roleEntitiesResult = await _roleRepository.GetAllAsync();

            if (roleEntitiesResult.IsSuccess && roleEntitiesResult.Data != null)
            {
                var rolesDto = roleEntitiesResult.Data.Select(ConvertToDto).ToList();

                if (rolesDto.Any())
                {
                    return OperationResult<IEnumerable<RoleDto>>.Success("Rollerna hämtades framgångsrikt.", rolesDto);
                }
                else
                {
                    return OperationResult<IEnumerable<RoleDto>>.Failure("Inga roller hittades.");
                }
            }
            else
            {
                return OperationResult<IEnumerable<RoleDto>>.Failure("Det gick inte att hämta rollerna.");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine("ERROR :: " + ex.Message);
            return OperationResult<IEnumerable<RoleDto>>.Failure("Ett internt fel inträffade när adresserna skulle hämtas.");
        }
    }

    public async Task<OperationResult<RoleDto>> GetRolesByIdAsync(int roleId)
    {
        try
        {
            var roleResult = await _roleRepository.GetOneAsync(r => r.Id == roleId);
            if (!roleResult.IsSuccess || roleResult.Data == null)
            {
                return OperationResult<RoleDto>.Failure("Rollen kunde inte hittas.");
            }

            var roleDto = ConvertToDto(roleResult.Data);
            return OperationResult<RoleDto>.Success("Rollen hämtades framgångsrikt.", roleDto);
        }
        catch (Exception ex)
        {
            Debug.WriteLine("ERROR :: " + ex.Message);
            return OperationResult<RoleDto>.Failure("Ett internt fel inträffade när rollen hämtades.");
        }
    }

    public async Task<OperationResult<bool>> RoleHasCustomersAsync(int roleId)
    {
        bool hasCustomers = await _roleRepository.HasCustomersAsync(roleId);
        if (hasCustomers)
        {
            return OperationResult<bool>.Failure("Det finns kunder kopplade till rollen.");
        }
        return OperationResult<bool>.Success("Det finns inga kunder kopplade till Rollen.");
    }

    public async Task<OperationResult<RoleDto>> UpdateRoleAsync(RoleDto roleDto)
    {
        try
        {
            var getRoleResult = await _roleRepository.GetOneAsync(r => r.Id == roleDto.Id);

            if (!getRoleResult.IsSuccess)
            {
                return OperationResult<RoleDto>.Failure("Rollen kunde inte hittas.");
            }

            var entityToUpdate = getRoleResult.Data;

            if (entityToUpdate != null)
            {
                entityToUpdate.RoleName = roleDto.RoleName;

                var updateResult = await _roleRepository.UpdateAsync(
                    r => r.Id == entityToUpdate.Id,
                    entityToUpdate
                );

                if (!updateResult.IsSuccess)
                {
                    return OperationResult<RoleDto>.Failure("Det gick inte att uppdatera rollen.");
                }

                var updatedDto = ConvertToDto(updateResult.Data);
                return OperationResult<RoleDto>.Success("Rollen uppdaterades framgångsrikt.", updatedDto);
            }
            else
            {
                return OperationResult<RoleDto>.Failure("Rollen kunde inte hittas.");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine("ERROR :: " + ex.Message);
            return OperationResult<RoleDto>.Failure("Ett internt fel inträffade när rollen skulle uppdateras.");
        }
    }

    private RoleDto ConvertToDto(RoleEntity roleEntity)
    {
        return new RoleDto
        {
            Id = roleEntity.Id,
            RoleName = roleEntity.RoleName
        };
    }
}
