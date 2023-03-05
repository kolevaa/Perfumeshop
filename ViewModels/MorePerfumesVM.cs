using Perfumeshop.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace Perfumeshop.ViewModels
{
    public class MorePerfumesVM
    {
        public User? user { get; set; }
        public IEnumerable<int>? selectedPerfumes { get; set; }
        public IEnumerable<SelectListItem>? perfumeList { get; set; }
        [Required]
        public String location { get; set; }

    }
}
