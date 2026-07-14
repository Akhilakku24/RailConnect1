using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization; 
using RailwayReservation.Interfaces;     
using RailwayReservation.Models;         

[Authorize(Roles = "Admin")]
[Route("api/[controller]")]
[ApiController]
public class AdminController : ControllerBase
{
    private readonly ITrainService _trainService;
    private readonly IBookingService _bookingService;

    public AdminController(ITrainService trainService, IBookingService bookingService)
    {
        _trainService = trainService;
        _bookingService = bookingService;
    }

    [HttpPost("add-train")]
    public async Task<IActionResult> CreateTrain([FromBody] Train train)
    {
        var created = await _trainService.AddTrainAsync(train);
        return CreatedAtAction(nameof(CreateTrain), new { id = created.TrainId }, created);
    }

    [HttpDelete("remove-train/{id}")]
    public async Task<IActionResult> RemoveTrain(int id)
    {
        await _trainService.DeleteTrainAsync(id);
        return Ok("Train Deactivated");
    }

    [HttpGet("booking-history")]
    public async Task<IActionResult> GetBookingHistory()
    {
        var all = await _bookingService.GetAllBookingsAsync();
        return Ok(all);
    }

}