using Monit0.Core.Interfaces; 
// using Monit0.Core.Models.WorldCheck; 
// using Monit0.Infrastructure.Services; 
using Microsoft.AspNetCore.Mvc;
using  Monit0.Api.DTOs; 

namespace Monit0.Api.Controllers
{
[ApiController]
[Route("api/[controller]")]
    public class WorldCheckController : ControllerBase
    {
        private readonly IWorldCheckService _worldcheckservice; 

        public WorldCheckController(IWorldCheckService worldcheckservice)
        {
            this._worldcheckservice = worldcheckservice; 
        }

        [HttpGet]
        public async Task<ActionResult> Get()
        {
            try
            {
                var worldcheckData = await _worldcheckservice.GetWorldCheckMonitoringAsync();
                if(worldcheckData == null)
                {
                    return NotFound(); 
                }

                var dto = new WorldCheckDto
                {
                    GlobalStatus = worldcheckData.GlobalStatus, 
                    LastDate = worldcheckData.LastDate
                }; 
                return Ok(worldcheckData); 
            }catch(Exception ex)
            {
                return StatusCode(500, ex.Message); 
            }
        
        }

        [HttpGet("status")]
        public async Task<IActionResult> GetStatus()
        {
            try
            {
                var statusWorldCheck = await _worldcheckservice.GetWorldCheckMonitoringAsync(); 

                if(statusWorldCheck == null)
                {
                    return NotFound();
                }
                var dtoStatut = new WorldCheckDto
                {
                    GlobalStatus = statusWorldCheck.GlobalStatus
                }; 

                return Ok(dtoStatut.GlobalStatus); 
            }catch(Exception ex)
            {
                return StatusCode(500, ex.Message); 
            }
            
        }

        [HttpPost("refresh")]

        public async Task<IActionResult> RefreshAsync()
        {
            try
            {
                var refreshData =await _worldcheckservice.GetWorldCheckMonitoringAsync();

                var dtoRefresh = new WorldCheckDto
                {
                    GlobalStatus = refreshData.GlobalStatus,
                    LastDate = refreshData.LastDate
                };
                    if(refreshData == null) return NotFound(); 
                    return Ok(refreshData); 
            }

            catch (Exception ex)
            {
                return StatusCode(500, ex.Message); 
            }
        }
    }
}