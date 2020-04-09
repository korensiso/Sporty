namespace Sporty.Common.Network.Http.QueryUrl
{
    public class PaginationQuery
    {
        const int c_maxPageSize = 50;
        private int _pageSize = 20;
        public int PageNumber { get; set; } = 1;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > c_maxPageSize) ? c_maxPageSize : value;
        }
        public bool IncludeCount { get; set; } = false;
    }
}
