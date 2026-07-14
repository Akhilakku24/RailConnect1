using RailwayReservation.DTOs;
using RailwayReservation.Interfaces;
using RailwayReservation.Models;
using RailwayReservation.Services;
using NUnit.Framework;

namespace RailwayReservation.Tests;

public class TrainServiceTests
{
    [Test]
    public async Task CalculateFareAsync_ReturnsExpectedFare()
    {
        var trainRepo = new FakeTrainRepository();
        var bookingRepo = new FakeBookingRepository();
        var service = new TrainService(trainRepo, bookingRepo);

        var fare = await service.CalculateFareAsync("TR100", 2, 1);

        Assert.That(fare, Is.EqualTo(250m));
    }

    [Test]
    public async Task GetAvailableTrainsAsync_ReturnsSeatsMinusConfirmedBookings()
    {
        var trainRepo = new FakeTrainRepository();
        var bookingRepo = new FakeBookingRepository(3);
        var service = new TrainService(trainRepo, bookingRepo);

        var result = await service.GetAvailableTrainsAsync("LHR", "KHI");
        var first = result.Single();

        Assert.That(first.AvailableSeats, Is.EqualTo(97));
    }

    private sealed class FakeTrainRepository : ITrainRepository
    {
        public Task<IEnumerable<Train>> GetAllTrainsAsync() => Task.FromResult<IEnumerable<Train>>(new[]
        {
            new Train
            {
                TrainId = 1,
                TrainNo = "TR100",
                Source = "LHR",
                Destination = "KHI",
                BaseFare = 100m,
                TotalSeats = 100,
                IsActive = true,
                DepartureTime = "10:00",
                ArrivalTime = "15:00"
            }
        });

        public Task<Train?> GetTrainByIdAsync(int id) => Task.FromResult<Train?>(new Train { TrainId = 1, TrainNo = "TR100", BaseFare = 100m, TotalSeats = 100, IsActive = true });
        public Task<Train?> GetTrainByTrainNoAsync(string trainNo) => Task.FromResult<Train?>(new Train { TrainId = 1, TrainNo = "TR100", BaseFare = 100m, TotalSeats = 100, IsActive = true });
        public Task AddTrainAsync(Train train) => Task.CompletedTask;
        public Task UpdateTrainAsync(Train train) => Task.CompletedTask;
        public Task DeleteTrainAsync(int id) => Task.CompletedTask;
    }

    private sealed class FakeBookingRepository : IBookingRepository
    {
        private readonly int _confirmedCount;

        public FakeBookingRepository(int confirmedCount = 0)
        {
            _confirmedCount = confirmedCount;
        }

        public Task<Booking> AddAsync(Booking booking) => Task.FromResult(booking);
        public Task<Booking?> GetByPnrAsync(string pnr) => Task.FromResult<Booking?>(null);
        public Task<IEnumerable<Booking>> GetBookingsByTrainAndDateAsync(int trainId, DateTime journeyDate) => Task.FromResult<IEnumerable<Booking>>(Array.Empty<Booking>());
        public Task<int> GetConfirmedBookingCountByTrainAsync(int trainId) => Task.FromResult(_confirmedCount);
        public Task<IEnumerable<Booking>> GetByUserIdAsync(string userId) => Task.FromResult<IEnumerable<Booking>>(Array.Empty<Booking>());
        public Task<IEnumerable<Booking>> GetAllAsync() => Task.FromResult<IEnumerable<Booking>>(Array.Empty<Booking>());
        public Task UpdateAsync(Booking booking) => Task.CompletedTask;
    }
}
