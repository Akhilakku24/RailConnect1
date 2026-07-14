using RailwayReservation.Models;

namespace RailwayReservation.Interfaces;

public interface ITrainRepository
{
    Task<IEnumerable<Train>> GetAllTrainsAsync();
    Task<Train?> GetTrainByIdAsync(int id);
    Task<Train?> GetTrainByTrainNoAsync(string trainNo);
    Task AddTrainAsync(Train train);
    Task UpdateTrainAsync(Train train);
    Task DeleteTrainAsync(int id); // Soft delete (IsActive = false)
}