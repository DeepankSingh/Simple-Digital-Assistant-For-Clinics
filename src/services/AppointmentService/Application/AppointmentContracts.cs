namespace AppointmentService.Application;

public enum AppointmentStatus { Scheduled = 1, Completed = 2, Cancelled = 3 }

public record BookAppointmentRequest(Guid PatientId, Guid DoctorUserId, DateTime StartAtUtc, DateTime EndAtUtc, string? Notes);
public record AppointmentDto(Guid Id, Guid PatientId, Guid DoctorUserId, DateTime StartAtUtc, DateTime EndAtUtc, AppointmentStatus Status, string? Notes);

public interface IAppointmentBookingService
{
    Task<AppointmentDto> BookAsync(BookAppointmentRequest request, System.Security.Claims.ClaimsPrincipal user, CancellationToken ct);
    Task<IReadOnlyCollection<AppointmentDto>> DailyScheduleAsync(DateOnly day, Guid doctorId, System.Security.Claims.ClaimsPrincipal user, CancellationToken ct);
    Task<IReadOnlyCollection<AppointmentDto>> MonthlyCalendarAsync(int year, int month, Guid doctorId, System.Security.Claims.ClaimsPrincipal user, CancellationToken ct);
}
