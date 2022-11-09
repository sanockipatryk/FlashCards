using Microsoft.AspNetCore.Mvc.Rendering;

namespace FlashCards.Helpers
{
    public static class IEnumerableExtension
    {
        public static IEnumerable<SelectListItem> ToSelectListItem<T>(this IEnumerable<T> items, int selectedValue, string? firstItemText)
        {
            var selectListItems = from item in items
                                  select new SelectListItem
                                  {
                                      Text = item.GetPropertyValue("Name"),
                                      Value = item.GetPropertyValue("Id"),
                                      Selected = item.GetPropertyValue("Id").Equals(selectedValue.ToString())
                                  };
            if (firstItemText != null)
            {
                selectListItems = selectListItems.Prepend(new SelectListItem
                {
                    Text = firstItemText,
                    Value = (-1).ToString(),
                    Selected = true
                });
            }

            return selectListItems;
        }
        public static string GetPropertyValue<T>(this T item, string propertyName)
        {
            return item.GetType().GetProperty(propertyName).GetValue(item, null).ToString();
        }
        
        public static IEnumerable<T> ShuffleArray<T>(this IEnumerable<T> list)
        {
            return list.OrderBy(i => Guid.NewGuid()).ToList();
        }
    }
}
