using System.Security.Claims;
using PatientService.Domain;

namespace PatientService.Application;

public class PatientService(IPatientRepository repo) : IPatientService
{
    public async Task<PatientDto> CreateAsync(CreatePatientRequest request, ClaimsPrincipal user, CancellationToken ct)
    {
        var tenantId = user.FindFirst("tenant_id")?.Value ?? throw new UnauthorizedAccessException("tenant_id missing");
        if (await repo.ExistsPhoneAsync(tenantId, request.Phone, null, ct))
            throw new InvalidOperationException("Phone must be unique per tenant.");

        var patient = new Patient
        {
            TenantId = tenantId,
            FullName = request.FullName,
            Phone = request.Phone,
            Age = request.Age,
            Gender = request.Gender,
            Address = request.Address,
            MedicalHistoryNotes = request.MedicalHistoryNotes,
            CreatedBy = user.Identity?.Name ?? "system"
        };

        await repo.AddAsync(patient, ct);
        await repo.SaveChangesAsync(ct);
        return new PatientDto(patient.Id, patient.FullName, patient.Phone, patient.Age, patient.Gender, patient.Address, patient.MedicalHistoryNotes, patient.CreatedAt);
    }

    public async Task<PatientDto> UpdateAsync(Guid id, UpdatePatientRequest request, ClaimsPrincipal user, CancellationToken ct)
    {
        var tenantId = user.FindFirst("tenant_id")?.Value ?? throw new UnauthorizedAccessException("tenant_id missing");
        var patient = await repo.GetAsync(tenantId, id, ct) ?? throw new KeyNotFoundException("Patient not found.");
        if (await repo.ExistsPhoneAsync(tenantId, request.Phone, id, ct))
            throw new InvalidOperationException("Phone must be unique per tenant.");

        patient.FullName = request.FullName;
        patient.Phone = request.Phone;
        patient.Age = request.Age;
        patient.Gender = request.Gender;
        patient.Address = request.Address;
        patient.MedicalHistoryNotes = request.MedicalHistoryNotes;
        patient.UpdatedAt = DateTime.UtcNow;
        patient.UpdatedBy = user.Identity?.Name;

        await repo.SaveChangesAsync(ct);
        return new PatientDto(patient.Id, patient.FullName, patient.Phone, patient.Age, patient.Gender, patient.Address, patient.MedicalHistoryNotes, patient.CreatedAt);
    }

    public async Task DeleteAsync(Guid id, ClaimsPrincipal user, CancellationToken ct)
    {
        var tenantId = user.FindFirst("tenant_id")?.Value ?? throw new UnauthorizedAccessException("tenant_id missing");
        var patient = await repo.GetAsync(tenantId, id, ct) ?? throw new KeyNotFoundException("Patient not found.");
        patient.IsDeleted = true;
        patient.UpdatedAt = DateTime.UtcNow;
        patient.UpdatedBy = user.Identity?.Name;
        await repo.SaveChangesAsync(ct);
    }

    public async Task<PagedResult<PatientDto>> SearchAsync(string? query, int page, int pageSize, ClaimsPrincipal user, CancellationToken ct)
    {
        var tenantId = user.FindFirst("tenant_id")?.Value ?? throw new UnauthorizedAccessException("tenant_id missing");
        var result = await repo.SearchAsync(tenantId, query, page, pageSize, ct);
        var items = result.Items.Select(p => new PatientDto(p.Id, p.FullName, p.Phone, p.Age, p.Gender, p.Address, p.MedicalHistoryNotes, p.CreatedAt)).ToList();
        return new PagedResult<PatientDto>(items, result.Page, result.PageSize, result.Total);
    }
}
