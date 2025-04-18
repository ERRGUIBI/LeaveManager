using LeaveManager.Application.DTOs;
using LeaveManager.Application.Interfaces;
using LeaveManager.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace LeaveManager.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LeaveRequestsController : ControllerBase
    {
        private readonly ILeaveRequestService _service;

        public LeaveRequestsController(ILeaveRequestService service)
        {
            _service = service;
        }

        [HttpGet("all")]
        public async Task<IActionResult> Get() => Ok(await _service.GetAllAsync());

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(int id) => Ok(await _service.GetByIdAsync(id));

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] LeaveRequestDto dto)
        {
            var id = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(Get), new { id }, dto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] LeaveRequestDto dto)
        {
            dto.Id = id;
            await _service.UpdateAsync(dto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAsync(id);
            return NoContent();
        }

        [HttpGet("filter")]
        public async Task<IActionResult> FilterLeaveRequests([FromQuery] LeaveRequestFilterDto filter)
        {
            try
            {
                var results = await _service.FilterLeaveRequestsAsync(filter);
                return Ok(results);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpGet("report")]
        public IActionResult GetLeaveReport([FromQuery] int year)
        {
            if (year == 0)
            {
                return BadRequest("Please provide a valid year.");
            }

            var report = _service.GetLeaveReport(year);
            return Ok(report);
        }

        [HttpPost("{id}/approve")]
        public IActionResult ApproveLeaveRequest(int id)
        {
            try
            {
                // Appeler le service pour approuver la demande
                var leaveRequest = _service.ApproveLeaveRequest(id);

                return Ok(new { message = "Leave request approved.", leaveRequest });
            }
            catch (Exception ex)
            {
                // Retourner un message d'erreur en cas d'échec
                return BadRequest(new { message = ex.Message });
            }
        }


    }
}
