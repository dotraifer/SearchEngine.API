using Microsoft.AspNetCore.Mvc;
using OpenSearch.Client;

namespace SearchEngine.API.Controllers;

/// <summary>
/// API controller for handling click updates.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class ClicksController(IOpenSearchClient client, Context context) : ControllerBase
{
    /// <summary>
    /// Updates the click count for a given URL.
    /// </summary>
    /// <param name="url">The URL of the document to update.</param>
    /// <returns>An <see cref="IActionResult"/> indicating the result of the operation.</returns>
    [HttpPut]
    public Task<IActionResult> UpdateClick(Uri url)
    {
        // Update the document's click count
        var updateResponse = client.Update(new DocumentPath<dynamic>(url.ToString()), u => u
            .Script(s => s
                .Source("ctx._source.clicks += params.increment")
                .Params(p => p.Add("increment", 1))
            )
        );

        // Check for errors
        return !updateResponse.IsValid ? Task.FromResult<IActionResult>(StatusCode(500, updateResponse.DebugInformation)) :
            // Return success
            Task.FromResult<IActionResult>(Ok());
    }
}