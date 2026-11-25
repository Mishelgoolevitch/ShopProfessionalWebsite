using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ShopProfessionalWebsite.Data;
using ShopProfessionalWebsite.Models;
using ShopProfessionalWebsite.Services;

namespace ShopProfessionalWebsite.Controllers
{

    public class WishlistController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;  // ADD THIS
        private readonly IShoppingCartService _cartService;
        private readonly IFileService _fileService;

        public WishlistController(
           UserManager<ApplicationUser> userManager,
           SignInManager<ApplicationUser> signInManager,
           RoleManager<IdentityRole> roleManager,
           ApplicationDbContext context,  // ADD THIS PARAMETER)
           IShoppingCartService cartService, IFileService fileService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _context = context;  // ADD THIS
            _cartService = cartService;
            _fileService = fileService;
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddToWishlist(int productId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
                return Unauthorized();

            var checkwishlist = _context.WishlistItems.Where(w => w.ProductId == productId && w.UserId == user.Id).FirstOrDefault();
            if (checkwishlist == null)
            {
                if (!user.WishlistItems.Any(w => w.ProductId == productId))
                {
                    user.WishlistItems.Add(new WishlistItem
                    {
                        ProductId = productId,
                        UserId = user.Id
                    });
                    await _userManager.UpdateAsync(user);
                }
                return Json(new { success = true, message = "Added to wishlist!" });
            }
            return Json(new { success = true, message = "Already Added!" });
        }
    }
}
