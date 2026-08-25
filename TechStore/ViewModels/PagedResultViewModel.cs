namespace TechStore.ViewModels
{
    public class PagedResultViewModel<T>
    {
        public List<T> Items { get; set; } = new();

        public int CurrentPage { get; set; } 

        public int PageSize { get; set; }

        public int TotalCount { get; set; }

        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

        public string? Search { get; set; }

        public bool HasPreviousPage => CurrentPage > 1;

        public bool HasNextPage => CurrentPage < TotalPages;
    }
}
