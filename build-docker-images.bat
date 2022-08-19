cd Publisher
dotnet publish -c Release
docker build -t rabbitmq-perftest-publisher .
cd ..

cd Subscriber
dotnet publish -c Release
docker build -t rabbitmq-perftest-subscriber .
cd ..
