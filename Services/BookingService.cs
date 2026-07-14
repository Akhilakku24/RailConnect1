using RailwayReservation.DTOs;
using RailwayReservation.Interfaces;
using RailwayReservation.Models;

using Microsoft.AspNetCore.Identity;




namespace RailwayReservation.Services
{
    public class BookingService : IBookingService
    {
        private readonly IBookingRepository _bookingRepo;
        private readonly ITrainRepository _trainRepo;
        private readonly IEmailService _emailService;
        private readonly UserManager<ApplicationUser> _userManager;

        public BookingService(IBookingRepository bookingRepo, ITrainRepository trainRepo, IEmailService emailService, UserManager<ApplicationUser> userManager)
        {
            _bookingRepo = bookingRepo;
            _trainRepo = trainRepo;
            _emailService = emailService;
            _userManager = userManager;
        }

        public async Task<string> BookTicketAsync(BookingRequestDTO request, string userId)
        {
            if (request.AdultCount < 0 || request.ChildCount < 0)
                throw new ArgumentException("Passenger counts cannot be negative.");

            var expectedPassengerCount = request.AdultCount + request.ChildCount;
            if (request.Passengers == null || request.Passengers.Count != expectedPassengerCount)
                throw new ArgumentException("Passenger details must match the requested adult and child counts.");

            var train = await _trainRepo.GetTrainByTrainNoAsync(request.TrainNo);
            if (train == null) throw new Exception("Train not found");

            var journeyDateTime = request.JourneyDate.Date.Add(ParseTime(train.DepartureTime));
            if (journeyDateTime <= DateTime.Now)
                throw new ArgumentException("This train has already departed or is no longer bookable.");

            // Calculate Final Fare using Service Logic
            decimal totalFare = (request.AdultCount * train.BaseFare) + (request.ChildCount * (train.BaseFare * 0.5m));

            // Generate Unique 8-character PNR
            string pnr = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper();

            // Load existing bookings for this train & journey date to avoid double-booking seats
            var existing = await _bookingRepo.GetBookingsByTrainAndDateAsync(train.TrainId, request.JourneyDate);
            var occupied = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach(var b in existing)
            {
                foreach(var p in b.Passengers)
                {
                    if (!string.IsNullOrEmpty(p.CoachNo) && !string.IsNullOrEmpty(p.SeatNo))
                        occupied.Add($"{p.CoachNo}-{p.SeatNo}");
                }
            }

            // Build coaches and seat allocation
            int numCoaches = train.NumCoaches <= 0 ? 10 : train.NumCoaches;
            int totalSeats = train.TotalSeats;
            int baseSeatsPerCoach = totalSeats / numCoaches;
            int remainder = totalSeats % numCoaches;

            // business and quota counts
            int businessSeatsTotal = (int)Math.Ceiling(totalSeats * (double)train.BusinessPercentage);
            int quotaSeatsTotal = (int)Math.Ceiling(totalSeats * (double)train.QuotaPercentage);

            var coaches = new List<(string name,int seatCount)>();
            for(int i=1;i<=numCoaches;i++)
            {
                int seats = baseSeatsPerCoach;
                if (i == numCoaches) seats += remainder;
                coaches.Add(($"PAK{i}" , seats));
            }

            // Create list of seat descriptors with class and quota flags
            var seatList = new List<(string coach,string seatNo,string cls,bool isQuota)>();
            int seatsAdded = 0;
            // Mark business seats starting from first BusinessCoachCount coaches
            int businessRemaining = Math.Max(0, Math.Min(totalSeats, train.BusinessCoachCount * baseSeatsPerCoach + Math.Min(remainder, 1)));
            businessRemaining = businessSeatsTotal; // prefer computed total

            // Fill seats sequentially coach by coach
            foreach(var c in coaches)
            {
                for(int s=1;s<=c.seatCount;s++)
                {
                    seatsAdded++;
                    bool isBusiness = seatsAdded <= businessSeatsTotal; // first N seats are business
                    bool isQuota = seatsAdded <= quotaSeatsTotal; // first M seats are quota
                    seatList.Add((c.name, s.ToString(), isBusiness ? "Business" : "Economy", isQuota));
                }
            }

            // Filter out occupied
            var available = seatList.Where(x => !occupied.Contains($"{x.coach}-{x.seatNo}")).ToList();

            // Helper to pick a seat matching preferences
            (string coach,string seatNo,string cls) PickSeat(string desiredClass, bool preferQuota, string gender)
            {
                // If gender qualifies for quota, try quota seats first
                if (preferQuota)
                {
                    var q = available.FirstOrDefault(x => x.isQuota && x.cls.Equals(desiredClass, StringComparison.OrdinalIgnoreCase));
                    if (!q.Equals(default)) return (q.coach,q.seatNo,q.cls);
                }
                // then try any in desired class
                var any = available.FirstOrDefault(x => x.cls.Equals(desiredClass, StringComparison.OrdinalIgnoreCase));
                if (!any.Equals(default)) return (any.coach,any.seatNo,any.cls);
                // fallback to any seat
                var fallback = available.FirstOrDefault();
                if (!fallback.Equals(default)) return (fallback.coach,fallback.seatNo,fallback.cls);
                throw new Exception("No seats available for selected journey");
            }

            // Create booking and assign seats
            var booking = new Booking
            {
                PNR = pnr,
                UserId = userId,
                TrainId = train.TrainId,
                JourneyDate = request.JourneyDate,
                ClassType = string.IsNullOrEmpty(request.ClassType) ? "Economy" : request.ClassType,
                Quota = string.IsNullOrEmpty(request.Quota) ? "General" : request.Quota,
                TotalFare = totalFare,
                BookingDate = DateTime.Now,
                Status = "Confirmed",
                Passengers = new List<Passenger>()
            };

            foreach(var p in request.Passengers)
            {
                bool isFemale = !string.IsNullOrEmpty(p.Gender) && p.Gender.Equals("Female", StringComparison.OrdinalIgnoreCase);
                var picked = PickSeat(booking.ClassType, isFemale, p.Gender);
                // remove from available
                available.RemoveAll(x => x.coach == picked.coach && x.seatNo == picked.seatNo);
                booking.Passengers.Add(new Passenger
                {
                    Name = p.Name,
                    Age = p.Age,
                    Gender = p.Gender,
                    CoachNo = picked.coach,
                    SeatNo = picked.seatNo
                });
            }

            
            try
            {
                await _bookingRepo.AddAsync(booking);

                var user = await _userManager.FindByIdAsync(userId);
                if (user == null || string.IsNullOrEmpty(user.Email))
                {
                    throw new Exception("User email not found");
                }

                var sb = new System.Text.StringBuilder();
                sb.Append($"<p>Your PNR is <strong>{pnr}</strong></p>");
                sb.Append($"<p>Train: {train.TrainNo} — Date: {request.JourneyDate:yyyy-MM-dd}</p>");
                sb.Append($"<p>Total Fare: {totalFare} PKR</p>");
                sb.Append("<h4>Passenger Details</h4>");
                sb.Append("<ul>");
                foreach (var pas in booking.Passengers)
                {
                    sb.Append($"<li>{pas.Name} ({pas.Age}) — Coach: {pas.CoachNo}, Seat: {pas.SeatNo}</li>");
                }
                sb.Append("</ul>");

                await _emailService.SendEmailAsync(user.Email, "Booking Confirmed", sb.ToString());
                
                return pnr;
            }
            catch
            {
                throw;
            }
        }

