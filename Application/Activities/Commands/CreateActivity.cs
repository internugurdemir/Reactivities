using System;
using Domain;
using MediatR;
using Persistence;

namespace Application.Activities.Commands;

public class CreateActivity
{

    /*
        In CQRS, a Command represents an intent to change 
                    the state of the system (Create, Update, Delete).

        The Package: Think of this as an envelope. 
                    Inside the envelope is the Activity data you want to save.

        The Return Type: IRequest<string> means that once this command is finished, 
                    it will send back a string (in this case, the ID of the new activity).
    */
    public class Command : IRequest<string>
    {
        public required Activity Activity { get; set; }
    }
/*
            The Worker: This is the class that actually does the heavy lifting.

            Dependency Injection: It asks for AppDbContext. 
                        It doesn't know how the database is set up; 
                        it just uses the "injected" version to save data.
*/
    public class Handler(AppDbContext context) : IRequestHandler<Command, string>
    {
        /*
            This is pure Business Logic. 
            It takes the data from the "envelope" (request), gives it to the database, and confirms it was saved.
        */
        public async Task<string> Handle(Command request, CancellationToken cancellationToken)
        {
            context.Activities.Add(request.Activity);

            await context.SaveChangesAsync(cancellationToken);

            return request.Activity.Id;
        }
    }
}