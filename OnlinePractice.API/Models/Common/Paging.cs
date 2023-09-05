namespace OnlinePractice.API.Models.Common
{
    public static class Paging
    {
        public static IQueryable<TSource> Page<TSource>(this IQueryable<TSource> source, int pageNo, int pageSize)
        {
            return source.Skip((pageNo - 1) * pageSize).Take(pageSize);
        }

        //used by LINQ
        public static IEnumerable<TSource> Page<TSource>(this IEnumerable<TSource> source, int pageNo, int pageSize)
        {
            return source.Skip((pageNo - 1) * pageSize).Take(pageSize);
        }
    }
}
