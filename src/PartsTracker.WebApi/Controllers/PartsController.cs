using Microsoft.AspNetCore.Mvc;
using PartsTracker.WebApi.Infrastricture;
using PartsTracker.WebApi.Models;

namespace PartsTracker.WebApi.Controllers;

/// <summary>
/// Controller for managing parts in the inventory system.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class PartsController : ControllerBase
{
    private readonly ILogger<PartsController> _logger;
    private readonly IPartsRepository _repository;

    /// <summary>
    /// Initializes a new instance of the <see cref="PartsController"/> class.
    /// </summary>
    /// <param name="repository">The repository used to access part data.</param>
    /// <param name="logger">Logger</param>
    public PartsController(IPartsRepository repository, ILogger<PartsController> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    /// <summary>
    /// Retrieves all parts from the inventory.
    /// </summary>
    /// <returns>
    /// A list of <see cref="Part"/> entities wrapped in an <see cref="ActionResult{T}"/>.
    /// Returns 200 OK with the list of parts.
    /// </returns>
    /// <remarks>
    /// This endpoint returns all parts.
    /// </remarks>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<Part>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<Part>>> GetAll()
    {
        var parts = await _repository.GetAllAsync();
        return Ok(parts);
    }

    /// <summary>
    /// Retrieves a specific part by its part number.
    /// </summary>
    /// <param name="partNumber">The unique identifier of the part.</param>
    /// <returns>
    /// An <see cref="ActionResult{T}"/> containing the requested <see cref="Part"/> if found; 
    /// otherwise, returns 404 Not Found if the part does not exist.
    /// </returns>
    /// <response code="200">Returns the part with the specified part number.</response>
    /// <response code="404">If the part is not found.</response>
    [HttpGet("{partNumber}")]
    [ProducesResponseType(typeof(Part), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Part>> Get(string partNumber)
    {
        var part = await _repository.GetByIdAsync(partNumber);
        if (part == null)
        {
            _logger.LogWarning("Part not found: {PartNumber}", partNumber);
            return NotFound();
        }

        return Ok(part);
    }

    /// <summary>
    /// Creates a new part in the inventory.
    /// </summary>
    /// <param name="part">The <see cref="Part"/> entity to create.</param>
    /// <returns>
    /// An <see cref="ActionResult{T}"/> containing the created <see cref="Part"/> with a 201 Created response, 
    /// or a 409 Conflict if a part with the same part number already exists.
    /// </returns>
    /// <response code="201">The part was successfully created.</response>
    /// <response code="409">A part with the given part number already exists.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<Part>> Create(Part part)
    {
        if (await _repository.GetByIdAsync(part.PartNumber) is not null)
            return Conflict($"Part with number {part.PartNumber} already exists.");

        _logger.LogInformation("Creating part: {PartNumber}", part.PartNumber);

        part.LastStockTake = DateTime.UtcNow;

        await _repository.AddAsync(part);
        await _repository.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { partNumber = part.PartNumber }, part);
    }

    /// <summary>
    /// Updates an existing part in the inventory.
    /// </summary>
    /// <param name="partNumber">The part number of the part to update (from the URL).</param>
    /// <param name="part">The updated <see cref="Part"/> object.</param>
    /// <returns>
    /// Returns 204 No Content if the update is successful, 
    /// 400 Bad Request if the URL part number does not match the request body,
    /// or 404 Not Found if the part does not exist.
    /// </returns>
    /// <response code="204">The part was successfully updated.</response>
    /// <response code="400">The part number in the URL does not match the request body.</response>
    /// <response code="404">The part was not found.</response>
    [HttpPut("{partNumber}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(string partNumber, Part part)
    {
        if (partNumber != part.PartNumber)
            return BadRequest("Part number in URL does not match request body.");

        var existing = await _repository.GetByIdAsync(partNumber);
        if (existing is null)
        {
            _logger.LogWarning("Part not found: {PartNumber}", partNumber);
            return NotFound();
        }


        _logger.LogInformation("Updating part: {PartNumber}", partNumber);
        existing.Description = part.Description;
        existing.QuantityOnHand = part.QuantityOnHand;
        existing.LocationCode = part.LocationCode;
        existing.LastStockTake = DateTime.UtcNow;
        //existing.LastStockTake = part.LastStockTake;

        _repository.Update(existing);
        await _repository.SaveChangesAsync();

        return NoContent();
    }

    /// <summary>
    /// Deletes a part from the inventory.
    /// </summary>
    /// <param name="partNumber">The part number of the part to delete.</param>
    /// <returns>
    /// Returns 204 No Content if the deletion is successful, 
    /// or 404 Not Found if the part does not exist.
    /// </returns>
    /// <response code="204">The part was successfully deleted.</response>
    /// <response code="404">The part was not found.</response>
    [HttpDelete("{partNumber}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(string partNumber)
    {
        var part = await _repository.GetByIdAsync(partNumber);
        if (part == null)
        {
            _logger.LogWarning("Part not found: {PartNumber}", partNumber);
            return NotFound();
        }

        _logger.LogInformation("Deleting Part: {PartNumber}", partNumber);
        _repository.Remove(part);
        await _repository.SaveChangesAsync();

        return NoContent();
    }
}
