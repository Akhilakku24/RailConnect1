using RailwayReservation.Models;

namespace RailwayReservation.Interfaces;

public interface IBookingRepository
{
    Task<Booking> AddAsync(Booking booking);
    Task<Booking?> GetByPnrAsync(string pnr);
    Task<IEnumerable<Booking>> GetBookingsByTrainAndDateAsync(int trainId, DateTime journeyDate);
    Task<int> GetBookedSeatCountByTrainAsync(int trainId);
    Task<IEnumerable<Booking>> GetByUserIdAsync(string userId);
    Task<IEnumerable<Booking>> GetAllAsync();
    Task UpdateAsync(Booking booking); // Used for cancellations
}