using System;
using Microsoft.AspNetCore.Builder;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();


var blogs = new List<Blog>
{
    new Blog{Title="my first post", Body = "first Post"},
    new Blog{Title="my Second post", Body = "second Post"}
};

// middleware tell how long does it take to make a request
app.Use(async (context, next) => {
    var startTime = DateTime.UtcNow;//1
    await next.Invoke();
    var duration = DateTime.UtcNow - startTime;
    System.Console.WriteLine($"Duration: {duration}");//4
});

// custom middleware, context is a object contain info about req and res, next is pointer to next middleware
app.Use(async (context, next) => {
    Console.WriteLine(context.Request.Path);//2
    // code before
    await next.Invoke();// invoke next middle ware
    // code after
    Console.WriteLine(context.Response.StatusCode);//3
});

// use run across all routes, useWhen runs conditionally
app.UseWhen(
    context => context.Request.Method != "GET",
    appBuilder => appBuilder.Use(async (context, next) => 
    {
        var extractedPassword = context.Request.Headers["x-api-key"];
        if(extractedPassword == "thisIsABadPassword") 
        {
            await next.Invoke();
        }
        else
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Invalid API Key");
        }

    })
    );


app.MapGet("/", () => "Root!");

app.MapGet("/blogs", () =>
{
    return Results.Ok(blogs);
});

app.MapGet("/blogs/{index}", (int index) =>
{
    if (index < 0 || index >= blogs.Count)
    {
        return Results.NotFound();
    }
    else
    {
        return Results.Ok(blogs[index]);
    }
});

app.MapPost("/blogs", (Blog blog) =>
{
    blogs.Add(blog);
    return Results.Created($"/blogs/{blogs.Count - 1}", blog);
});

app.MapDelete("/blogs/{index}", (int index) =>
{
    if (index < 0 || index >= blogs.Count)
    {
        return Results.NotFound();
    }
    else
    {
        // var blog = blogs[index]; // grab it before delete
        blogs.RemoveAt(index);
        return Results.NoContent();

    }
});

app.MapPut("/blogs/{index}", (int index, Blog blog) =>
{
    if (index < 0 || index >= blogs.Count)
    {
        return Results.NotFound();
    }
    else
    {
        blogs[index] = blog;
        return Results.Ok(blog);
    }
});

app.Run();


public class Blog
{
    public required string Title { get; set; }
    public required string Body { get; set; }
}