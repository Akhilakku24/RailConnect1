using RailConnect.Configurations;
using RailwayReservation.DTOs;
using RailwayReservation.Interfaces;
using RailwayReservation.Models;
using RailwayReservation.Repositories;

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

        private decimal CalculateTicketFare(Train train,
        int adults,int children,string classType,string quota)
        {
            decimal baseFare = train.BaseFare;
            if (classType.Equals("Business",StringComparison.OrdinalIgnoreCase))
            {
                baseFare += train.BaseFare * train.BusinessPercentage;
            }
            if (quota.Equals("Tatkal", StringComparison.OrdinalIgnoreCase))
            {
                baseFare += train.BaseFare * QuotaConfig.Tatkal;
            }
            decimal adultFare = adults * baseFare;
            decimal childFare = children * (baseFare * 0.5m);
            return adultFare + childFare;
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
                    var confirmedBookings = await _bookingRepo.GetBookedSeatCountByTrainAsync(t.TrainId);
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
        public async Task<decimal> CalculateFareAsync(string trainNo,int adultCount,
        int childCount,string classType,string quota)
        {
            var train = await _trainRepo.GetTrainByTrainNoAsync(trainNo);
            if (train == null){
                throw new Exception("Train not found.");
            }
            return CalculateTicketFare(train,adultCount,childCount,classType,quota);
        }

        public async Task<Train> AddTrainAsync(Train train)
        {
            if (string.IsNullOrWhiteSpace(train.TrainNo))
            throw new Exception("Train number is required.");
            
            if (string.IsNullOrWhiteSpace(train.Source))
            throw new Exception("Source is required.");

            if (string.IsNullOrWhiteSpace(train.Destination))
            throw new Exception("Destination is required.");
            
            train.IsActive = true;
            await _trainRepo.AddTrainAsync(train);
            return train;
        }

        public async Task<bool> DeleteTrainAsync(int trainId)
        {
            await _trainRepo.DeleteTrainAsync(trainId);
            return true;
        }
        public async Task<Train?> UpdateTrainAsync(string trainNo, UpdateTrainDTO dto){
            var train = await _trainRepo.GetTrainByTrainNoAsync(trainNo);
            if (train == null){
                return null;
            }
            if(dto.Source != null)
            {
                train.Source = dto.Source;
            }
            if(dto.Destination != null)
            {
                train.Destination = dto.Destination;
            }
            if(dto.DepartureTime != null){
                train.DepartureTime = dto.DepartureTime;
            }
            if(dto.ArrivalTime != null)
            {
                train.ArrivalTime = dto.ArrivalTime;
            }
            if(dto.BaseFare.HasValue)
            {
                train.BaseFare = dto.BaseFare.Value;
            }
            if(dto.TotalSeats.HasValue){
                train.TotalSeats = dto.TotalSeats.Value;
            }
            if(dto.NumCoaches.HasValue)
            {
                train.NumCoaches = dto.NumCoaches.Value;
            }
            if(dto.BusinessPercentage.HasValue)
            {
                train.BusinessPercentage = dto.BusinessPercentage.Value;
            }
            if(dto.IsActive.HasValue){
                train.IsActive = dto.IsActive.Value;
            }
            await _trainRepo.UpdateTrainAsync(train);

            return train;
        }

        public async Task<IEnumerable<TrainResponseDTO>> GetAllTrainsAsync()
        {
            var trains = await _trainRepo.GetAllTrainsAsync();
            return await Task.WhenAll(trains.Select(async t =>
            {
                var confirmedBookings = await _bookingRepo.GetBookedSeatCountByTrainAsync(t.TrainId);
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