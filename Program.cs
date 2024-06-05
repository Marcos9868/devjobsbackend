using DevJobsBackend.IoC.Services;

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://*:5000");
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddControllers();
ServicesMapping.AddAuthentication(builder.Services, builder, builder.Configuration);
ServicesMapping.AddDbContext(builder.Services, builder);
ServicesMapping.AddServices(builder.Services,builder.Configuration);
ServicesMapping.AddSwagger(builder.Services);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseSwaggerUI(c => 
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "DevJobsBackend v1");
    });
}

// Remova ou comente a linha abaixo se estiver desativando HTTPS
// app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
