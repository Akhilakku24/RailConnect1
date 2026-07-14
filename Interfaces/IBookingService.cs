using RailwayReservation.Models;
using RailwayReservation.DTOs;

namespace RailwayReservation.Interfaces;

public interface IBookingService
{
    Task<string> BookTicketAsync(BookingRequestDTO request, string userId);
    Task<IEnumerable<BookingResponseDTO>> GetUserBookingsAsync(string userId);
    Task<BookingResponseDTO?> GetBookingByPnrAsync(string pnr);
    Task<bool> CancelBookingAsync(string pnr);
    Task<IEnumerable<BookingResponseDTO>> GetAllBookingsAsync();
}