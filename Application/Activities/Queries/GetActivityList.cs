using System;
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
    public class Query : IRequest<List<Activity>> { }
/*
    This is the Interface Adapter or Use Case logic.

    Dependency Injection: Notice AppDbContext context in the constructor. 
                            The database is "injected" here so 
                                the handler doesn't have to create it manually. 

    It implements IRequestHandler, which is the contract saying: 
                        "I know how to handle the Query and return the List<Activity>."
*/
    public class Handler(AppDbContext context) : IRequestHandler<Query, List<Activity>>
    {

        /*
        This is where the actual work happens. 
                It talks to the database (context) and fetches the data asynchronously.

        Isolation: This handler doesn't know if the request came from 
                        a Web API, a Mobile App, or a Test. It just knows it needs to return activities. 
        */
        public async Task<List<Activity>> Handle(Query request, CancellationToken cancellationToken)
        {

            return await context.Activities.ToListAsync(CancellationToken.None);
        }
    }
}