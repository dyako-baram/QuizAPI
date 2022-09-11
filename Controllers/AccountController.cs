using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using QuizAPI.Data;
using QuizAPI.ModelDTO;
using QuizAPI.Models;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace QuizAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : Controller
    {
        #region Fields
        private readonly ApplicationDbContext _context;
        private readonly UserManager<QuizUser> _userManager;
        private readonly SignInManager<QuizUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IMemoryCache _memoryCache;

        #endregion


        public AccountController(ApplicationDbContext context, 
            UserManager<QuizUser> userManager, 
            SignInManager<QuizUser> signInManager, 
            RoleManager<IdentityRole> roleManager,
            IMemoryCache memoryCache)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _memoryCache = memoryCache;
        }
        [HttpPost("[action]")]
        public async Task<ActionResult> Login(LoginDTO loginDTO)
        {
            var user = await _userManager.FindByEmailAsync(loginDTO.Email);
            if (user == null)
            {
                return NotFound();
            }
            await _signInManager.PasswordSignInAsync(user, loginDTO.Password, false, false);
            return Ok();
        }
        
        [HttpGet("[action]")]
        public async Task<ActionResult> LogOut()
        {
            var user = _userManager.GetUserAsync(User);
            if (user != null)
            {
                await _signInManager.SignOutAsync();
            }

            return Ok();
        }
        
        [HttpGet("[action]")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<List<QuizUserDTO>>> GetAllUsers()
        {
            if (User.Identity.IsAuthenticated)
            {
                //Impleament Caching
                List<QuizUser> users;
                users = _memoryCache.Get<List<QuizUser>>("AllUsers");
                if (users is null)
                {
                    users = new();
                    users=await _context.Users.AsNoTracking().ToListAsync();
                    _memoryCache.Set("AllUsers",users,TimeSpan.FromSeconds(30));
                }
                var allUsers=new List<QuizUserDTO>();
                foreach (var user in users)
                {
                    
                    allUsers.Add(new QuizUserDTO
                    {
                        Id = user.Id,
                        UserName = user.UserName,
                        Email = user.Email,
                        Age = user.Age,
                        Country = user.Country,
                        Roles = await _userManager.GetRolesAsync(user)
                    });
                }
                return allUsers;
            }
            else
            {
                return Unauthorized("You Need to Be Admin to Access This");
            }
            
        }
        
        [HttpGet("[action]/{id}")]
        [Authorize(Roles ="Admin")]
        public async Task<ActionResult<QuizUserDTO>> GetUserById([Required] string id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }

            var userId= await _context.Users.Where(x=>x.Id==id).AsNoTracking().FirstOrDefaultAsync();
            
            if (userId != null)
            {
                return new QuizUserDTO
                {
                    Id = userId.Id,
                    UserName = userId.UserName,
                    Email = userId.Email,
                    Age = userId.Age,
                    Country = userId.Country,
                    Roles = await _userManager.GetRolesAsync(userId)
                };
            }
            else
            {
                return NotFound("This User Doesn't Exist");
            }
        }

        [HttpPut("[action]/{userId}")]
        [Authorize(Roles ="Admin")]
        public async Task<IActionResult> UpdateUserRole([Required] string userId,[Required]string roleName)
        {
            if(!User.Identity.IsAuthenticated)
            {
                return Unauthorized("You Need to be Admin to Make This Change");
            }


            var oldUser=await _userManager.FindByIdAsync(userId);
            if (oldUser == null)
            {
                return NotFound("This User Is Doesn't Exist");
            }
            var userRoles =await _userManager.GetRolesAsync(oldUser);

            var capitalizedRoleName = new CultureInfo("en-US", false).TextInfo.ToTitleCase(roleName);
            var role=await _roleManager.FindByNameAsync(capitalizedRoleName);
            if (role==null)
            {
                return BadRequest("Such Role Doesn't Exist");
            }
            foreach (var item in userRoles)
            {
                await _userManager.RemoveFromRoleAsync(oldUser,item);
            }
            await _userManager.AddToRoleAsync(oldUser, capitalizedRoleName);
            return Ok();
            
        }

        [HttpPost("[action]")]
        [AllowAnonymous]
        public async Task<ActionResult<RegisterUserDTO>> Register(RegisterUserDTO registerUserDTO)
        {
            var user = new QuizUser { UserName = registerUserDTO.UserName, Email = registerUserDTO.Email,Age=registerUserDTO.Age,Country=registerUserDTO.Country };
            var result = await _userManager.CreateAsync(user, registerUserDTO.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "User");
                await _signInManager.SignInAsync(user, isPersistent: true);
                return Ok(registerUserDTO);
            }
            else
            {
                return BadRequest("Something Went Wrong please try again");
            }
        }
        
        [HttpDelete("[action]/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteUserById([Required]string id)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }
            try
            {
                var user = await _userManager.FindByIdAsync(id);
                if(user==null)
                {
                    return NotFound("User Was Not Found");
                }
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                return Ok();
            }
            catch (Exception e)
            {
                return StatusCode(500, e.Message);
            }
            

        }
        
        [HttpPut("[action]/{id}")]
        [Authorize]
        public async Task<ActionResult<RegisterUserDTO>> UpdateUserById([Required] string id, UpdateDTO updateDTO)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }
            var result =await _userManager.FindByIdAsync(id);
            if (result==null)
            {
                return NotFound();
            }
            result.UserName = updateDTO.UserName;
            result.Email = updateDTO.Email;
            result.Age=updateDTO.Age;
            result.Country=updateDTO.Country;
            await _userManager.UpdateAsync(result);
            return Ok();
        }
        

    }
    
}
