namespace PlaylistControl.Application.Common.Models
{
    /// <summary>
    /// Represents a single page of results along with paging metadata
    /// </summary>
    /// <typeparam name="T">Type of the items in the page</typeparam>
    /// <param name="Items">Items on the current page</param>
    /// <param name="Page">Current page number (1-based)</param>
    /// <param name="PageSize">Number of items requested per page</param>
    /// <param name="TotalCount">Total number of items across all pages</param>
    public record PagedResult<T>(
        IReadOnlyList<T> Items,
        int Page,
        int PageSize,
        int TotalCount)
    {
        /// <summary>
        /// Total number of pages given <see cref="TotalCount"/> and <see cref="PageSize"/>
        /// </summary>
        public int TotalPages => PageSize <= 0
            ? 0
            : (int)Math.Ceiling(TotalCount / (double)PageSize);
    }
}