namespace StrokeForEgypt.Service
{
    public class Paging
    {
        private const int _maxSize = 1000;
        private int _pageSize = 10;

        /// <summary>
        /// PageNumber = 1
        /// </summary>
        public int PageNumber { get; set; } = 1;

        /// <summary>
        /// PageSize = 20, maxSize = 1000
        /// </summary>
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > _maxSize) ? _maxSize : value;
        }

        public string OrderBy { get; set; } = "Order";
    }
}
