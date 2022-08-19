# Description

This repo contains .NET console programs for testing RabbitMQ performance.

There are two RabbitMQ clients - a publisher and a subscriber that both connect to the same Rebus over a RabbitMQ message bus.

Each program can be started in multiple instances.

The exchanged messages and the corresponding transmission delays are logged to the console.

### Usage

The test is running in docker containers using docker-compose.

Build the docker images for the publisher and the subscriber by running **build-docker-images.bat**

Then run the test using this command

    docker-compose up

When the test has completed abort docker-compse by pressing CTRL-C.<br />
Then run this command to cleanup:

    docker-compose down


### Comparison with Rebus

In this repo you will find a similar test using Rebus on top of RabbitMQ.<br />
https://github.com/boskjoett/rebus-performance-test