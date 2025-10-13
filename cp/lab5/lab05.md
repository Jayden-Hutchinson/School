## 4958 Lab 5

Implement (supervised) partition supervisor

Creates dynamic supervisors used to dynamically create `Card.Worker`s

`Counter.Application`

- Application Supervisor
- Create an ETS table
- Start the process registry
- Start the partition supervisor

`Card.WorkerSupervisor`

- Dynamic Supervisor
- start_worker(card_worker_name) -> starts card worker with start_link
- Prints a message with its pid on start

`Card.Worker`

- Registered with the Process Registry
- Retains its state on restart
- Prints a message with pid on start/restart
- deal(name, n) causes crash if n is not integer
- Card.Worker.deal("worker1", :die)

`Card.Store`

- ETS Table

`Card.Registry`

- Process Registry

**Submit File:** `card.zip`
