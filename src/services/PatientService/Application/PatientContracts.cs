namespace PatientService.Application;

public record PatientDto(Guid Id, string FullName, string Phone, int Age, string Gender, string Address, string? MedicalHistoryNotes, DateTime CreatedAt);
public record CreatePatientRequest(string FullName, string Phone, int Age, string Gender, string Address, string? MedicalHistoryNotes);
public record UpdatePatientRequest(string FullName, string Phone, int Age, string Gender, string Address, string? MedicalHistoryNotes);
public record PagedResult<T>(IReadOnlyCollection<T> Items, int Page, int PageSize, int Total);

public interface IPatientRepository
{
    Task<bool> ExistsPhoneAsync(string tenantId, string phone, Guid? excludingId, CancellationToken ct);
    Task AddAsync(Domain.Patient patient, CancellationToken ct);
    Task<Domain.Patient?> GetAsync(string tenantId, Guid patientId, CancellationToken ct);
    Task<PagedResult<Domain.Patient>> SearchAsync(string tenantId, string? query, int page, int pageSize, CancellationToken ct);
    Task SaveChangesAsync(CancellationToken ct);
}
