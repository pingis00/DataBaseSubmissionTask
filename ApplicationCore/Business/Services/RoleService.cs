﻿using ApplicationCore.Business.Dtos;
using ApplicationCore.Business.Helpers;
using ApplicationCore.Business.Interfaces;
using ApplicationCore.Infrastructure.Entities;
using ApplicationCore.Infrastructure.Interfaces;
using ApplicationCore.Infrastructure.Repositories;
using System.Diagnostics;

namespace ApplicationCore.Business.Services;

public class RoleService(IRoleRepository roleRepository) : IRoleService
{
    private readonly IRoleRepository _roleRepository = roleRepository;

    public async Task<OperationResult<RoleDto>> CreateRoleAsync(RoleDto role)
    {
        try
        {
            var existingRoleResult = await GetRolesByIdAsync(role.Id);

            if (existingRoleResult.IsSuccess && existingRoleResult.Data != null)
            {
                return OperationResult<RoleDto>.Success("Rollen finns redan i systemet.", existingRoleResult.Data);
            }
            else
            {
                var normalizedRoleName = TextNormalizationHelper.NormalizeText(role.RoleName).Data;

                var newRoleEntityResult = await _roleRepository.CreateAsync(new RoleEntity
                {
                    RoleName = normalizedRoleName
                });

                if (!newRoleEntityResult.IsSuccess)
                {
                    return OperationResult<RoleDto>.Failure("Det gick inte att skapa rollen.");
                }
                var newRoleEntity = newRoleEntityResult.Data;

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

    public async Task<OperationResult<bool>> DeleteRoleAsync(int roleId)
    {
        try
        {
            var roleToDeleteResult = await GetRolesByIdAsync(roleId);

            if (!roleToDeleteResult.IsSuccess)
            {
                return OperationResult<bool>.Failure("Rollen kunde inte hittas.");
            }
            var roleToDelete = roleToDeleteResult.Data;


            var result = await _roleRepository.DeleteAsync(a => a.Id == roleToDelete.Id);
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
                var rolesDto = roleEntitiesResult.Data.Select(roleEntity => new RoleDto
                {
                    Id = roleEntity.Id,
                    RoleName = roleEntity.RoleName,
                }).ToList();

                if (rolesDto.Any())
                {
                    return OperationResult<IEnumerable<RoleDto>>.Success("Adresser hämtades framgångsrikt.", rolesDto);
                }
                else
                {
                    return OperationResult<IEnumerable<RoleDto>>.Failure("Inga adresser hittades.");
                }
            }
            else
            {
                return OperationResult<IEnumerable<RoleDto>>.Failure("Det gick inte att hämta adresserna.");
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
            if (roleResult.IsSuccess && roleResult.Data != null)
            {
                var role = roleResult.Data;
                var roleDto = new RoleDto
                {
                    Id = role.Id,
                    RoleName = role.RoleName
                };
                return OperationResult<RoleDto>.Success("Rollen hämtades framgångsrikt.", roleDto);
            }
            else
            {
                return OperationResult<RoleDto>.Failure("Rollen kunde inte hittas.");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine("ERROR :: " + ex.Message);
            return OperationResult<RoleDto>.Failure("Ett internt fel inträffade när rollen hämtades.");
        }
    }

    public async Task<OperationResult<RoleDto>> UpdateRoleAsync(RoleDto roleDto)
    {
        try
        {
            return await CreateRoleAsync(roleDto);
        }
        catch (Exception ex)
        {
            Debug.WriteLine("ERROR :: " + ex.Message);
            return OperationResult<RoleDto>.Failure("Ett internt fel inträffade när rollen skulle uppdateras.");
        }
    }
}
