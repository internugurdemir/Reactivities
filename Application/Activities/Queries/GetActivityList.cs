using System;
using Application.Activities.DTOs;
using Application.Core;
using Application.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Domain;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistence;

namespace Application.Activities.Queries;

public class GetActivityList
{
    /*
    This is a "Query" in CQRS terms because 
            it is only reading data, not changing it.

    By implementing IRequest<List<Activity>>,
            it tells the Mediator: "Whenever someone sends this request, 
                                        they expect a list of Activities back."

    */

    public class Query : IRequest<Result<PagedList<ActivityDto, DateTime?>>>
    {
        public ActivityParams Params { get; set; }
    }
    /*
        This is the Interface Adapter or Use Case logic.

        Dependency Injection: Notice AppDbContext context in the constructor. 
                                The database is "injected" here so 
                                    the handler doesn't have to create it manually. 

        It implements IRequestHandler, which is the contract saying: 
                            "I know how to handle the Query and return the List<Activity>."
    */
    public class Handler(AppDbContext context,
                        IMapper mapper,
                        IUserAccessor userAccessor
                        ) : IRequestHandler<Query, Result<PagedList<ActivityDto, DateTime?>>>
    {

        /*
        This is where the actual work happens. 
                It talks to the database (context) and fetches the data asynchronously.

        Isolation: This handler doesn't know if the request came from 
                        a Web API, a Mobile App, or a Test. It just knows it needs to return activities. 
        */
        public async Task<Result<PagedList<ActivityDto, DateTime?>>> Handle(Query request, CancellationToken cancellationToken)
        {
            var query = context.Activities
                .OrderBy(x => x.Date)
                .Where(x => x.Date >= (request.Params.Cursor ?? request.Params.StartDate))
                .AsQueryable();

            if (!string.IsNullOrEmpty(request.Params.Filter))
            {
                query = request.Params.Filter switch
                {
                //  "isGoing" => query.Where(x => x.Attendees.Any(a => a.UserId == userAccessor.GetUserId())),
                    "isGoing" => query.Where(x => x.Attendees.Any(a => a.UserId == userAccessor.GetUserId() && !a.IsHost)),
                    "isHost" => query.Where(x => x.Attendees.Any(a => a.IsHost && a.UserId == userAccessor.GetUserId())),
                    _ => query
                };
            }

            var projectedActivities = query.ProjectTo<ActivityDto>(
                                                        mapper.ConfigurationProvider,
                                                        new { currentUserId = userAccessor.GetUserId() });

            var activities = await projectedActivities
                .Take(request.Params.PageSize + 1)
                .ToListAsync(cancellationToken);

            DateTime? nextCursor = null;
            if (activities.Count > request.Params.PageSize)
            {
                nextCursor = activities.Last().Date;
                activities.RemoveAt(activities.Count - 1);
            }

            return Result<PagedList<ActivityDto, DateTime?>>.Success(
                new PagedList<ActivityDto, DateTime?>
                {
                    Items = activities,
                    NextCursor = nextCursor
                });
            // return await context.Activities
            //                     .ProjectTo<ActivityDto>(mapper.ConfigurationProvider
            //                         , new { currentUserId = userAccessor.GetUserId() })
            //                     .ToListAsync(CancellationToken.None);
        }
    }
}