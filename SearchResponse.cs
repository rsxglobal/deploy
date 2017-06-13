using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Sabio.Web.Models.Responses
{
    public class SearchResponse<T> : ItemsResponse<T>
    {
        public int ResultCount { get; set; }
    }
}