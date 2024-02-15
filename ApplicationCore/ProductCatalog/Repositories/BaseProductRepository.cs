using ApplicationCore.Business.Helpers;
using ApplicationCore.Infrastructure.Interfaces;
using ApplicationCore.ProductCatalog.Context;
using ApplicationCore.ProductCatalog.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using System;
using System.Diagnostics;
using System.Linq.Expressions;

namespace ApplicationCore.ProductCatalog.Repositories;

public abstract class BaseProductRepository<TEntity> : IDataRepository<TEntity> where TEntity : class
{
    private readonly DataContext _context;

    protected BaseProductRepository(DataContext context)
    {
        _context = context;
    }

    public virtual async Task<OperationResult<TEntity>> ProductCreateAsync(TEntity entity)
    {
        try
        {
            await _context.Set<TEntity>().AddAsync(entity);
            await _context.SaveChangesAsync();
            return OperationResult<TEntity>.Success("Entiteten har skapats.", entity);
        }
        catch (Exception ex)
        {
            Debug.WriteLine("ERROR :: " + ex.Message);
            return OperationResult<TEntity>.Failure("Ett fel uppstod när entiteten skulle skapas: " + ex.Message);
        }
    }

    public virtual async Task<OperationResult<bool>> ProductDeleteAsync(Expression<Func<TEntity, bool>> predicate)
    {
        try
        {
            var entityToDelete = await _context.Set<TEntity>().FirstOrDefaultAsync(predicate);
            if (entityToDelete != null)
            {
                _context.Set<TEntity>().Remove(entityToDelete);
                await _context.SaveChangesAsync();
                return OperationResult<bool>.Success("Entiteten har tagits bort.", true);
            }
            return OperationResult<bool>.Failure("Entiteten kunde inte hittas för borttagning.");

        }
        catch (Exception ex)
        {
            Debug.WriteLine("ERROR :: " + ex.Message);
            return OperationResult<bool>.Failure("Ett fel uppstod när entiteten skulle tas bort: " + ex.Message);
        }
    }

    public virtual async Task<OperationResult<IEnumerable<TEntity>>> ProductFindAsync(Expression<Func<TEntity, bool>> predicate)
    {
        try
        {
            var filteredEntities = await _context.Set<TEntity>().Where(predicate).ToListAsync();
            if (filteredEntities != null)
            {
                return OperationResult<IEnumerable<TEntity>>.Success("Filtrerade entiteter har hämtats.", filteredEntities);
            }
            return OperationResult<IEnumerable<TEntity>>.Failure("Inga filtrerade entiteter hittades.");
        }
        catch (Exception ex)
        {
            Debug.WriteLine("ERROR :: " + ex.Message);
            return OperationResult<IEnumerable<TEntity>>.Failure($"Ett fel uppstod när de filtrerade entiteterna skulle hämtas: {ex.Message}");
        }
    }

    public virtual async Task<OperationResult<IEnumerable<TEntity>>> ProductGetAllAsync()
    {
        try
        {
            var existingEntities = await _context.Set<TEntity>().ToListAsync();
            if (existingEntities != null)
            {
                return OperationResult<IEnumerable<TEntity>>.Success("Entiteterna har hämtats.", existingEntities);
            }
            return OperationResult<IEnumerable<TEntity>>.Failure("Entiteterna hittades inte.");

        }
        catch (Exception ex)
        {
            Debug.WriteLine("ERROR :: " + ex.Message);
            return OperationResult<IEnumerable<TEntity>>.Failure("Ett fel uppstod när entiteterna skulle hämtas: " + ex.Message);
        }
    }

    public virtual async Task<OperationResult<TEntity>> ProductGetOneAsync(Expression<Func<TEntity, bool>> predicate)
    {
        try
        {
            var existingEntity = await _context.Set<TEntity>().FirstOrDefaultAsync(predicate);
            if (existingEntity != null)
            {
                return OperationResult<TEntity>.Success("Entiteten har hittats.", existingEntity);
            }
            return OperationResult<TEntity>.Failure("Entiteten kunde inte hittas.");
        }
        catch (Exception ex)
        {
            Debug.WriteLine("ERROR :: " + ex.Message);
            return OperationResult<TEntity>.Failure("Ett fel uppstod när entiteten skulle hämtas: " + ex.Message);
        }
    }

    public virtual async Task<OperationResult<TEntity>> ProductUpdateAsync(Expression<Func<TEntity, bool>> predicate, TEntity entity)
    {
        try
        {
            var entityToUpdate = await _context.Set<TEntity>().FirstOrDefaultAsync(predicate);
            if (entityToUpdate != null)
            {
                _context.Entry(entityToUpdate).CurrentValues.SetValues(entity);
                await _context.SaveChangesAsync();
                return OperationResult<TEntity>.Success("Entiteten har uppdaterats.", entityToUpdate);
            }
            return OperationResult<TEntity>.Failure("Entiteten kunde inte hittas för uppdatering.");
        }
        catch (Exception ex)
        {
            Debug.WriteLine("ERROR :: " + ex.Message);
            return OperationResult<TEntity>.Failure("Ett fel uppstod när entiteten skulle uppdateras: " + ex.Message);
        }
    }
}
