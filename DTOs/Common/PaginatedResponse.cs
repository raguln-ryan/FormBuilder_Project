using System.Collections.Generic;

namespace FormBuilder.API.DTOs.Common
{
    public class PaginatedResponse<T>
    {
        public List<T> Data { get; set; }
        public int TotalCount { get; set; }

        public PaginatedResponse()
        {
            Data = new List<T>();
            TotalCount = 0;
        }

        public PaginatedResponse(List<T> data, int totalCount)
        {
            Data = data ?? new List<T>();
            TotalCount = totalCount;
        }
    }
}