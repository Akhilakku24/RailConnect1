using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using RailwayReservation.DTOs;
using RailwayReservation.Interfaces;
using RailwayReservation.Models;
using RailwayReservation.Services;
using NUnit.Framework;

namespace RailwayReservation.Tests;

public class BookingServiceTests
{
    [Test]
    public void BookTicketAsync_Throws_WhenPassengerCountDoesNotMatchRequest()
    {
        var bookingRepo = new FakeBookingRepository();
        var trainRepo = new FakeTrainRepository();
        var emailService = new FakeEmailService();
        var userManager = new FakeUserManager();
        

        var service = new BookingService(bookingRepo, trainRepo, emailService, userManager);

        var request = new BookingRequestDTO
        {
            TrainNo = "TR100",
            AdultCount = 1,
            ChildCount = 0,
            Passengers = new List<PassengerRequestDTO>
            {
                new() { Name = "A", Age = 30, Gender = "Female" },
                new() { Name = "B", Age = 28, Gender = "Male" }
            }
        };

        Assert.ThrowsAsync<ArgumentException>(async () => await service.BookTicketAsync(request, "user-1"));
    }

    [Test]
    public void BookTicketAsync_Throws_WhenJourneyDateIsInThePast()
    {
        var bookingRepo = new FakeBookingRepository();
        var trainRepo = new FakeTrainRepository();
        var emailService = new FakeEmailService();
        var userManager = new FakeUserManager();
        

        var service = new BookingService(bookingRepo, trainRepo, emailService, userManager);

        var request = new BookingRequestDTO
        {
            TrainNo = "TR100",
            AdultCount = 1,
            ChildCount = 0,
            JourneyDate = DateTime.Today.AddDays(-1),
            Passengers = new List<PassengerRequestDTO>
            {
                new() { Name = "A", Age = 30, Gender = "Female" }
            }
        };

        Assert.ThrowsAsync<ArgumentException>(async () => await service.BookTicketAsync(request, "user-1"));
    }

    [Test]
    public void BookTicketAsync_Throws_WhenJourneyIsSameDayAndDepartureTimeHasPassed()
    {
        var bookingRepo = new FakeBookingRepository();
        var trainRepo = new FakeTrainRepository();
        var emailService = new FakeEmailService();
        var userManager = new FakeUserManager();
        

        var service = new BookingService(bookingRepo, trainRepo, emailService, userManager);

        var request = new BookingRequestDTO
        {
            TrainNo = "TR100",
            AdultCount = 1,
            ChildCount = 0,
            JourneyDate = DateTime.Today,
            Passengers = new List<PassengerRequestDTO>
            {
                new() { Name = "A", Age = 30, Gender = "Female" }
            }
        };

        var train = new Train
        {
            TrainId = 1,
            TrainNo = "TR100",
            BaseFare = 100m,
            TotalSeats = 100,
            IsActive = true,
            DepartureTime = DateTime.Now.AddMinutes(-30).ToString("HH:mm")
        };

        ((FakeTrainRepository)trainRepo).SetTrain(train);

        Assert.ThrowsAsync<ArgumentException>(async () => await service.BookTicketAsync(request, "user-1"));
    }

    [Test]
    public async Task BookTicketAsync_Succeeds_WhenRequestIsValidAndTrainIsStillOpen()
    {
        var bookingRepo = new FakeBookingRepository();
        var trainRepo = new FakeTrainRepository();
        var emailService = new FakeEmailService();
        var userManager = new FakeUserManager();
        

        var service = new BookingService(bookingRepo, trainRepo, emailService, userManager);

        ((FakeTrainRepository)trainRepo).SetTrain(new Train
        {
            TrainId = 1,
            TrainNo = "TR100",
            BaseFare = 100m,
            TotalSeats = 100,
            IsActive = true,
            DepartureTime = DateTime.Now.AddHours(2).ToString("HH:mm")
        });

        var request = new BookingRequestDTO
        {
            TrainNo = "TR100",
            AdultCount = 1,
            ChildCount = 0,
            JourneyDate = DateTime.Today.AddDays(1),
            Passengers = new List<PassengerRequestDTO>
            {
                new() { Name = "A", Age = 30, Gender = "Female" }
            }
        };

        var pnr = await service.BookTicketAsync(request, "user-1");

        Assert.That(pnr, Is.Not.Null.And.Not.Empty);
    }

