using Microsoft.EntityFrameworkCore;

namespace Project.Extensions
{
    public class PaginatedList<T> : List<T>
    {
        public PaginatedList(List<T> items, int pageIndex, int count, int pageSize)
        {

            TotalPages = (int) Math.Ceiling((count/(double)pageSize)) ;
            PageIndex = pageIndex;
            TotalCount = count;
            AddRange(items) ;
        }




        // page index, total count , page size, items
        public int TotalPages {  get; set; }
        public int PageIndex {  get; set; }
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public bool HasPrevPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPages;
        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pagesize, int pageindex)
        {
            var Count = source.Count();
            var items = await source.Skip((pageindex - 1) * pagesize).Take(pagesize).ToListAsync();
            return new PaginatedList<T>(items, pageindex,Count, pagesize);
        }
        
    }
}
