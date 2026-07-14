using RailwayReservation.DTOs;
using RailwayReservation.Interfaces;
using RailwayReservation.Models;

namespace RailwayReservation.Services
{
    public class TrainService : ITrainService
    {
        private readonly ITrainRepository _trainRepo;
        private readonly IBookingRepository _bookingRepo;

        public TrainService(ITrainRepository trainRepo, IBookingRepository bookingRepo)
        {
            _trainRepo = trainRepo;
            _bookingRepo = bookingRepo;
        }

        public async Task<IEnumerable<TrainResponseDTO>> GetAvailableTrainsAsync(string source, string destination)
        {
            var trains = await _trainRepo.GetAllTrainsAsync();
            
            // Filter by route and map to DTO
            return await Task.WhenAll(trains
                .Where(t => t.Source.Equals(source, StringComparison.OrdinalIgnoreCase) && 
                            t.Destination.Equals(destination, StringComparison.OrdinalIgnoreCase))
                .Select(async t =>
                {
                    var confirmedBookings = await _bookingRepo.GetConfirmedBookingCountByTrainAsync(t.TrainId);
                    return new TrainResponseDTO
                    {
                        TrainNo = t.TrainNo,
                        Source = t.Source,
                        Destination = t.Destination,
                        ArrivalTime = t.ArrivalTime,
                        DepartureTime = t.DepartureTime,
                        BaseFare = t.BaseFare,
                        AvailableSeats = Math.Max(0, t.TotalSeats - confirmedBookings)
                    };
                }));
        }
        public async Task<decimal> CalculateFareAsync(string trainNo, int adultCount, int childCount)
        {
            var train = await _trainRepo.GetTrainByTrainNoAsync(trainNo);
            if (train == null) return 0;

            // SRP: Business Rule - Adults pay full, Children pay 50%
            decimal adultTotal = adultCount * train.BaseFare;
            decimal childTotal = childCount * (train.BaseFare * 0.5m);

            return adultTotal + childTotal;
        }

        public async Task<Train> AddTrainAsync(Train train)
        {
            train.IsActive = true;
            await _trainRepo.AddTrainAsync(train);
            return train;
        }

        public async Task<bool> DeleteTrainAsync(int trainId)
        {
            await _trainRepo.DeleteTrainAsync(trainId);
            return true;
        }

        public async Task<IEnumerable<TrainResponseDTO>> GetAllTrainsAsync()
        {
            var trains = await _trainRepo.GetAllTrainsAsync();
            return await Task.WhenAll(trains.Select(async t =>
            {
                var confirmedBookings = await _bookingRepo.GetConfirmedBookingCountByTrainAsync(t.TrainId);
                return new TrainResponseDTO
                {
                    TrainNo = t.TrainNo,
                    Source = t.Source,
                    Destination = t.Destination,
                    ArrivalTime = t.ArrivalTime,
                    DepartureTime = t.DepartureTime,
                    BaseFare = t.BaseFare,
                    AvailableSeats = Math.Max(0, t.TotalSeats - confirmedBookings)
                };
            }));
        }
    }
}