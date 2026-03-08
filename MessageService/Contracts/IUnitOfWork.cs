using MessageService.Data.Models;

namespace MessageService.Contracts;

public interface IUnitOfWork : IDisposable
{
    IGenericRepository<Conversation> Conversations { get; }
    IGenericRepository<Message> Messages { get; }
    IGenericRepository<AiMessage> AiMessages { get; }
    Task<int> SaveChangesAsync();
}
