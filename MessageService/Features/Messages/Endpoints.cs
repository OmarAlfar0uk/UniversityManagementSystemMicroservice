using System.Security.Claims;
using MessageService.Features.Messages.GetConversations;
using MessageService.Features.Messages.GetMessages;
using MessageService.Features.Messages.SendMessage;
using MessageService.Features.Conversations.SendFile;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace MessageService.Features.Messages;

public static class Endpoints
{
    public static void MapMessageEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/messages")
                       .RequireAuthorization()
                       .WithTags("Messages");

        group.MapGet("/conversations", async (
            ISender sender,
            ClaimsPrincipal user,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 20) =>
        {
            var userId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await sender.Send(new GetConversationsQuery(userId, pageNumber, pageSize));
            return Results.Ok(result);
        }).WithSummary("Get conversations (paginated)");

        group.MapGet("/conversations/{conversationId:guid}", async (
            Guid conversationId,
            ISender sender,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 30) =>
        {
            var result = await sender.Send(new GetMessagesQuery(conversationId, pageNumber, pageSize));
            return Results.Ok(result);
        }).WithSummary("Get messages in a conversation (paginated)");

        group.MapPost("/send", async (
            [FromBody] SendBody body,
            ISender sender,
            ClaimsPrincipal user) =>
        {
            var senderId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await sender.Send(new SendMessageCommand(senderId, body.ReceiverId, body.Content));
            return Results.Created($"/api/v1/messages/conversations/{result.ConversationId}", result);
        }).WithSummary("Send a message");

        // POST /api/v1/messages/conversations/{conversationId}/send-file
        group.MapPost("/conversations/{conversationId:guid}/send-file", async (
            Guid conversationId,
            IFormFile file,
            ISender sender,
            ClaimsPrincipal user,
            [FromForm] string? content = null,
            [FromForm] bool isCamera = false) =>
        {
            var senderId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await sender.Send(new SendFileCommand(conversationId, senderId, file, content, isCamera));
            return Results.Created($"/api/v1/messages/conversations/{conversationId}", result);
        })
        .WithSummary("Send a file in a conversation")
        .DisableAntiforgery();
    }
}

public record SendBody(Guid ReceiverId, string Content);

