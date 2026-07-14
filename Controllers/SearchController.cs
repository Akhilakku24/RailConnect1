using Microsoft.AspNetCore.Mvc;
using RailwayReservation.Interfaces;

[Route("api/[controller]")]
[ApiController]
public class SearchController : ControllerBase
{
    private readonly ITrainService _trainService;

    public SearchController(ITrainService trainService)
    {
        _trainService = trainService;
    }

    [HttpGet("trains")]
    public async Task<IActionResult> GetTrains([FromQuery] string source, [FromQuery] string destination)
    {
        var trains = await _trainService.GetAvailableTrainsAsync(source, destination);
        return Ok(trains);
    }

    [HttpGet("time-table")]
    public async Task<IActionResult> GetTimeTable()
    {
        var trains = await _trainService.GetAllTrainsAsync();
        return Ok(trains);
    }


    [HttpGet("fare-estimate")]
    public async Task<IActionResult> GetFare([FromQuery] string trainNo, [FromQuery] int adults, [FromQuery] int children)
    {
        var totalFare = await _trainService.CalculateFareAsync(trainNo, adults, children);
        return Ok(new { TotalFare = totalFare });
    }
}