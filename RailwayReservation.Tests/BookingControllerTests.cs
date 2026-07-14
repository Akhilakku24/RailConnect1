using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using RailwayReservation.DTOs;
using RailwayReservation.Interfaces;
using RailwayReservation.Models;

namespace RailwayReservation.Tests;

public class BookingControllerTests
{
    [Test]
    public async Task Book_ReturnsBadRequest_WhenServiceThrowsArgumentException()
    {
        var bookingService = new ThrowingBookingService();
        var controller = new BookingController(bookingService);

        var claims = new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.NameIdentifier, "user-1") }, "Test"));
        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = claims }
        };

        var result = await controller.Book(new BookingRequestDTO { TrainNo = "TR100", AdultCount = 1, ChildCount = 0, Passengers = new List<PassengerRequestDTO>() });

        Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
    }

    [Test]
    public async Task Cancel_ReturnsBadRequest_WhenBookingCannotBeCancelled()
    {
        var bookingService = new CancellingBookingService(false);
        var controller = new BookingController(bookingService);

        var result = await controller.Cancel("PNR123");

        Assert.That(result, Is.TypeOf<BadRequestObjectResult>());
    }

    private sealed class ThrowingBookingService : IBookingService
    {
        public Task<string> BookTicketAsync(BookingRequestDTO request, string userId) => throw new ArgumentException("Invalid booking request");
        public Task<IEnumerable<BookingResponseDTO>> GetUserBookingsAsync(string userId) => Task.FromResult<IEnumerable<BookingResponseDTO>>(Array.Empty<BookingResponseDTO>());
        public Task<BookingResponseDTO?> GetBookingByPnrAsync(string pnr) => Task.FromResult<BookingResponseDTO?>(null);
        public Task<bool> CancelBookingAsync(string pnr) => Task.FromResult(true);
        public Task<IEnumerable<BookingResponseDTO>> GetAllBookingsAsync() => Task.FromResult<IEnumerable<BookingResponseDTO>>(Array.Empty<BookingResponseDTO>());
    }

    private sealed class CancellingBookingService : IBookingService
    {
        private readonly bool _cancelled;

        public CancellingBookingService(bool cancelled) => _cancelled = cancelled;

        public Task<string> BookTicketAsync(BookingRequestDTO request, string userId) => Task.FromResult("PNR123");
        public Task<IEnumerable<BookingResponseDTO>> GetUserBookingsAsync(string userId) => Task.FromResult<IEnumerable<BookingResponseDTO>>(Array.Empty<BookingResponseDTO>());
        public Task<BookingResponseDTO?> GetBookingByPnrAsync(string pnr) => Task.FromResult<BookingResponseDTO?>(null);
        public Task<bool> CancelBookingAsync(string pnr) => Task.FromResult(_cancelled);
        public Task<IEnumerable<BookingResponseDTO>> GetAllBookingsAsync() => Task.FromResult<IEnumerable<BookingResponseDTO>>(Array.Empty<BookingResponseDTO>());
    }
}
