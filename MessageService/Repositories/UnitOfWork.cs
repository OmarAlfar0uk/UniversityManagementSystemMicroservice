using MessageService.Contracts;
using MessageService.Data;
using MessageService.Data.Models;

namespace MessageService.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly MessageDbContext _context;

    private IGenericRepository<Conversation>? _conversations;
    private IGenericRepository<Message>? _messages;
    private IGenericRepository<AiMessage>? _aiMessages;

    public UnitOfWork(MessageDbContext context)
    {
        _context = context;
    }

    public IGenericRepository<Conversation> Conversations =>
        _conversations ??= new GenericRepository<Conversation>(_context);

    public IGenericRepository<Message> Messages =>
        _messages ??= new GenericRepository<Message>(_context);

    public IGenericRepository<AiMessage> AiMessages =>
        _aiMessages ??= new GenericRepository<AiMessage>(_context);

    public async Task<int> SaveChangesAsync() =>
        await _context.SaveChangesAsync();

    public void Dispose() => _context.Dispose();
}
