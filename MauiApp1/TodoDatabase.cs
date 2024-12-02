﻿using SQLite;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TodoApp
{
    public class TodoDatabase
    {
        readonly SQLiteAsyncConnection _database;

        public TodoDatabase(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<TodoItem>().Wait();
        }

        public Task<List<TodoItem>> GetItemsAsync()
        {
            return _database.Table<TodoItem>().ToListAsync();
        }

        public Task<int> SaveItemAsync(TodoItem item)
        {
            if (item.Id != 0)
            {
                return _database.UpdateAsync(item);
            }
            else
            {
                return _database.InsertAsync(item);
            }
        }

        public Task<int> UpdateItemAsync(TodoItem item)
        {
            return _database.UpdateAsync(item);
        }

        public Task<int> DeleteItemAsync(TodoItem item)
        {
            return _database.DeleteAsync(item);
        }
    }
}
