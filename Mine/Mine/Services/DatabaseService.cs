using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Threading.Tasks;
using SQLite;

using Mine.Models;


namespace Mine.Services
{
    public class DatabaseService : IDataStore<ItemModel>
    {
        static readonly Lazy<SQLiteAsyncConnection> LazyInitializer = new Lazy<SQLiteAsyncConnection>(() =>
        {
            return new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
        });

        static SQLiteAsyncConnection Database => LazyInitializer.Value;
        static bool initialized = false;

        public DatabaseService()
        {
            InitializeAsync().SafeFireAndForget(false);
        }

        async Task InitializeAsync()
        {
            if (!initialized)
            {
                if (!Database.TableMappings.Any(m => m.MappedType.Name == typeof(ItemModel).Name))
                {
                    await Database.CreateTablesAsync(CreateFlags.None, typeof(ItemModel)).ConfigureAwait(false);
                }
                initialized = true;
            }
        }

        /// <summary>
        /// Insertion of Item
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public async Task<bool> CreatAsync(ItemModel item)
        {
            
            if(item == null)
            {
                return false;
            }
            var result = await Database.InsertAsync(item);
            if (result == 0)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Updation of an item
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public async Task<bool> UpdateAsync(ItemModel item)
        {
            if (item == null)
            {
                return false;
            }
            //Database call to update an item
            var result = await Database.UpdateAsync(item);
            if (result == 0)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Deletion of items from database
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<bool> DeleteAsync(string id)
        {
            var data = await ReadAsync(id);
            if (data == null)
            {
                return false;
            }
            //Delete the data where id matches
            var result = await Database.DeleteAsync(data);
            if (result == 0)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Reading of items from database
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Task<ItemModel> ReadAsync(string id)
        {
            if (id == null)
            {
                return null;
            }
            //Call database to read ID
            //Using Linq syntax to find the first record that matches with ID
            var result = Database.Table<ItemModel>().FirstOrDefaultAsync(m => m.Id.Equals(id));
            return result;
        }

        /// <summary>
        /// For Index List
        /// </summary>
        /// <param name="forceRefresh"></param>
        /// <returns></returns>
        public async Task<IEnumerable<ItemModel>> IndexAsync(bool forceRefresh = false)
        {
            var result = await Database.Table<ItemModel>().ToListAsync();
            return result;
        }


    }
}