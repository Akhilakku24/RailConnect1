using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization; 
using RailwayReservation.Interfaces;     
using RailwayReservation.Models;
using RailwayReservation.DTOs;


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
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }
        var created = await _trainService.AddTrainAsync(train);
        return CreatedAtAction(nameof(CreateTrain), new { id = created.TrainId }, created);
    }

    [HttpDelete("remove-train/{id}")]
    public async Task<IActionResult> RemoveTrain(int id)
    {
        try
        {
            await _trainService.DeleteTrainAsync(id);
            return Ok(new
            {
                Message = "Train deactivated successfully."
            });
        }
        catch(Exception ex)
        {
            return NotFound(new
            {
                Message = ex.Message
            });
        }
    }

    [HttpPut("update-train/{trainNo}")]
    public async Task<IActionResult> UpdateTrain(string trainNo,[FromBody] UpdateTrainDTO dto)
    {
        var updatedTrain = await _trainService.UpdateTrainAsync(trainNo, dto);
        if (updatedTrain == null)
        {
        return NotFound($"Train with TrainNo {trainNo} not found.");
        }
        return Ok(updatedTrain);
    }

    [HttpGet("booking-history")]
    public async Task<IActionResult> GetBookingHistory()
    {
        var all = await _bookingService.GetAllBookingsAsync();
        return Ok(all);
    }

}