using AppointmentService.Application;
using Microsoft.EntityFrameworkCore;

namespace AppointmentService.Infrastructure;

public class AppointmentBookingService(AppointmentDbContext db) : IAppointmentBookingService
{
    public async Task<AppointmentDto> BookAsync(BookAppointmentRequest request, System.Security.Claims.ClaimsPrincipal user, CancellationToken ct)
    {
        var tenantId = user.FindFirst("tenant_id")?.Value ?? throw new UnauthorizedAccessException("Missing tenant");

        await using var trx = await db.Database.BeginTransactionAsync(ct);

        var conflict = await db.Appointments
            .AnyAsync(a => a.TenantId == tenantId
                        && a.DoctorUserId == request.DoctorUserId
                        && a.StartAtUtc == request.StartAtUtc
                        && !a.IsDeleted
                        && a.Status != AppointmentStatus.Cancelled, ct);

        if (conflict)
            throw new InvalidOperationException("Doctor already booked for selected time slot.");

        var appointment = new AppointmentEntity
        {
            TenantId = tenantId,
            PatientId = request.PatientId,
            DoctorUserId = request.DoctorUserId,
            StartAtUtc = request.StartAtUtc,
            EndAtUtc = request.EndAtUtc,
            Status = AppointmentStatus.Scheduled,
            Notes = request.Notes,
            CreatedBy = user.Identity?.Name ?? "system"
        };

        await db.Appointments.AddAsync(appointment, ct);
        await db.SaveChangesAsync(ct);
        await trx.CommitAsync(ct);

        return new AppointmentDto(appointment.Id, appointment.PatientId, appointment.DoctorUserId, appointment.StartAtUtc, appointment.EndAtUtc, appointment.Status, appointment.Notes);
    }

    public async Task<IReadOnlyCollection<AppointmentDto>> DailyScheduleAsync(DateOnly day, Guid doctorId, System.Security.Claims.ClaimsPrincipal user, CancellationToken ct)
        => await db.Appointments.Where(a => a.DoctorUserId == doctorId && a.StartAtUtc.Date == day.ToDateTime(TimeOnly.MinValue).Date && !a.IsDeleted)
            .Select(a => new AppointmentDto(a.Id, a.PatientId, a.DoctorUserId, a.StartAtUtc, a.EndAtUtc, a.Status, a.Notes)).ToListAsync(ct);

    public async Task<IReadOnlyCollection<AppointmentDto>> MonthlyCalendarAsync(int year, int month, Guid doctorId, System.Security.Claims.ClaimsPrincipal user, CancellationToken ct)
        => await db.Appointments.Where(a => a.DoctorUserId == doctorId && a.StartAtUtc.Year == year && a.StartAtUtc.Month == month && !a.IsDeleted)
            .Select(a => new AppointmentDto(a.Id, a.PatientId, a.DoctorUserId, a.StartAtUtc, a.EndAtUtc, a.Status, a.Notes)).ToListAsync(ct);
}

public class AppointmentEntity
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string TenantId { get; set; } = default!;
    public Guid PatientId { get; set; }
    public Guid DoctorUserId { get; set; }
    public DateTime StartAtUtc { get; set; }
    public DateTime EndAtUtc { get; set; }
    public AppointmentStatus Status { get; set; }
    public string? Notes { get; set; }
    public string CreatedBy { get; set; } = default!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsDeleted { get; set; }
}

public class AppointmentDbContext(DbContextOptions<AppointmentDbContext> options) : DbContext(options)
{
    public DbSet<AppointmentEntity> Appointments => Set<AppointmentEntity>();
}
