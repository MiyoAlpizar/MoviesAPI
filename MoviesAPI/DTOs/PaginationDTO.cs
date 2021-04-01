using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MoviesAPI.DTOs
{
    public class PaginationDTO
    {
        public int Page { get; set; } = 1;
        public int QuantityPerPage { get; set; } = 10;

        private readonly int MaxPageCantityPerPage = 50;

        public int QuantityRegistersPerPage
        {
            get => QuantityPerPage;
            set
            {
                QuantityPerPage = (value > MaxPageCantityPerPage ? QuantityPerPage : value);
            }
        }
    }
}
