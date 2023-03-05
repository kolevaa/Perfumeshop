using Perfumeshop.Models;
using System.ComponentModel.DataAnnotations;

namespace Perfumeshop.ViewModels
{
    public class UserEditPicture
    {
        public User? user { get; set; }

        [Display(Name = "Upload picture")]
        public IFormFile? pictureFile { get; set; }

        [Display(Name = "Picture")]
        public string? pictureName { get; set; }
    }
}
