using Microsoft.AspNetCore.Mvc;
using OpenSearch.Client;

namespace SearchEngine.API.Controllers;

/// <summary>
/// API controller for handling search requests.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class SearchController(IOpenSearchClient client, Context context) : ControllerBase
{
    /// <summary>
    /// Searches for documents based on the provided query.
    /// </summary>
    /// <param name="q">The search query.</param>
    /// <param name="pagesToReturn">The number of pages to return.</param>
    /// <returns>An <see cref="IActionResult"/> containing the search results.</returns>
    [HttpGet]
    public async Task<IActionResult> Search([FromQuery] string q, int pagesToReturn)
    {
        if (string.IsNullOrWhiteSpace(q))
        {
            return BadRequest("Search query cannot be empty.");
        }
        
        // Build the query
        var searchResponse = await client.SearchAsync<dynamic>(s => s
                .Query(query => query
                    .FunctionScore(fs => fs
                        .Query(qb => qb
                            .MultiMatch(mm => mm
                                .Query(q) // User-provided query
                                .Fields(f => f
                                    .Field("title^5")
                                    .Field("Headings^3")
                                    .Field("Paragraphs^2")
                                    .Field("content^1")
                                )
                            )
                        )
                        .Functions(funcs => funcs
                        // Recency Boost using Gauss Decay
                        .GaussDate(g => g
                            .Field("scrapedAt")
                            .Origin(DateTime.UtcNow.ToString("yyyy-MM-dd"))
                            .Scale("7d")
                        )
                        // Popularity Boost using FieldValueFactor
                        .FieldValueFactor(ff => ff
                                .Field("clicks")          // Field to use for boosting
                                .Factor(1.5)             // Multiply score by 1.5
                                .Modifier(FieldValueFactorModifier.SquareRoot) // Square root transformation
                                .Missing(0)              // Default to 0 if clicks field is missing
                        )
                        )
                        .BoostMode(FunctionBoostMode.Sum)
                    )
                )
                .Size(pagesToReturn) // How much pages to return
        );

        // Check for errors
        if (!searchResponse.IsValid)
        {
            return StatusCode(500, searchResponse.DebugInformation);
        }

        // Return search results
        return Ok(searchResponse.Documents);
    }
}