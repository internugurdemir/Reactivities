
using System;
using Application.Activities.DTOs;
using Application.Core;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities.Queries;

public class GetActivityDetails
{
    public class Query : IRequest<Result<ActivityDto>>
    {
        public required string Id { get; set; }
    }

    public class Handler(AppDbContext context, IMapper mapper) : IRequestHandler<Query, Result<ActivityDto>>
    {
        public async Task<Result<ActivityDto>> Handle(Query request, CancellationToken cancellationToken)
        {
            /*
Eager Loading vs Lazy Loading 

1. Eager Loading
Eager Loading means that the related data is loaded from the database as part of the initial query.
use the .Include() and .ThenInclude() methods to tell EF Core exactly which related entities I want to retrieve.

how works:
When I execute  query, EF Core generates a single SQL query with JOIN statements. 
It brings back the "Main Entity" (ActivityDto) and the "Related Entities" (Attendees and Users) all at once.

Code Example: context.Activities.Include(x => x.Attendees).ThenInclude(x => x.User)...

Result: One trip to the database, a big result set containing everything.
Performance: It is much faster that  will need the related data (like showing a list of attendees on an ActivityDto detail page).

Reduced Latency: Fewer "round-trips" to the database server.

               var activityDto = await context.Activities
                                    .Include(x => x.Attendees)
                                    .ThenInclude(x => x.User)
                                    .FirstOrDefaultAsync(x => request.Id == x.Id, cancellationToken);


   Executed DbCommand (5ms) [Parameters=[@request_Id='?' (Size = 36)], CommandType='Text', CommandTimeout='30']
      SELECT "a2"."Id", "a2"."Category", "a2"."City", "a2"."Date", "a2"."Description", "a2"."IsCancelled", "a2"."Latitude", "a2"."Longitude", "a2"."Title", "a2"."Venue", "s"."ActivityId", "s"."UserId", "s"."DateJoined", "s"."IsHost", "s"."Id", "s"."AccessFailedCount", "s"."Bio", "s"."ConcurrencyStamp", "s"."DisplayName", "s"."Email", "s"."EmailConfirmed", "s"."ImageUrl", "s"."LockoutEnabled", "s"."LockoutEnd", "s"."NormalizedEmail", "s"."NormalizedUserName", "s"."PasswordHash", "s"."PhoneNumber", "s"."PhoneNumberConfirmed", "s"."SecurityStamp", "s"."TwoFactorEnabled", "s"."UserName"
      FROM (
          SELECT "a"."Id", "a"."Category", "a"."City", "a"."Date", "a"."Description", "a"."IsCancelled", "a"."Latitude", "a"."Longitude", "a"."Title", "a"."Venue"
          FROM "Activities" AS "a"
          WHERE @request_Id = "a"."Id"
          LIMIT 1
      ) AS "a2"
      LEFT JOIN (
          SELECT "a0"."ActivityId", "a0"."UserId", "a0"."DateJoined", "a0"."IsHost", "a1"."Id", "a1"."AccessFailedCount", "a1"."Bio", "a1"."ConcurrencyStamp", "a1"."DisplayName", "a1"."Email", "a1"."EmailConfirmed", "a1"."ImageUrl", "a1"."LockoutEnabled", "a1"."LockoutEnd", "a1"."NormalizedEmail", "a1"."NormalizedUserName", "a1"."PasswordHash", "a1"."PhoneNumber", "a1"."PhoneNumberConfirmed", "a1"."SecurityStamp", "a1"."TwoFactorEnabled", "a1"."UserName"
          FROM "ActivityAttendees" AS "a0"
          INNER JOIN "AspNetUsers" AS "a1" ON "a0"."UserId" = "a1"."Id"
      ) AS "s" ON "a2"."Id" = "s"."ActivityId"
      ORDER BY "a2"."Id", "s"."ActivityId", "s"."UserId"


2. Lazy Loading
opposite. 
It means that the related data is not loaded until the first time 
try to access the property in  code.

how it works:
The initial query only fetches the ActivityDto. 
later write var name = ActivityDto.Attendees[0].User.DisplayName;, 
            EF Core will "wake up" and send another query to the database to get that specific data.

usually need the Microsoft.EntityFrameworkCore.Proxies package and  navigation properties must be marked as virtual.

Result: Multiple trips to the database (The "N+1" problem).

            var activity = await context.Activities
                .ProjectTo<ActivityDto>(mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);

                
      Executed DbCommand (5ms) [Parameters=[@request_Id='?' (Size = 36)], CommandType='Text', CommandTimeout='30']
      SELECT "a6"."Id", "a6"."Title", "a6"."Date", "a6"."Description", "a6"."Category", "a6"."IsCancelled", "a6"."City", "a6"."Venue", "a6"."Latitude", "a6"."Longitude", "s"."DisplayName", "s"."Id", "s"."Bio", "s"."ImageUrl", "s"."ActivityId", "s"."UserId", "a6"."c", 
"a6"."c0"
      FROM (
          SELECT "a"."Id", "a"."Title", "a"."Date", "a"."Description", "a"."Category", "a"."IsCancelled", "a"."City", "a"."Venue", "a"."Latitude", "a"."Longitude", (
              SELECT "a3"."Id"
              FROM "ActivityAttendees" AS "a2"
              INNER JOIN "AspNetUsers" AS "a3" ON "a2"."UserId" = "a3"."Id"
              WHERE "a"."Id" = "a2"."ActivityId" AND "a2"."IsHost"
              LIMIT 1) AS "c", (
              SELECT "a5"."DisplayName"
              FROM "ActivityAttendees" AS "a4"
              INNER JOIN "AspNetUsers" AS "a5" ON "a4"."UserId" = "a5"."Id"
              WHERE "a"."Id" = "a4"."ActivityId" AND "a4"."IsHost"
              LIMIT 1) AS "c0"
          FROM "Activities" AS "a"
          WHERE "a"."Id" = @request_Id
          LIMIT 1
      ) AS "a6"
      LEFT JOIN (
          SELECT "a1"."DisplayName", "a1"."Id", "a1"."Bio", "a1"."ImageUrl", "a0"."ActivityId", "a0"."UserId"
          FROM "ActivityAttendees" AS "a0"
          INNER JOIN "AspNetUsers" AS "a1" ON "a0"."UserId" = "a1"."Id"
      ) AS "s" ON "a6"."Id" = "s"."ActivityId"
      ORDER BY "a6"."Id", "s"."ActivityId", "s"."UserId"
            */
            var activity = await context.Activities
                .ProjectTo<ActivityDto>(mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(x => x.Id == request.Id, cancellationToken);
                
            if(activity==null) return Result<ActivityDto>.Failure("Activity not found",404);

            return Result<ActivityDto>.Success(mapper.Map<ActivityDto>(activity));
        }
    }
}