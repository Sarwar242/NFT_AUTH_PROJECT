using ModelClasses;

namespace ServerSideApplication.Service
{
    public interface IGroceryService
    {
        Task<string> PostGroceryListData(GroceryList _groceryList);
        Task<List<GroceryList>> GetGroceryData();
    }
}
