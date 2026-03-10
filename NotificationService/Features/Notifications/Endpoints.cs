using System.Security.Claims;
using NotificationService.Features.Notifications.GetNotifications;
using NotificationService.Features.Notifications.MarkAllNotificationsRead;
using NotificationService.Features.Notifications.MarkNotificationRead;
using NotificationService.Features.Notifications.SendNotification;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace NotificationService.Features.Notifications;

public static class Endpoints
{
    public static void MapNotificationEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/v1/notifications")
                       .RequireAuthorization()
                       .WithTags("Notifications");

        group.MapGet("/", async (
            ISender sender,
            ClaimsPrincipal user,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 15) =>
        {
            var userId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            var result = await sender.Send(new GetNotificationsQuery(userId, pageNumber, pageSize));
            return Results.Ok(result);
        }).WithSummary("Get notifications (paginated)");

        group.MapPut("/{notificationId:guid}/read", async (
            Guid notificationId,
            ISender sender,
            ClaimsPrincipal user) =>
        {
            var userId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            await sender.Send(new MarkNotificationReadCommand(notificationId, userId));
            return Results.Ok(new { message = "Notification marked as read." });
        }).WithSummary("Mark notification as read");

        group.MapPut("/read-all", async (ISender sender, ClaimsPrincipal user) =>
        {
            var userId = Guid.Parse(user.FindFirst(ClaimTypes.NameIdentifier)!.Value);
            await sender.Send(new MarkAllNotificationsReadCommand(userId));
            return Results.Ok(new { message = "All notifications marked as read." });
        }).WithSummary("Mark all notifications as read");

        // Admin-only endpoint to push notifications
        app.MapPost("/api/v1/admin/notifications", async (
            [FromBody] SendNotificationCommand command,
            ISender sender) =>
        {
            await sender.Send(command);
            return Results.Created();
        }).RequireAuthorization().WithTags("Notifications (Admin)").WithSummary("Send notification to user (Admin)");
    }
}
