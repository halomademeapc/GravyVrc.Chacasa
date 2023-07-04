using System.Threading;
using System.Threading.Tasks;
using GravyVrc.Chacasa.Windows.Templates;
using Microsoft.EntityFrameworkCore;

namespace GravyVrc.Chacasa.Windows.Data;

public class SettingsContext : DbContext
{
    public virtual DbSet<Setting> Settings { get; set; }
    public virtual DbSet<DisplayPage> Pages { get; set; }

    public SettingsContext(DbContextOptions<SettingsContext> options) : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Setting>(b =>
        {
            b.HasKey(s => s.Key);
            b.Property(s => s.Value)
                .IsRequired();
        });

        modelBuilder.Entity<DisplayPage>(b =>
        {
            b.HasKey(p => p.Id);
            b.Property(p => p.Label)
                .IsRequired();
            b.Property(p => p.Template)
                .IsRequired();
        });

        base.OnModelCreating(modelBuilder);
    }
}

public class Setting
{
    public string Key { get; set; }
    public string Value { get; set; }
}

public class SettingsService
{
    public const string UrlKey = "Hass:Url";
    public const string TokenKey = "Hass:Token";

    private readonly SettingsContext _dbContext;

    public SettingsService(SettingsContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Setting> SetAsync(string key, string value, CancellationToken cancellationToken = default)
    {
        var entry = await _dbContext.Settings.FindAsync(new object[] { key }, cancellationToken: cancellationToken);
        if (entry is not null)
            entry.Value = value;
        else
            entry = _dbContext.Settings.Add(new Setting { Key = key, Value = value }).Entity;
        await _dbContext.SaveChangesAsync(cancellationToken);
        return entry;
    }

    public async Task<Setting?> GetAsync(string key, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Settings
            .FindAsync(new object[] { key }, cancellationToken: cancellationToken);
    }
}