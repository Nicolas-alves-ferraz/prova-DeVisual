using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDataContext>();

var app = builder.Build();


app.MapGet("/", () => "Prova A1");

//ENDPOINTS DE CATEGORIA
//GET: http://localhost:5000/api/categoria/listar
app.MapGet("/api/categoria/listar", ([FromServices] AppDataContext ctx) =>
{
    if (ctx.Categorias.Any())
    {
        return Results.Ok(ctx.Categorias.ToList());
    }
    return Results.NotFound("Nenhuma categoria encontrada");
});

//POST: http://localhost:5000/api/categoria/cadastrar
app.MapPost("/api/categoria/cadastrar", ([FromServices] AppDataContext ctx, [FromBody] Categoria categoria) =>
{
    ctx.Categorias.Add(categoria);
    ctx.SaveChanges();
    return Results.Created("", categoria);
});

//ENDPOINTS DE TAREFA
//GET: http://localhost:5000/api/tarefas/listar
app.MapGet("/api/tarefas/listar", ([FromServices] AppDataContext ctx) =>
{
    if (ctx.Tarefas.Any())
    {
        return Results.Ok(ctx.Tarefas.Include(x => x.Categoria).ToList());
    }
    return Results.NotFound("Nenhuma tarefa encontrada");
});

//POST: http://localhost:5000/api/tarefas/cadastrar
app.MapPost("/api/tarefas/cadastrar", ([FromServices] AppDataContext ctx, [FromBody] Tarefa tarefa) =>
{
    Categoria? categoria = ctx.Categorias.Find(tarefa.CategoriaId);
    if (categoria == null)
    {
        return Results.NotFound("Categoria não encontrada");
    }
    tarefa.Categoria = categoria;
    ctx.Tarefas.Add(tarefa);
    ctx.SaveChanges();
    return Results.Created("", tarefa);
});

//PUT: http://localhost:5000/tarefas/alterar/{id}
app.MapPut("/api/tarefas/alterar/{id}", async ([FromServices] AppDataContext ctx, [FromRoute] String id) =>
{
    var tarefa = await ctx.Categorias.FindAsync(id);

    Categoria? CategoriaId = ctx.Categorias.Find(tarefa.CategoriaId);

    if (tarefa == null)
    {
        return Results.NotFound("Tarefa não iniciada.");
    }else if(tarefa != CategoriaId){
        return Results.NotFound("Tarefa em andamento");
    }else if(tarefa == CategoriaId){
        return Results.NotFound("Concluida");
    }else{

    tarefa.CategoriaId = tarefa.CategoriaId;
    ctx.SaveChanges();
    return Results.Ok("Tarefa alterada com sucesso.");
    }
});

//GET: http://localhost:5273/tarefas/naoconcluidas
app.MapGet("/api/tarefas/naoconcluidas", ([FromServices] AppDataContext ctx) =>
{
   if (ctx.Tarefas.Any())
    {
        return Results.Ok(ctx.Tarefas.Include(x => x.CategoriaId).ToList());
    }
    return Results.NotFound("Nenhuma tarefa encontrada");
});

//GET: http://localhost:5273/tarefas/concluidas
app.MapGet("/api/tarefas/concluidas", async ([FromServices] AppDataContext ctx) =>
{
    char CategoriaId = default;
    var tarefa = await ctx.Categorias.Where(p => p.CategoriaId.Contains(CategoriaId)).ToListAsync();
    return tarefa.Any() ? Results.Ok(tarefa) : Results.NotFound("Tarefa em andamento.");
});

app.Run();
