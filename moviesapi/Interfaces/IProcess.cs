

namespace moviesapi.Interfaces
{
    public interface IProcess<T>
    {
        Task<T> GetItemAsync(string id, string partitionKey);
        Task<T> AddItemAsync(T item);
        Task<T> UpdateItemAsync(string id,T item);
        Task DeleteItemAsync(string id, string partitionKey);
        Task<List<T>> GetAllItemsAsync(string userId);
    }
}