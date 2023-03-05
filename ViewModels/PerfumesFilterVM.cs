using Perfumeshop.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Perfumeshop.ViewModels
{
    public class PerfumesFilterVM
    {
        public IList<Perfume> perfumes { get; set; }
        public SelectList Categories { get; set; }
        public String perfumeCategory { get; set; }
        public SelectList Brands { get; set; }
        public String perfumeBrand { get; set; }
        public String SearchString { get; set; }

    }
}
