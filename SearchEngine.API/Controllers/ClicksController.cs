using Microsoft.AspNetCore.Mvc;
using OpenSearch.Client;

namespace SearchEngine.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClicksController(IOpenSearchClient client, Context context) : ControllerBase
{
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