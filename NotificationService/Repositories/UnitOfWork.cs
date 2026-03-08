using NotificationService.Contracts;
using NotificationService.Data;
using NotificationService.Data.Models;

namespace NotificationService.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly NotificationDbContext _context;

    private IGenericRepository<Notification>? _notifications;

    public UnitOfWork(NotificationDbContext context)
    {
        _context = context;
    }

    public IGenericRepository<Notification> Notifications =>
        _notifications ??= new GenericRepository<Notification>(_context);

    public async Task<int> SaveChangesAsync() =>
        await _context.SaveChangesAsync();

    public void Dispose() => _context.Dispose();
}
