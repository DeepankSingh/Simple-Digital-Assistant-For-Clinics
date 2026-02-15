namespace PrescriptionService.Application;

public record CreatePrescriptionRequest(Guid PatientId, Guid AppointmentId, string Medicines, string Dosage, string? Notes);
public record PrescriptionDto(Guid Id, Guid PatientId, Guid AppointmentId, string Medicines, string Dosage, string? Notes, DateTime CreatedDateUtc);

public interface IPrescriptionService
{
    Task<PrescriptionDto> CreateAsync(CreatePrescriptionRequest request, System.Security.Claims.ClaimsPrincipal user, CancellationToken ct);
    Task<byte[]> DownloadPdfAsync(Guid prescriptionId, System.Security.Claims.ClaimsPrincipal user, CancellationToken ct);
}
