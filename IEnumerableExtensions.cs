using System;
using System.Collections.Generic;
using System.IO;
using System.Data.SqlClient;
using System.Collections.Specialized;

using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace System.Collections.Generic
{
    public static class IEnumerableExtensions
    {

        public static IEnumerable<SelectListItem> ToSelectListItems<T>(
            this IEnumerable<T> items,
            Func<T, string> nameSelector,
            Func<T, string> valueSelector
            )
        {
            return items.OrderBy(item => nameSelector(item))
                   .Select(item =>
                           new SelectListItem
                           {
                               Text = nameSelector(item),
                               Value = valueSelector(item)
                           });
        }
    }
}
