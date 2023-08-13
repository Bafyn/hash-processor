# Hash Processor App [coding challenge]

The task is to implement two applications that generate and process data using RabbitMQ.

## Application 1 [API]
- REST API application.
- Endpoint **POST** `/hashes` generates 40.000 random SHA1 hashes and sends them to RabbitMQ queue for further processing.
- Endpoint **GET** `/hashes` returns the number of hashes from the database grouped by day in JSON.

## Application 2 [Processor]
- Background worker application.
- Connects to RabbitMQ queue and processes messages in parallel using 4 threads.
- Saves messages into the database table `hashes` (id, date, sha1).

## Extra features
- Split generated hashes into batches and send them into RabbitMQ in parallel.
- Retrieve hashes from the database without recalculating data on the fly.
