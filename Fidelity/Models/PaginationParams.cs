using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Fidelity.Models
{
    public class PaginationParams
    {
        /// <summary>
        /// Page
        /// </summary>
        public int Page { get; set; }

        /// <summary>
        /// Page size
        /// </summary>
        public int PageSize { get; set; }
    }
}