using Perfumeshop.Models;
using System.ComponentModel.DataAnnotations;

namespace Perfumeshop.ViewModels
{
    public class PerfumePicture
    {
        public Perfume? perfume { get; set; }

        [Display(Name = "Upload picture")]
        public IFormFile? pictureFile { get; set; }

        [Display(Name = "Picture")]
        public string? pictureName { get; set; }
    }
}
