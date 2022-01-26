using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;

using Xamarin.Forms;

using Mine.Models;
using Mine.Views;

namespace Mine.ViewModels
{
    public class ItemsIndexViewModel : BaseViewModel
    {
        public ObservableCollection<ItemModel> DataSet { get; set; }
        public Command LoadItemsCommand { get; set; }

        public ItemsIndexViewModel()
        {
            Title = "Items";
            DataSet = new ObservableCollection<ItemModel>();
            LoadItemsCommand = new Command(async () => await ExecuteLoadItemsCommand());

            MessagingCenter.Subscribe<ItemCreatePage, ItemModel>(this, "AddItem", async (obj, item) =>
            {
                var newItem = item as ItemModel;
                DataSet.Add(newItem);
                await DataStore.CreatAsync(newItem);
            });
        }



        async Task ExecuteLoadItemsCommand()
        {
            if (IsBusy)
                return;

            IsBusy = true;

            try
            {
                DataSet.Clear();
                var items = await DataStore.IndexAsync(true);
                foreach (var item in items)
                {
                    DataSet.Add(item);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
        }

        ///<summary>
        /// Read an item from the database
        ///</summary>
        ///<param name="id">ID of the record</param>
        ///<returns>The record from ReadAsync</returns>
        public async Task<ItemModel> ReadAsync(string id)
        {
            var result = await DataStore.ReadAsync(id);

            return result;

        }

        ///<summary>
        ///Delete the record from the record
        ///</summary>
        ///<param name="data">The record to Delete</param>
        ///<returns>True if Deleted</returns>
        public async Task<bool> DeleteAsync(ItemModel data)
        {
            //Check if the record exists, if it does not, null will be returned
            var record = await ReadAsync(data.Id);
            if (record != null)
            {

                //Remove from local dataset cache
                DataSet.Remove(data);

                //Call to remove it from the data store
                var result = await DataStore.DeleteAsync(data.Id);

                return result;
            }
            return false;
        }
    }
}