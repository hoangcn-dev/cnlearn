using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Models
{
    public class Paginated<TItem> where TItem : class
    {
        public int TotalItems { get; set; }
        public int TotalPages => TotalItems / PageSize + (TotalItems % PageSize > 0 ? 1 : 0);
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public IEnumerable<TItem> Items { get; set; }
    }
}
