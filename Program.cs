using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Net.Http.Headers;

var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();


var blogs = new List<Blog>
{
    new Blog{Title="my first post", Body = "first Post"},
    new Blog{Title="my Second post", Body = "second Post"}
};


app.MapGet("/", () => "Root!");

app.MapGet("/blogs", () =>
{
    return blogs;
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