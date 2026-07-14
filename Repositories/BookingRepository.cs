using Microsoft.EntityFrameworkCore;
using RailwayReservation.Data;
using RailwayReservation.Interfaces;
using RailwayReservation.Models;

namespace RailwayReservation.Repositories
{
    public class BookingRepository : IBookingRepository
    {
        private readonly AppDbContext _context;

        public BookingRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Booking> AddAsync(Booking booking)
        {
            await _context.Bookings.AddAsync(booking);
            await _context.SaveChangesAsync();
            return booking;
        }

        public async Task<Booking?> GetByPnrAsync(string pnr)
        {
            // We Include Passengers because a Booking is useless without knowing who is traveling
            return await _context.Bookings
                .Include(b => b.Passengers)
                .FirstOrDefaultAsync(b => b.PNR == pnr);
        }

        public async Task<IEnumerable<Booking>> GetBookingsByTrainAndDateAsync(int trainId, DateTime journeyDate)
        {
            return await _context.Bookings
                .Include(b => b.Passengers)
                .Where(b => b.TrainId == trainId && b.JourneyDate.Date == journeyDate.Date && b.Status != "Cancelled")
                .ToListAsync();
        }

        public async Task<int> GetConfirmedBookingCountByTrainAsync(int trainId)
        {
            return await _context.Bookings
                .CountAsync(b => b.TrainId == trainId && b.Status != "Cancelled");
        }

        public async Task<IEnumerable<Booking>> GetByUserIdAsync(string userId)
        {
            return await _context.Bookings
                .Include(b => b.Passengers)
                .Where(b => b.UserId == userId)
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Booking>> GetAllAsync()
        {
            return await _context.Bookings
                .Include(b => b.Passengers)
                .ToListAsync();
        }

        public async Task UpdateAsync(Booking booking)
        {
            _context.Bookings.Update(booking);
            await _context.SaveChangesAsync();
        }
        
    }
}