    private sealed class FakeBookingRepository : IBookingRepository
    {
        public Task<Booking> AddAsync(Booking booking) => Task.FromResult(booking);
        public Task<Booking?> GetByPnrAsync(string pnr) => Task.FromResult<Booking?>(null);
        public Task<IEnumerable<Booking>> GetBookingsByTrainAndDateAsync(int trainId, DateTime journeyDate) => Task.FromResult<IEnumerable<Booking>>(Array.Empty<Booking>());
        public Task<int> GetConfirmedBookingCountByTrainAsync(int trainId) => Task.FromResult(0);
        public Task<IEnumerable<Booking>> GetByUserIdAsync(string userId) => Task.FromResult<IEnumerable<Booking>>(Array.Empty<Booking>());
        public Task<IEnumerable<Booking>> GetAllAsync() => Task.FromResult<IEnumerable<Booking>>(Array.Empty<Booking>());
        public Task UpdateAsync(Booking booking) => Task.CompletedTask;
    }

    private sealed class FakeTrainRepository : ITrainRepository
    {
        private Train? _train;

        public FakeTrainRepository()
        {
            _train = new Train { TrainId = 1, TrainNo = "TR100", BaseFare = 100m, TotalSeats = 100, IsActive = true, DepartureTime = "10:00" };
        }

        public void SetTrain(Train train) => _train = train;

        public Task<IEnumerable<Train>> GetAllTrainsAsync() => Task.FromResult<IEnumerable<Train>>(new[] { _train! });
        public Task<Train?> GetTrainByIdAsync(int id) => Task.FromResult<Train?>(_train);
        public Task<Train?> GetTrainByTrainNoAsync(string trainNo) => Task.FromResult<Train?>(_train);
        public Task AddTrainAsync(Train train) => Task.CompletedTask;
        public Task UpdateTrainAsync(Train train) => Task.CompletedTask;
        public Task DeleteTrainAsync(int id) => Task.CompletedTask;
    }

    private sealed class FakeEmailService : IEmailService
    {
        public Task SendEmailAsync(string to, string subject, string body) => Task.CompletedTask;
    }


    private sealed class FakeUserManager : UserManager<ApplicationUser>
    {
        public FakeUserManager() : base(new StubUserStore(), null!, null!, null!, null!, null!, null!, null!, null!) { }

        public override Task<ApplicationUser?> FindByIdAsync(string userId) => Task.FromResult<ApplicationUser?>(new ApplicationUser { Id = userId, Email = "user@example.com" });
    }

    private sealed class StubUserStore : IUserStore<ApplicationUser>
    {
        public Task<IdentityResult> CreateAsync(ApplicationUser user, CancellationToken cancellationToken) => Task.FromResult(IdentityResult.Success);
        public Task<IdentityResult> DeleteAsync(ApplicationUser user, CancellationToken cancellationToken) => Task.FromResult(IdentityResult.Success);
        public Task<ApplicationUser?> FindByIdAsync(string userId, CancellationToken cancellationToken) => Task.FromResult<ApplicationUser?>(null);
        public Task<ApplicationUser?> FindByNameAsync(string normalizedUserName, CancellationToken cancellationToken) => Task.FromResult<ApplicationUser?>(null);
        public Task<string?> GetNormalizedUserNameAsync(ApplicationUser user, CancellationToken cancellationToken) => Task.FromResult<string?>(null);
        public Task<string> GetUserIdAsync(ApplicationUser user, CancellationToken cancellationToken) => Task.FromResult(user.Id);
        public Task<string?> GetUserNameAsync(ApplicationUser user, CancellationToken cancellationToken) => Task.FromResult(user.UserName);
        public Task SetNormalizedUserNameAsync(ApplicationUser user, string? normalizedName, CancellationToken cancellationToken) => Task.FromResult(0);
        public Task SetUserNameAsync(ApplicationUser user, string? userName, CancellationToken cancellationToken) => Task.FromResult(0);
        public Task<IdentityResult> UpdateAsync(ApplicationUser user, CancellationToken cancellationToken) => Task.FromResult(IdentityResult.Success);
        public void Dispose() { }
    }
}
