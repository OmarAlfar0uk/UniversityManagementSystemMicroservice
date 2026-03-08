using NotificationService.Data.Models;

namespace NotificationService.Contracts;

public interface IUnitOfWork : IDisposable
{
    IGenericRepository<Notification> Notifications { get; }
    Task<int> SaveChangesAsync();
}
