using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using WhatToCook.DAL.Data;

namespace WhatToCook.DAL.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly AppDbContext _context;
    private readonly DbSet<T> _dbSet;

    public Repository(AppDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task<T?> GetByIdAsync(int id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(T entity)
    {
        var entityType = _context.Model.FindEntityType(typeof(T));
        var primaryKey = entityType?.FindPrimaryKey()?.Properties.FirstOrDefault();

        if (primaryKey == null)
        {
            throw new InvalidOperationException($"Entity {typeof(T).Name} does not have a primary key.");
        }

        var keyValue = primaryKey.PropertyInfo?.GetValue(entity);

        if (keyValue == null)
        {
            throw new InvalidOperationException($"Entity {typeof(T).Name} has null primary key value.");
        }

        var existingEntity = await _dbSet.FindAsync(keyValue);

        if (existingEntity == null)
        {
            throw new InvalidOperationException($"{typeof(T).Name} with key {keyValue} was not found.");
        }

        _context.Entry(existingEntity).CurrentValues.SetValues(entity);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await _dbSet.FindAsync(id);

        if (entity == null)
        {
            return;
        }

        _dbSet.Remove(entity);
        await _context.SaveChangesAsync();
    }
}