var builder = WebApplication.CreateBuilder(args);

Starter.RegisterServices(builder);

var app = builder.Build();

Starter.Configure(app);

Starter.RegisterEndpoints(app);

app.Run();
