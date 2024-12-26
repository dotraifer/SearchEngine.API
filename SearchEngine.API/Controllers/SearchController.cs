using Microsoft.AspNetCore.Mvc;
using OpenSearch.Client;

namespace SearchEngine.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SearchController(IOpenSearchClient client, Context context) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Search([FromQuery] string q)
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
                                    .Field("title^3")
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
                .Size(context.Configuration.PagesToReturn) // How much pages to return
        );




        // Check for errors
        if (!searchResponse.IsValid)
        {
            return StatusCode(500, searchResponse.DebugInformation);
        }

        // Return search results
        return Ok(searchResponse.Documents);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateClick(Uri url)
    {
        // Update the document's click count
        var updateResponse = await client.UpdateAsync<dynamic>(url, u => u
            .Script(s => s
                .Source("ctx._source.clicks += 1")
            )
        );

        // Check for errors
        if (!updateResponse.IsValid)
        {
            return StatusCode(500, updateResponse.DebugInformation);
        }

        // Return success
        return Ok();
    }
    
}