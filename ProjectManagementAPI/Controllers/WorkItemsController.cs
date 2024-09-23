using Microsoft.AspNetCore.Mvc;
using ProjectManagementAPI.Application.Interfaces;
using ProjectManagementAPI.Domain.Entities;

[Route("api/[controller]")]
[ApiController]
public class WorkItemsController : ControllerBase
{
    private readonly IWorkItemService _workItemService;

    public WorkItemsController(IWorkItemService workItemService)
    {
        _workItemService = workItemService;
    }

    [HttpGet("Project/{projectId}")]
    public async Task<ActionResult<IEnumerable<WorkItem>>> GetWorkItemsByProject(int projectId)
    {
        return Ok(await _workItemService.GetWorkItemsByProjectIdAsync(projectId));
    }

    [HttpPost]
    public async Task<ActionResult<WorkItem>> PostWorkItem(WorkItem workItem)
    {
        var createdWorkItem = await _workItemService.CreateWorkItemAsync(workItem);
        return CreatedAtAction(nameof(GetWorkItemsByProject), new { projectId = workItem.ProjectId }, createdWorkItem);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> PutWorkItem(int id, WorkItem workItem, string updatedBy)
    {
        if (id != workItem.Id)
        {
            return BadRequest();
        }

        await _workItemService.UpdateWorkItemAsync(workItem, updatedBy);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteWorkItem(int id)
    {
        await _workItemService.DeleteWorkItemAsync(id);
        return NoContent();
    }

    [HttpPost("{id}/comments")]
    public async Task<IActionResult> AddComment(int id, [FromBody] string commentText, string commentedBy)
    {
        await _workItemService.AddCommentAsync(id, commentText, commentedBy);
        return Ok(new { Message = "Comentário adicionado com sucesso" });
    }
}