        private static TimeSpan ParseTime(string departureTime)
        {
            if (TimeSpan.TryParse(departureTime, out var parsed))
                return parsed;

            return TimeSpan.Zero;
        }

        public async Task<bool> CancelBookingAsync(string pnr)
        {
            var booking = await _bookingRepo.GetByPnrAsync(pnr);
            if (booking == null) return false;

            booking.Status = "Cancelled";
            await _bookingRepo.UpdateAsync(booking);
            
            return true;
        }

        public async Task<IEnumerable<BookingResponseDTO>> GetUserBookingsAsync(string userId)
        {
            var bookings = await _bookingRepo.GetByUserIdAsync(userId);
            var result = new List<BookingResponseDTO>();
            foreach(var b in bookings)
            {
                var train = await _trainRepo.GetTrainByIdAsync(b.TrainId);
                result.Add(new BookingResponseDTO
                {
                    PNR = b.PNR,
                    TrainNo = train?.TrainNo ?? string.Empty,
                    TotalFare = b.TotalFare,
                    BookingDate = b.BookingDate,
                    Status = b.Status,
                    Passengers = b.Passengers.Select(p => new PassengerResponseDTO { Name = p.Name, Age = p.Age, Gender = p.Gender, CoachNo = p.CoachNo, SeatNo = p.SeatNo }).ToList()
                });
            }
            return result;
        }

        public async Task<BookingResponseDTO?> GetBookingByPnrAsync(string pnr)
        {
            var b = await _bookingRepo.GetByPnrAsync(pnr);
            if (b == null) return null;
            var train = await _trainRepo.GetTrainByIdAsync(b.TrainId);
            return new BookingResponseDTO
            {
                PNR = b.PNR,
                TrainNo = train?.TrainNo ?? string.Empty,
                TotalFare = b.TotalFare,
                BookingDate = b.BookingDate,
                Status = b.Status,
                Passengers = b.Passengers.Select(p => new PassengerResponseDTO { Name = p.Name, Age = p.Age, Gender = p.Gender, CoachNo = p.CoachNo, SeatNo = p.SeatNo }).ToList()
            };
        }

        public async Task<IEnumerable<BookingResponseDTO>> GetAllBookingsAsync()
        {
            var bookings = await _bookingRepo.GetAllAsync();
            var result = new List<BookingResponseDTO>();
            foreach(var b in bookings)
            {
                var train = await _trainRepo.GetTrainByIdAsync(b.TrainId);
                result.Add(new BookingResponseDTO
                {
                    PNR = b.PNR,
                    TrainNo = train?.TrainNo ?? string.Empty,
                    TotalFare = b.TotalFare,
                    BookingDate = b.BookingDate,
                    Status = b.Status,
                    Passengers = b.Passengers.Select(p => new PassengerResponseDTO { Name = p.Name, Age = p.Age, Gender = p.Gender, CoachNo = p.CoachNo, SeatNo = p.SeatNo }).ToList()
                });
            }
            return result;
        }
    }
}