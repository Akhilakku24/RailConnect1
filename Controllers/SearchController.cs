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
        if(string.IsNullOrWhiteSpace(source))
        {
            return BadRequest(new
            {
                Message = "Source is required."
            });
        }
        if(string.IsNullOrWhiteSpace(destination))
        {
            return BadRequest(new
            {
                Message = "Destination is required."
            });
        }

        var trains = await _trainService.GetAvailableTrainsAsync(source, destination);
        if (!trains.Any())
        {
        return NotFound(new
        {
            Message =
            $"No trains found from {source} to {destination}"
        });
    }

        return Ok(trains);
    }

    [HttpGet("time-table")]
    public async Task<IActionResult> GetTimeTable()
    {
        var trains = await _trainService.GetAllTrainsAsync();
        if(!trains.Any())
        {
            return NotFound(new{
        Message = "No trains available."
    });
}

        return Ok(trains);
    }


    [HttpGet("fare-estimate")]
    public async Task<IActionResult> GetFare([FromQuery] string trainNo,[FromQuery] int adults,
    [FromQuery] int children,[FromQuery] string classType,[FromQuery] string quota)
    {
        try{
        var totalFare = await _trainService.CalculateFareAsync(trainNo,adults,children,classType,quota);
        return Ok(new
        {
            TrainNo = trainNo,
            Adults = adults,
            Children = children,
            ClassType = classType,
            Quota = quota,
            TotalFare = totalFare
        });
        }
        catch(Exception ex)
        {
            return NotFound(new { message = ex.Message });
        }
}
}