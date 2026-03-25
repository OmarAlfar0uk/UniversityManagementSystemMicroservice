using System.Security.Claims;
using MediatR;
using MessageService.Data.Enums;
using MessageService.Features.AI.AskRashed;
using MessageService.Features.AI.ClearAiHistory;
using MessageService.Features.AI.GetAiHistory;
using Microsoft.AspNetCore.Mvc;

namespace MessageService.Features.AI;

public static class AiEndpoints
{
    public static void MapAiEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/ai")
                       .RequireAuthorization()
                       .WithTags("Rashed AI Assistant");

        // POST /api/v1/ai/chat — Send a message to Rashed
        group.MapPost("/chat", async (
            [FromBody] AskRashedBody body,
            ISender sender,
            ClaimsPrincipal user) =>
        {
            var studentId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var cmd = new AskRashedCommand(studentId, body.Message, body.FileUrl, body.FileType);
            var result = await sender.Send(cmd);
            return Results.Ok(result);
        }).WithSummary("Send a message to Rashed (Learnify AI Academic Assistant)");

        // GET /api/v1/ai/history — Get paginated AI conversation history
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

        // DELETE /api/v1/ai/history — Clear all AI history for the student
        group.MapDelete("/history", async (
            ISender sender,
            ClaimsPrincipal user) =>
        {
            var studentId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            await sender.Send(new ClearAiHistoryCommand(studentId));
            return Results.NoContent();
        }).WithSummary("Clear all AI conversation history for the current student");
    }
}

public record AskRashedBody(
    string Message,
    string? FileUrl = null,
    FileType FileType = FileType.None
);
