namespace Application.Utils
{
    public class PagedResult<T>
    {
        public List<T> Data { get; }
        public int TotalCount { get; }
        public PagedResult(List<T> data, int totalCount)
        {
            Data = data;
            TotalCount = totalCount;
        }
    }
}
