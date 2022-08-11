using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SharpStore.Entities;
using SharpStore.Models;
using SharpStore.Repositories;
using SharpStore.Services;

namespace SharpStore.Controllers;

[ApiController]
public class UserController
{
    private readonly ILogger<UserController> _logger;
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserRepository _userRepository;
    private readonly IJwtAuthService _authService;
    private readonly RoleManager<User> _roleManager;

    public UserController(ILogger<UserController>  logger, UserManager<User>  userManager,
        SignInManager<User> signInManager, IHttpContextAccessor httpContextAccessor,
        IUserRepository userRepository, IJwtAuthService authService, RoleManager<User> roleManager)
    {
        _logger = logger;
        _userManager = userManager;
        _signInManager = signInManager;
        _httpContextAccessor = httpContextAccessor;
        _userRepository = userRepository;
        _authService = authService;
        _roleManager = roleManager;
    }
    
    public async Task<string> GetAuthToken(UserModels.GetUserToken model)
    {
        var user = await _userRepository.GetUserById(model.Username); 
        var result = await _signInManager.PasswordSignInAsync(user, model.Password,  true,  false);
        var roles = await _userManager.GetRolesAsync(user);
         if (result.Succeeded)
         {
             var token = await _authService.GetToken(user.Email, user.Id, (List<string>)roles);
             return token;
         }

         return string.Empty;
    }
}