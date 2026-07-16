using RailwayReservation.DTOs;
using RailwayReservation.Models;

namespace RailwayReservation.Interfaces;

public interface ITrainService
{
    Task<IEnumerable<TrainResponseDTO>> GetAvailableTrainsAsync(string source, string destination);
    Task<IEnumerable<TrainResponseDTO>> GetAllTrainsAsync();
    Task<decimal> CalculateFareAsync(string trainNo,int adultCount,int childCount,string classType,string quota);
    Task<Train> AddTrainAsync(Train train);
    Task<bool> DeleteTrainAsync(int trainId);
    Task<Train?> UpdateTrainAsync(string trainNo, UpdateTrainDTO dto);

}