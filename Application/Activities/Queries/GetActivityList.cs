using System;
using Application.Activities.DTOs;
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
    public class Query : IRequest<List<ActivityDto>> { }
/*
    This is the Interface Adapter or Use Case logic.

    Dependency Injection: Notice AppDbContext context in the constructor. 
                            The database is "injected" here so 
                                the handler doesn't have to create it manually. 

    It implements IRequestHandler, which is the contract saying: 
                        "I know how to handle the Query and return the List<Activity>."
*/
    public class Handler(AppDbContext context, IMapper mapper, IUserAccessor userAccessor) : IRequestHandler<Query, List<ActivityDto>>
    {

        /*
        This is where the actual work happens. 
                It talks to the database (context) and fetches the data asynchronously.

        Isolation: This handler doesn't know if the request came from 
                        a Web API, a Mobile App, or a Test. It just knows it needs to return activities. 
        */
        public async Task<List<ActivityDto>> Handle(Query request, CancellationToken cancellationToken)
        {
                
            return await context.Activities
                                .ProjectTo<ActivityDto>(mapper.ConfigurationProvider
                                    , new { currentUserId = userAccessor.GetUserId() })
                                .ToListAsync(CancellationToken.None);
        }
    }
}