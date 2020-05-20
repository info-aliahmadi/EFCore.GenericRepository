﻿// <copyright file="Repository.cs" company="TanvirArjel">
// Copyright (c) TanvirArjel. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;

namespace TanvirArjel.EFCore.GenericRepository.Implementations
{
    internal class Repository : IRepository
    {
        private readonly DbContext _dbContext;

        public Repository(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public IQueryable<T> GetQueryable<T>()
            where T : class
        {
            return _dbContext.Set<T>();
        }

        public async Task<List<T>> GetEntityListAsync<T>(bool asNoTracking = false)
            where T : class
        {
            IQueryable<T> query = _dbContext.Set<T>();

            if (asNoTracking)
            {
                query = query.AsNoTracking();
            }

            List<T> entities = await query.ToListAsync();

            return entities;
        }

        public async Task<List<T>> GetEntityListAsync<T>(Expression<Func<T, bool>> condition, bool asNoTracking = false)
             where T : class
        {
            IQueryable<T> query = _dbContext.Set<T>();

            if (condition != null)
            {
                query = query.Where(condition);
            }

            if (asNoTracking)
            {
                query = query.AsNoTracking();
            }

            List<T> entities = await query.ToListAsync();

            return entities;
        }

        public async Task<List<T>> GetEntityListAsync<T>(Specification<T> specification, bool asNoTracking = false)
            where T : class
        {
            IQueryable<T> query = _dbContext.Set<T>();

            if (specification != null)
            {
                query = query.GetSpecifiedQuery(specification);
            }

            if (asNoTracking)
            {
                query = query.AsNoTracking();
            }

            return await query.ToListAsync();
        }

        public async Task<List<TProjectedType>> GetProjectedEntityListAsync<T, TProjectedType>(
            Expression<Func<T, TProjectedType>> selectExpression)
            where T : class
        {
            if (selectExpression == null)
            {
                throw new ArgumentNullException(nameof(selectExpression));
            }

            List<TProjectedType> entities = await _dbContext.Set<T>().Select(selectExpression).ToListAsync();

            return entities;
        }

        public async Task<List<TProjectedType>> GetProjectedEntityListAsync<T, TProjectedType>(
            Expression<Func<T, bool>> condition,
            Expression<Func<T, TProjectedType>> selectExpression)
            where T : class
        {
            if (selectExpression == null)
            {
                throw new ArgumentNullException(nameof(selectExpression));
            }

            IQueryable<T> query = _dbContext.Set<T>();

            if (condition != null)
            {
                query = query.Where(condition);
            }

            List<TProjectedType> projectedEntites = await query.Select(selectExpression).ToListAsync();

            return projectedEntites;
        }

        public async Task<List<TProjectedType>> GetProjectedEntityListAsync<T, TProjectedType>(
            Specification<T> specification,
            Expression<Func<T, TProjectedType>> selectExpression)
            where T : class
        {
            if (selectExpression == null)
            {
                throw new ArgumentNullException(nameof(selectExpression));
            }

            IQueryable<T> query = _dbContext.Set<T>();

            if (specification != null)
            {
                query = query.GetSpecifiedQuery(specification);
            }

            return await query.Select(selectExpression).ToListAsync();
        }

        public async Task<T> GetEntityByIdAsync<T>(object id, bool asNoTracking = false)
            where T : class
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            IEntityType entityType = _dbContext.Model.FindEntityType(typeof(T));

            string primaryKeyName = entityType.FindPrimaryKey().Properties.Select(p => p.Name).FirstOrDefault();
            Type primaryKeyType = entityType.FindPrimaryKey().Properties.Select(p => p.ClrType).FirstOrDefault();

            if (primaryKeyName == null || primaryKeyType == null)
            {
                throw new ArgumentException("Entity does not have any primary key defined", nameof(id));
            }

            object primayKeyValue = null;

            try
            {
                primayKeyValue = Convert.ChangeType(id, primaryKeyType, CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {
                throw new ArgumentException($"You can not assign a value of type {id.GetType()} to a property of type {primaryKeyType}");
            }

            ParameterExpression pe = Expression.Parameter(typeof(T), "entity");
            MemberExpression me = Expression.Property(pe, primaryKeyName);
            ConstantExpression constant = Expression.Constant(primayKeyValue, primaryKeyType);
            BinaryExpression body = Expression.Equal(me, constant);
            Expression<Func<T, bool>> expressionTree = Expression.Lambda<Func<T, bool>>(body, new[] { pe });

            IQueryable<T> query = _dbContext.Set<T>();

            if (asNoTracking)
            {
                T noTrackedEntity = await query.AsNoTracking().FirstOrDefaultAsync(expressionTree);
                return noTrackedEntity;
            }

            T trackedEntity = await query.FirstOrDefaultAsync(expressionTree);
            return trackedEntity;
        }

        public async Task<TProjectedType> GetProjectedEntityByIdAsync<T, TProjectedType>(
            object id,
            Expression<Func<T, TProjectedType>> selectExpression)
            where T : class
        {
            if (id == null)
            {
                throw new ArgumentNullException(nameof(id));
            }

            if (selectExpression == null)
            {
                throw new ArgumentNullException(nameof(selectExpression));
            }

            IEntityType entityType = _dbContext.Model.FindEntityType(typeof(T));

            string primaryKeyName = entityType.FindPrimaryKey().Properties.Select(p => p.Name).FirstOrDefault();
            Type primaryKeyType = entityType.FindPrimaryKey().Properties.Select(p => p.ClrType).FirstOrDefault();

            if (primaryKeyName == null || primaryKeyType == null)
            {
                throw new ArgumentException("Entity does not have any primary key defined", nameof(id));
            }

            object primayKeyValue = null;

            try
            {
                primayKeyValue = Convert.ChangeType(id, primaryKeyType, CultureInfo.InvariantCulture);
            }
            catch (Exception)
            {
                throw new ArgumentException($"You can not assign a value of type {id.GetType()} to a property of type {primaryKeyType}");
            }

            ParameterExpression pe = Expression.Parameter(typeof(T), "entity");
            MemberExpression me = Expression.Property(pe, primaryKeyName);
            ConstantExpression constant = Expression.Constant(primayKeyValue, primaryKeyType);
            BinaryExpression body = Expression.Equal(me, constant);
            Expression<Func<T, bool>> expressionTree = Expression.Lambda<Func<T, bool>>(body, new[] { pe });

            IQueryable<T> query = _dbContext.Set<T>();

            return await query.Where(expressionTree).Select(selectExpression).FirstOrDefaultAsync();
        }

        public async Task<T> GetEntityAsync<T>(Expression<Func<T, bool>> condition, bool asNoTracking = false)
           where T : class
        {
            IQueryable<T> query = _dbContext.Set<T>();

            if (condition != null)
            {
                query = query.Where(condition);
            }

            if (asNoTracking)
            {
                query = query.AsNoTracking();
            }

            return await query.FirstOrDefaultAsync();
        }

        public async Task<T> GetEntityAsync<T>(Specification<T> specification, bool asNoTracking = false)
            where T : class
        {
            IQueryable<T> query = _dbContext.Set<T>();

            if (specification != null)
            {
                query = query.GetSpecifiedQuery(specification);
            }

            if (asNoTracking)
            {
                query = query.AsNoTracking();
            }

            return await query.FirstOrDefaultAsync();
        }

        public async Task<TProjectedType> GetProjectedEntityAsync<T, TProjectedType>(
            Expression<Func<T, bool>> condition,
            Expression<Func<T, TProjectedType>> selectExpression)
            where T : class
        {
            if (selectExpression == null)
            {
                throw new ArgumentNullException(nameof(selectExpression));
            }

            IQueryable<T> query = _dbContext.Set<T>();

            if (condition != null)
            {
                query = query.Where(condition);
            }

            return await query.Select(selectExpression).FirstOrDefaultAsync();
        }

        public async Task<TProjectedType> GetProjectedEntityAsync<T, TProjectedType>(
            Specification<T> specification,
            Expression<Func<T, TProjectedType>> selectExpression)
            where T : class
        {
            if (selectExpression == null)
            {
                throw new ArgumentNullException(nameof(selectExpression));
            }

            IQueryable<T> query = _dbContext.Set<T>();

            if (specification != null)
            {
                query = query.GetSpecifiedQuery(specification);
            }

            return await query.Select(selectExpression).FirstOrDefaultAsync();
        }

        public async Task<bool> IsEntityExistsAsync<T>(Expression<Func<T, bool>> condition)
           where T : class
        {
            if (condition == null)
            {
                return false;
            }

            bool isExists = await _dbContext.Set<T>().AnyAsync(condition);
            return isExists;
        }

        public async Task InsertEntityAsync<T>(T entity)
           where T : class
        {
            await _dbContext.Set<T>().AddAsync(entity);
        }

        public async Task InsertEntitiesAsync<T>(IEnumerable<T> entities)
            where T : class
        {
            await _dbContext.Set<T>().AddRangeAsync(entities);
        }

        public void UpdateEntity<T>(T entity)
            where T : class
        {
            EntityEntry<T> trackedEntity = _dbContext.ChangeTracker.Entries<T>().FirstOrDefault(x => x.Entity == entity);

            if (trackedEntity != null)
            {
                _dbContext.Entry(entity).CurrentValues.SetValues(entity);
            }
            else
            {
                _dbContext.Set<T>().Update(entity);
            }
        }

        public void UpdateEntities<T>(IEnumerable<T> entities)
            where T : class
        {
            _dbContext.Set<T>().UpdateRange(entities);
        }

        public void DeleteEntity<T>(T entity)
            where T : class
        {
            _dbContext.Set<T>().Remove(entity);
        }

        public void DeleteEntities<T>(IEnumerable<T> entities)
            where T : class
        {
            _dbContext.Set<T>().RemoveRange(entities);
        }

        public async Task<int> GetCountAsync<T>()
            where T : class
        {
            int count = await _dbContext.Set<T>().CountAsync();
            return count;
        }

        public async Task<int> GetCountAsync<T>(params Expression<Func<T, bool>>[] conditions)
            where T : class
        {
            IQueryable<T> query = _dbContext.Set<T>();

            if (conditions == null)
            {
                return await query.CountAsync();
            }

            foreach (Expression<Func<T, bool>> expression in conditions)
            {
                query = query.Where(expression);
            }

            return await query.CountAsync();
        }

        public async Task<long> GetLongCountAsync<T>()
            where T : class
        {
            long count = await _dbContext.Set<T>().LongCountAsync();
            return count;
        }

        public async Task<long> GetLongCountAsync<T>(params Expression<Func<T, bool>>[] conditions)
            where T : class
        {
            IQueryable<T> query = _dbContext.Set<T>();

            if (conditions == null)
            {
                return await query.LongCountAsync();
            }

            foreach (Expression<Func<T, bool>> expression in conditions)
            {
                query = query.Where(expression);
            }

            return await query.LongCountAsync();
        }

        // DbConext level members
        public int ExecuteSqlCommand(string sql, params object[] parameters)
        {
            return _dbContext.Database.ExecuteSqlRaw(sql, parameters);
        }

        public async Task<int> ExecuteSqlCommandAsync(string sql, params object[] parameters)
        {
            return await _dbContext.Database.ExecuteSqlRawAsync(sql, parameters);
        }

        public void ResetContextState()
        {
            _dbContext.ChangeTracker.Entries().Where(e => e.Entity != null).ToList()
                .ForEach(e => e.State = EntityState.Detached);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
