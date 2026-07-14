using Microsoft.EntityFrameworkCore;
using RailwayReservation.Data;
using RailwayReservation.Interfaces;
using RailwayReservation.Models;

namespace RailwayReservation.Repositories
{
    public class TrainRepository : ITrainRepository
    {
        private readonly AppDbContext _context;

        public TrainRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Train>> GetAllTrainsAsync()
        {
            // Only fetch trains that are currently active/scheduled
            return await _context.Trains
                .Where(t => t.IsActive)
                .ToListAsync();
        }

        public async Task<Train?> GetTrainByIdAsync(int id)
        {
            return await _context.Trains
                .FirstOrDefaultAsync(t => t.TrainId == id && t.IsActive);
        }

        public async Task<Train?> GetTrainByTrainNoAsync(string trainNo)
        {
            return await _context.Trains
                .FirstOrDefaultAsync(t => t.TrainNo == trainNo && t.IsActive);
        }

        public async Task AddTrainAsync(Train train)
        {
            await _context.Trains.AddAsync(train);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateTrainAsync(Train train)
        {
            _context.Entry(train).State = EntityState.Modified;
            await _context.SaveChangesAsync();
        }

        public async Task DeleteTrainAsync(int id)
        {
            var train = await _context.Trains.FindAsync(id);
            if (train != null)
            {
                // SRP: We don't hard delete; we deactivate so bookings remain valid in history
                train.IsActive = false; 
                await _context.SaveChangesAsync();
            }
        }
    }
}