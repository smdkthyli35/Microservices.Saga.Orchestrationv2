using SagaStateMachine.Service;

var builder = Host.CreateApplicationBuilder(args);

var host = builder.Build();
host.Run();
