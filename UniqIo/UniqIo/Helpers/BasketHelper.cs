using System.Text.Json;
using UniqIo.ViewModel.Baskets;

namespace UniqIo.Helpers
{
    public class BasketHelper
    {
        public static List<BasketCookieItemVM> GetBasket(HttpRequest request)
        {
            string? value = request.Cookies["basket"];
            if (string.IsNullOrEmpty(value)) return [];
            return JsonSerializer.Deserialize<List<BasketCookieItemVM>>(value) ?? [];
        }
    }
}
