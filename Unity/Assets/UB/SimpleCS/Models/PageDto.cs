using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Utilities;

namespace UB.SimpleCS.Models
{
    /// <summary>
    /// A helper class holds a list of <see cref="BaseDto"/>. You can easily implement paging for incoming responses.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class PageDto<T> : BaseDto where T : BaseDto
    {
        public PageDto()
        {
            AotHelper.EnsureList<T>();
        }

        public static string CurrentPagePropertyName = "CurrentPage";
        private int _currentPage;
        /// <summary>
        /// Holds the current page number. You can increment it by 1 and request other page from server, or decrement and request previous page.
        /// </summary>
        public int CurrentPage
        {
            get { return _currentPage; }
            set
            {
                _currentPage = value;
                OnPropertyChanged(CurrentPagePropertyName);
            }
        }

        public static string TotalPagesPropertyName = "TotalPages";
        private int _totalPages;
        /// <summary>
        /// Holds the total pages.So user can see how many pages contains related response.
        /// </summary>
        public int TotalPages
        {
            get { return _totalPages; }
            set
            {
                _totalPages = value;
                OnPropertyChanged(TotalPagesPropertyName);
            }
        }

        public static string PageSizePropertyName = "PageSize";
        private int _pageSize;
        /// <summary>
        /// Max page size of the current page
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int PageSize
        {
            get { return _pageSize; }
            set
            {
                _pageSize = value;
                OnPropertyChanged(PageSizePropertyName);
            }
        }

        public static string CollectionPropertyName = "Collection";
        private IList<T> _collection;
        /// <summary>
        /// Page data (collection of <see cref="BaseDto"/>)
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public IList<T> Collection
        {
            get { return _collection; }
            set
            {
                _collection = value;
                OnPropertyChanged(CollectionPropertyName);
            }
        }

    }
}
