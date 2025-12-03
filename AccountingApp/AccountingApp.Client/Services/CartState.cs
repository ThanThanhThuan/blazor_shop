using AccountingApp.Shared.Models;
using Blazored.LocalStorage;

namespace AccountingApp.Client.Services
{
    public class CartState
    {
        private readonly ILocalStorageService _localStorage;
        private const string StorageKey = "cart";

        // The actual list of items
        public List<CartItem> Items { get; private set; } = new();

        // Event to notify components when state changes
        public event Action? OnChange;

        public CartState(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        // Initialize: Load from LocalStorage
        public async Task InitializeAsync()
        {
            var storedItems = await _localStorage.GetItemAsync<List<CartItem>>(StorageKey);
            if (storedItems != null)
            {
                Items = storedItems;
                NotifyStateChanged();
            }
        }

        public async Task AddToCart(Product product)
        {
            var existing = Items.FirstOrDefault(i => i.Product.Id == product.Id);
            if (existing != null)
            {
                existing.Quantity++;
            }
            else
            {
                Items.Add(new CartItem { Product = product, Quantity = 1 });
            }

            await SaveState();
            NotifyStateChanged();
        }

        public async Task RemoveFromCart(Product product)
        {
            var existing = Items.FirstOrDefault(i => i.Product.Id == product.Id);
            if (existing != null)
            {
                Items.Remove(existing);
                await SaveState();
                NotifyStateChanged();
            }
        }

        private async Task SaveState()
        {
            await _localStorage.SetItemAsync(StorageKey, Items);
        }

        public async Task ClearCart()
        {
            Items.Clear();
            await _localStorage.RemoveItemAsync("cart");
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();

        // Computed Properties for UI
        public int TotalCount => Items.Sum(i => i.Quantity);
        public decimal TotalAmount => Items.Sum(i => i.TotalPrice);
    }
}