using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PostsBlogApi.Data;
using PostsBlogApi.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using PostsBlogApi.DTOs;

[Route("API/[controller]")]
[ApiController]
public class ArticlesController : ControllerBase
{
    private readonly AppDbContext _context;
    private readonly UserManager<User> _userManager;

    public ArticlesController(AppDbContext context, UserManager<User> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    [Authorize]
    [HttpGet]
    public async Task<ActionResult<IEnumerable<ArticleShort>>> GetArticles()
    {
        // find user
        var userId = User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized(new { message = "No valid user ID in token" });
        var user = await _userManager.FindByNameAsync(userId);
        if (user == null)
            return NotFound(new { message = "User not found" });

        // filter articles
        return await _context.Articles
            .Where(a => a.User == user)
            .Select(b => new ArticleShort { Id = b.Id, Name = b.Name, Description = b.Description })
            .ToListAsync();
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<ActionResult<ArticleShort>> GetArticle(int id)
    {
        // find user
        var userId = User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized(new { message = "No valid user ID in token" });
        var user = await _userManager.FindByNameAsync(userId);
        if (user == null)
            return NotFound(new { message = "User not found" });

        var article = await _context.Articles
            .Where(a => a.User == user && a.Id == id)
            .Select(b => new ArticleShort { Id = b.Id, Name = b.Name, Description = b.Description })
            .FirstOrDefaultAsync();

        if (article == null)
        {
            return NotFound();
        }

        return article;
    }

    [HttpPost]
    [Authorize]
    public async Task<ActionResult<Article>> PostArticle(CreateArticle articleIn)
    {
        // find user
        var userId = User?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return Unauthorized(new { message = "No valid user ID in token" });
        var userDb = await _userManager.FindByNameAsync(userId);
        if (userDb == null)
            return NotFound(new { message = "User not found" });

        // create article
        var article = new Article { Name = articleIn.Name, Description = articleIn.Description, User = userDb };
        article.CreatedAt = DateTime.UtcNow;
        _context.Articles.Add(article);
        await _context.SaveChangesAsync();

        // make response
        var response = new ArticleShort { Name = article.Name, Description = article.Description, Id = article.Id };
        return CreatedAtAction("GetArticle", new { id = article.Id }, response);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutArticle(int id, Article article)
    {
        if (id != article.Id)
        {
            return BadRequest();
        }

        _context.Entry(article).State = EntityState.Modified;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateConcurrencyException)
        {
            if (!ArticleExists(id))
            {
                return NotFound();
            }
            else
            {
                throw;
            }
        }

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteArticle(int id)
    {
        var article = await _context.Articles.FindAsync(id);

        if (article == null)
        {
            return NotFound();
        }

        _context.Articles.Remove(article);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private bool ArticleExists(int id)
    {
        return _context.Articles.Any(e => e.Id == id);
    }
}