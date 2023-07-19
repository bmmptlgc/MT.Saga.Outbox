# MT.Saga.Outbox
Saga Outbox with SNS and Kafka events and SQS commands

## Requirements
- Docker
- Query access to the SqlServer database (I used MS SMS with `localhost,1433`, `sa` and `MyS@P@ssw0rd`)

## Setup instructions
- Run `docker-compose up` from the root folder (`if you get` *Windows named pipe error: The pipe has been ended. (code: 109)* `at the end, ignore it, because all necessary containers will be up and running`)
- Restore nuget packages
- Make sure `MT.Saga.Outbox` is set as the Startup Project. Run `update-database` from the *Package Manager Console* for the MT.Saga.Outbox project (there is already a migrations folder in the solution)
