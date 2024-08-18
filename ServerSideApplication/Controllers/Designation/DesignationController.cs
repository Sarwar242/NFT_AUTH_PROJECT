using Microsoft.AspNetCore.Mvc;
using ModelClasses;
using ServerSideApplication.Service;

namespace ServerSideApplication.Controllers;

[Route("api/[controller]")]
[ApiController]
public class DesignationController : ControllerBase
{

    private readonly IDesignationService _designationService;

    public DesignationController(IDesignationService designationService)
    {
        _designationService = designationService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        var designationList = await _designationService.GetAllAsync();
        return Ok(designationList);
    }

    [HttpPost]
    public async Task<IActionResult> CreateAsync(DesignationModel designationModel)
    {
        var designationList = await _designationService.CreateAsync(designationModel);
        return Ok(designationList);
    }
}
