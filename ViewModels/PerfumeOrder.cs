using Perfumeshop.Models;
using System.ComponentModel.DataAnnotations;

namespace Perfumeshop.ViewModels
{
    public class PerfumeOrder
    {
        public User? user { get; set; }
        public int? perfumeId { get; set; }
        [Required]
        public String location { get; set; }
    }
}
