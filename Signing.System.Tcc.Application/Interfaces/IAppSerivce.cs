﻿using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Signing.System.Tcc.Application.Interfaces
{
    public interface IAppSerivce<T> where T : class
    {
        T Get(int id);
        IEnumerable<T> GetAll();
        IEnumerable<T> Find(Expression<Func<T, bool>> predicate);
        T FirstOrDefault(Expression<Func<T, bool>> predicate);
        void Add(T entity);
        void AddRange(IEnumerable<T> entities);
        void Remove(T entity);
        void RemoveRange(IEnumerable<T> entities);
        void Dispose();
    }
}