using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using RailwayReservation.DTOs;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization; 
using RailwayReservation.Interfaces;
using RailwayConnect.DTOs;


[Authorize(Roles = "Passenger")]
[Route("api/[controller]")]
[ApiController]
public class BookingController : ControllerBase
{
    private readonly IBookingService _bookingService;

    public BookingController(IBookingService bookingService) => _bookingService = bookingService;

    [HttpPost("book")]
    public async Task<IActionResult> Book([FromBody] BookingRequestDTO request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var pnr = await _bookingService.BookTicketAsync(request, userId!);
            return Ok(new { message = "Booking Successful", pnr });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    [HttpGet("my-history")]
    public async Task<IActionResult> GetMyBookings()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var history = await _bookingService.GetUserBookingsAsync(userId!);
        return Ok(history);
    }

    [HttpPost("cancel/{pnr}")]
    public async Task<IActionResult> Cancel(string pnr)
    {
        var result = await _bookingService.CancelBookingAsync(pnr);
        return result ? Ok(new
        {
            Message = "Booking cancelled successfully."
        }) : NotFound(new
        {
            Message = "PNR not found."
        });
    }
        
    [HttpGet("status/{pnr}")]
    public async Task<IActionResult> CheckStatus(string pnr)
    {
        var booking = await _bookingService.GetBookingByPnrAsync(pnr);
        return booking != null ? Ok(booking) : NotFound("Invalid PNR");
    }
}