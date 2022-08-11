using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SharpStore.Entities;
using SharpStore.Repositories;

namespace SharpStore.Controllers;

[ApiController]
public class StoreController
{
    private readonly ILogger<StoreController> _logger;
    private readonly IStoreRepository _storeRepository;
    private readonly UserManager<User> _userManager;

    public StoreController(ILogger<StoreController>  logger, IStoreRepository storeRepository, UserManager<User> userManager)
    {
        _logger = logger;
        _storeRepository = storeRepository;
        _userManager = userManager;
    }
    
    [HttpGet("GetStores")]
    [Authorize("Admin")]
    public async Task<List<Store>> GetStores()
    {
        var result = await _storeRepository.GetStores();
        return result;
    }
    
}