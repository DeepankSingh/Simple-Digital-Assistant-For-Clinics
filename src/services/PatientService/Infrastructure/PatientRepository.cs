using Microsoft.EntityFrameworkCore;
using PatientService.Application;
using PatientService.Domain;

namespace PatientService.Infrastructure;

public class PatientDbContext(DbContextOptions<PatientDbContext> options) : DbContext(options)
{
    public DbSet<Patient> Patients => Set<Patient>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Patient>().HasIndex(x => new { x.TenantId, x.Phone }).IsUnique();
        modelBuilder.Entity<Patient>().HasIndex(x => new { x.TenantId, x.FullName, x.Phone });
    }
}

public class PatientRepository(PatientDbContext db) : IPatientRepository
{
    public async Task AddAsync(Patient patient, CancellationToken ct) => await db.Patients.AddAsync(patient, ct);

    public Task<bool> ExistsPhoneAsync(string tenantId, string phone, Guid? excludingId, CancellationToken ct)
        => db.Patients.AnyAsync(p => p.TenantId == tenantId && p.Phone == phone && !p.IsDeleted && (!excludingId.HasValue || p.Id != excludingId), ct);

    public Task<Patient?> GetAsync(string tenantId, Guid patientId, CancellationToken ct)
        => db.Patients.SingleOrDefaultAsync(p => p.TenantId == tenantId && p.Id == patientId && !p.IsDeleted, ct);

    public async Task<PagedResult<Patient>> SearchAsync(string tenantId, string? query, int page, int pageSize, CancellationToken ct)
    {
        IQueryable<Patient> q = db.Patients.Where(p => p.TenantId == tenantId && !p.IsDeleted);
        if (!string.IsNullOrWhiteSpace(query))
            q = q.Where(p => p.FullName.Contains(query) || p.Phone.Contains(query));

        var total = await q.CountAsync(ct);
        var items = await q.OrderByDescending(p => p.CreatedAt).Skip((page - 1) * pageSize).Take(pageSize).ToListAsync(ct);
        return new PagedResult<Patient>(items, page, pageSize, total);
    }

    public Task SaveChangesAsync(CancellationToken ct) => db.SaveChangesAsync(ct);
}
