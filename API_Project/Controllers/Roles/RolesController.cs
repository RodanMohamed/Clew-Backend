using Clew.BLL;
using Clew.DAL;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CompanySystem.API
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "AdminOnly")]
    public class RolesController : ControllerBase
    {
        
        private readonly RoleManager<ApplicationRole> _roleManager;
   
        public RolesController(RoleManager<ApplicationRole> roleManager)
        {
            _roleManager = roleManager;
        }
        
        [HttpPost]
        public async Task<ActionResult> Create([FromBody] RoleCreateDto roleCreateDto)
        {
            ApplicationRole applicationRole = new ApplicationRole
            {
                Name = roleCreateDto.Name,
            };
            var result = await _roleManager.CreateAsync(applicationRole);
            return Ok(result);
        }
        
    }
}
