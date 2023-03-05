using Perfumeshop.Areas.Identity.Data;
using Perfumeshop.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Perfumeshop.Models
{
    public class SeedData
    {
        public static async Task CreateUserRoles(IServiceProvider serviceProvider)
        {
            var RoleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var UserManager = serviceProvider.GetRequiredService<UserManager<PerfumeshopUser>>();
            IdentityResult roleResult;
            //Add Admin Role
            var roleCheck = await RoleManager.RoleExistsAsync("Admin");
            if (!roleCheck) { roleResult = await RoleManager.CreateAsync(new IdentityRole("Admin")); }
            PerfumeshopUser user = await UserManager.FindByEmailAsync("admin@perfume.com");
            if (user == null)
            {
                var User = new PerfumeshopUser();
                User.Email = "admin@perfume.com";
                User.UserName = "admin@perfume.com";
                string userPWD = "admin123";
                IdentityResult chkUser = await UserManager.CreateAsync(User, userPWD);
                //Add default User to Role Admin
                if (chkUser.Succeeded) { var result1 = await UserManager.AddToRoleAsync(User, "Admin"); }
            }

            /*   var x = await RoleManager.RoleExistsAsync("Guest");
               if (!x)
               {
                   var role = new IdentityRole();
                   role.Name = "Guest";
                   await RoleManager.CreateAsync(role);
               }*/
            // }



            roleCheck = await RoleManager.RoleExistsAsync("User");
            if (!roleCheck) { roleResult = await RoleManager.CreateAsync(new IdentityRole("User")); }
            user = await UserManager.FindByEmailAsync("user@perfume.com");
            if (user == null)
            {
                var User = new PerfumeshopUser();
                User.Email = "user@perfume.com";
                User.UserName = "user@perfume.com";
                string userPWD = "user123";
                IdentityResult chkUser = await UserManager.CreateAsync(User, userPWD);

                if (chkUser.Succeeded)
                {
                    var result1 = await UserManager.AddToRoleAsync(User, "User");
                }
            }
        }
        

        public static void Initialize(IServiceProvider serviceProvider)
        {
            using (var context = new PerfumeshopContext(
               serviceProvider.GetRequiredService<
                   DbContextOptions<PerfumeshopContext>>()))
            {
                CreateUserRoles(serviceProvider).Wait();

                if (context.Brand.Any() || context.Perfume.Any() || context.User.Any() || context.Order.Any())
                {
                    return;
                }
                context.Brand.AddRange(
                    new Brand
                    {
                       // Id = 1,
                        Name = "Calvin Klein",
                        Description = "Calvin Klein Inc. is an internationally operating, American fashion house. The company, which became famous for its designer underwear and denim lines in the 1980s, specializes in mass-market ready-to-wear clothing for all genders and age groups as well as leather products, lifestyle accessories and shoes, home furnishings, perfume/cosmetics, eyewear, jewellery and watches in the mid-price segment. "
                    },
                    new Brand
                    {
                      //  Id = 2,
                        Name = "PRADA",
                        Description = "Prada S.p.A is an Italian luxury fashion house founded in 1913 in Milan by Mario Prada. It specializes in leather handbags, travel accessories, shoes, ready-to-wear, and other fashion accessories. Prada licenses its name and branding to Luxottica for eyewear and L’Oréal for fragrances"
                    },
                    new Brand()
                    {
                      //  Id = 3,
                        Name = "Tom Ford",
                        Description = "Tom Ford SA (stylized as TOM FORD) is a luxury fashion house founded by designer Tom Ford in 2005. Its product line features ready-to-wear and made-to-measure offerings, as well as footwear. "
                    },
                    new Brand()
                    {
                      //  Id = 4,
                        Name = "Paco Rabanne",
                        Description = "Francisco Rabaneda Cuervo (18 February 1934 – 3 February 2023), more commonly known under the pseudonym of Paco Rabanne was a Spanish fashion designer. He was an enfant terrible of the 1960s French fashion world, gaining notoriety for his space age style"
                    });
                context.SaveChanges();

                context.Perfume.AddRange(
                    new Perfume
                    {
                       // Id = 1,
                        Name = "Calvin Klein",
                        Description = "Free-spirited. Romantic. Eternal.\r\nA spirited floral fragrance\r\n",
                        Price = "$44",
                        Size = "50ml",
                        Picture = "./pictures/ckzenski.jpg",
                        Category = "Female",
                        BrandId = 1
                    },
                    new Perfume
                    {
                      //  Id = 2,
                        Name = "Paco Rabanne",
                        Description = "Top notes are coriander, Amalfi lemon, mandarin orange & pine ",
                        Price = "$76",
                        Size = "100ml",
                        Picture = "./pictures/pacozenski.jpg",
                        Category = "Female",
                        BrandId = 4
                    },
                    new Perfume
                    {
                      //  Id = 3,
                        Name = "Tom Ford",
                        Description = ": A warm and spicy eau de parfum that reinvents classic tobacco.",
                        Price = "$83",
                        Size = "50ml",
                        Picture = "./pictures/tommaski.jpg",
                        Category = "Male",
                        BrandId = 3
                    },
                    new Perfume
                    {
                     //   Id = 4,
                        Name = "PRADA",
                        Description = "A floral ambery fragrance that embraces the paradoxes of iconic ingredients to reveal new scented sensations.",
                        Price = "$98",
                        Size = "50ml",
                        Picture = "./pictures/pradazenski.jpg",
                        Category = "Female",
                        BrandId = 2
                    },
                    new Perfume
                    {
                      //  Id = 5,
                        Name = "PRADA",
                        Description = "Base notes are musk, sandalwood, Virginian cedar & tonka bean " ,
                    
                        Price = "$130",
                        Size = "70ml",
                        Picture = "./pictures/pradamaski.jpg",
                        Category = "Male",
                        BrandId = 2
                    },
                    new Perfume
                    {
                       // Id = 6,
                        Name = "Tom Ford",
                        Description = "Middle notes are lavender, jasmine, tea & cyclamen ",
                        Price = "$142",
                        Size = "125ml",
                        Picture = "./pictures/tomzenski.jpg",
                        Category = "Female",
                        BrandId = 3
                    
                    
                    });
                context.SaveChanges();
                context.User.AddRange(
                    new User
                    {
                        FirstName = "Angela",
                        LastName = "Jordeva",
                        ProfilePicture = null
                    },
                    new User
                    {
                        FirstName = "Mia",
                        LastName = "Ivankova",
                        ProfilePicture = null
                    },
                    new User
                    {
                        FirstName = "Simon",
                        LastName = "Milic",
                        ProfilePicture = null
                    });
                context.SaveChanges();
                context.Order.AddRange(
                    new Order { Status = "Pending Approval", UserId = 1, PerfumeId = 2 },
                    new Order { Status = "Pending Approval", UserId = 1, PerfumeId = 5 },
                    new Order { Status = "Approved", UserId = 1, PerfumeId = 2 },
                    new Order { Status = "Approved", UserId = 3, PerfumeId = 4 },
                    new Order { Status = "Approved", UserId = 2, PerfumeId = 3 });
                context.SaveChanges();
            }
        }
    }
}

