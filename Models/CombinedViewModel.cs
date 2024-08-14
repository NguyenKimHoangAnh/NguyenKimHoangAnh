using System.Collections.Generic;

namespace NguyenKimHoangAnh.Context
{
    public class CombinedViewModel
    {
        public IEnumerable<Category> Categories { get; set; }
        public IEnumerable<Product> Products { get; set; }
    }
}
