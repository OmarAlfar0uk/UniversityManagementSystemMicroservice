using System.Security.Claims;
using MediatR;
using MessageService.Features.AI.GetAiHistory;
using MessageService.Features.AI.SendMessage;
using MessageService.Features.AI.SendFile;
using MessageService.Features.AI.DeleteHistory;
using Microsoft.AspNetCore.Mvc;

namespace MessageService.Features.AI;

public static class AiEndpoints
{
    public static void MapAiEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/message/ai")
                       .RequireAuthorization()
                       .WithTags("Rashed AI Assistant");

        // GET /api/v1/message/ai/history
        group.MapGet("/history", async (
            ISender sender,
            ClaimsPrincipal user,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 20) =>
        {
            var studentId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await sender.Send(new GetAiHistoryQuery(studentId, page, pageSize));
            return Results.Ok(result);
        }).WithSummary("Get paginated AI conversation history");

        // POST /api/v1/message/ai/send
        group.MapPost("/send", async (
            [FromBody] SendAiMessageBody body,
            ISender sender,
            ClaimsPrincipal user) =>
        {
            var studentId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await sender.Send(new SendAiMessageCommand(studentId, body.Message));
            return Results.Ok(result);
        }).WithSummary("Send a text message to Rashed AI");

        // POST /api/v1/message/ai/send-file
        group.MapPost("/send-file", async (
            IFormFile file,
            ISender sender,
            ClaimsPrincipal user,
            [FromForm] string? message = null) =>
        {
            var studentId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await sender.Send(new SendAiFileCommand(studentId, file, message));
            return Results.Ok(result);
        })
        .WithSummary("Send a file to Rashed AI for analysis")
        .DisableAntiforgery();

        // DELETE /api/v1/message/ai/history
        group.MapDelete("/history", async (
            ISender sender,
            ClaimsPrincipal user) =>
        {
            var studentId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            await sender.Send(new DeleteAiHistoryCommand(studentId));
            return Results.NoContent();
        }).WithSummary("Delete all AI conversation history for the current student");
    }
}

public record SendAiMessageBody(string Message);

