using Microsoft.AspNetCore.Mvc;
using OpenSearch.Client;

namespace SearchEngine.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class SearchController(IOpenSearchClient client) : ControllerBase
{
    // GET api/search?q=stock+market+crash
    [HttpGet]
    public async Task<ActionResult<Result>> Search([FromQuery] string q)
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
                                .Field("timestamp")
                                .Origin(DateTime.UtcNow.ToString("yyyy-MM-dd"))
                                .Scale("30d")
                            )
                            // Popularity Boost using Field Value Factor
                            .FieldValueFactor(ff => ff
                                .Field("clicks")
                                .Factor(1.5)
                                .Modifier(FieldValueFactorModifier.SquareRoot)
                            )
                        )
                        .BoostMode(FunctionBoostMode.Sum)
                    )
                )
                .Size(10) // Return top 10 results
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