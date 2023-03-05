using Perfumeshop.Models;

namespace Perfumeshop.ViewModels
{
    public class BrandsFilterVM
    {
        public IList<Brand> brands { get; set; }

        public string SearchString { get; set; }
    }
}
