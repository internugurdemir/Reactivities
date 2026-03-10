using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Infrastructure.Security;
// checks if a user is the host of a specific activity before allowing them to edit or delete it.
public class IsHostRequirement : IAuthorizationRequirement
{
}

public class IsHostRequirementHandler(AppDbContext dbContext,
                                      IHttpContextAccessor httpContextAccessor
                                    ) 
    : AuthorizationHandler<IsHostRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, IsHostRequirement requirement)
    {
        var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null) return;
        
        var httpContext = httpContextAccessor.HttpContext;

/*
The code looks at the URL to find the ID of the activity.

For example, in /api/activities/123, the activityId would be "123".
*/
        if (httpContext?.GetRouteValue("id") is not string activityId) return;

/*
It searches the ActivityAttendees table in the database.

It looks for a record that matches both the User ID and the Activity ID.

AsNoTracking() is used to make the database query faster because we aren't changing the data.
*/
        var attendee = await dbContext.ActivityAttendees
            .AsNoTracking()
            .SingleOrDefaultAsync(x => x.UserId == userId &&
                x.ActivityId == activityId);
        
        if (attendee == null) return;
        
        if (attendee.IsHost) context.Succeed(requirement);
    }
}