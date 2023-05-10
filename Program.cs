using Microsoft.AspNetCore.Mvc;
using ToDoApi;


var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<ToDoDbContext>();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddCors(options =>
{
    options.AddPolicy("OpenPolicy",
                          policy =>
                          {
                              policy.WithOrigins("http://localhost:3000")
                                                  .AllowAnyHeader()
                                                  .AllowAnyMethod();
                          });
});



var app = builder.Build();

app.UseCors("OpenPolicy");
app.UseHttpsRedirection();


// app.UseCors(x => x.AllowAnyHeader()
//       .AllowAnyMethod()
//       .WithOrigins("https://localhost:3000"));

// app.UseAuthentication();

app.MapGet("/", () => "Hello World!");


app.MapGet("/items", (ToDoDbContext context) =>
{
    return context.Items.ToList();
});

app.MapPost("/items", async(ToDoDbContext context, Item item)=>{
    context.Add(item);
    await context.SaveChangesAsync();
    return item;
});

app.MapPut("/items/{id}", async(ToDoDbContext context, [FromBody]Item item, int id)=>{
    var existItem = await context.Items.FindAsync(id);
    if(existItem is null) return Results.NotFound();

    existItem.Name = item.Name;
    existItem.IsComplete = item.IsComplete;

    await context.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("/items", async (ToDoDbContext context, int id) =>
{
    context.Items.Remove(context.Items.FirstOrDefault(item => item.Id == id));
    await context.SaveChangesAsync();
});

app.Run();
