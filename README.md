# MT.Saga.Outbox
Saga Outbox with SNS and Kafka events and SQS commands

## Problem statement
The Order saga is executing well and in the end the OrderState (saga table) records are deleted for each saga instance that finishes. However, no MessageOutbox records are inserted when an SNS event is consumed, and a corresponding command sent out by the saga. Essentially, the outbox is only working, under this scenario, for events consumed from Kafka. It would be desirable if the inbox/outbox was used for the SNS events as well.

## Requirements
- Docker
- Query access to the SqlServer database (I used MS SMS with `localhost,1433`, `sa` and `MyS@P@ssw0rd`)

## Setup instructions
- Run `docker-compose up` from the root folder (`if you get` *Windows named pipe error: The pipe has been ended. (code: 109)* `at the end, ignore it, because all necessary containers will be up and running`)
- Restore nuget packages
- Make sure `MT.Saga.Outbox` is set as the Startup Project. Run `update-database` from the *Package Manager Console* for the MT.Saga.Outbox project (there is already a migrations folder in the solution)

## Reproducing the issue (I used Resharper to decompile MT sources)
- Put breakpoints in lines `34`, `52` and `65` of `OrderStateMachine.cs`
- Put breakpoints in lines `49` and `97` of `DbContextOutboxConsumeContext.cs` from *MassTransit.EntityFrameworkCoreIntegration*
- Use the IDE to start debugging `MT.Saga.Outbox`
- Execute `MT.Saga.Outbox.exe` from `MT.Saga.Outbox\MT.All.In.One.Service\bin\Debug\net6.0`
- The breakpoint on line `34` on the state machine should be hit. Query InboxState to confirm it has 1 record. Press Continue.
- The breakpoint on line `49` on the outbox code should be hit. Query OutboxMessage to confirm it has 0 records. Press Continue.
- Whatever breakpoint is hit next, query OutboxMessage to confirm it has 1 record.
- Continue debugging through the breakpoints and notice that there is no new record inserted into OutboxMessage, when I was expecting that the command sent on line 52 would cause a record to be inserted, just like the command sent on line 34 did.
