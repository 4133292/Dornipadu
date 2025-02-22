﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Mine.Services
{
    public interface IDataStore<T>
    {
        Task<bool> CreatAsync(T item);
        Task<bool> UpdateAsync(T item);
        Task<bool> DeleteAsync(string id);
        Task<T> ReadAsync(string id);
        Task<IEnumerable<T>> IndexAsync(bool forceRefresh = false);
    }
}
