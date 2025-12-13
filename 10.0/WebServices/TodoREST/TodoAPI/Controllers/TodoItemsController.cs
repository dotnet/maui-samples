using Microsoft.AspNetCore.Mvc;
using TodoAPI.Interfaces;
using TodoAPI.Models;

namespace TodoAPI.Controllers
{
    #region snippetErrorCode
    public enum ErrorCode
    {
        TodoItemNameAndNotesRequired,
        TodoItemIDInUse,
        RecordNotFound,
        CouldNotCreateItem,
        CouldNotUpdateItem,
        CouldNotDeleteItem
    }
    #endregion

    #region snippetDI
    [ApiController]
    [Route("api/[controller]")]
    public class TodoItemsController : ControllerBase
    {
        private readonly ITodoRepository _todoRepository;

        public TodoItemsController(ITodoRepository todoRepository)
        {
            _todoRepository = todoRepository;
        }
        #endregion

        #region snippet
        [HttpGet]
        public IActionResult List()
        {
            return Ok(_todoRepository.All);
        }
        #endregion

        #region snippetCreate
        [HttpPost]
        public IActionResult Create([FromBody]TodoItem item)
        {
            try
            {
                if (item == null || !ModelState.IsValid)
                {
                    return BadRequest(ErrorCode.TodoItemNameAndNotesRequired.ToString());
                }
                bool itemExists = _todoRepository.DoesItemExist(item.ID);
                if (itemExists)
                {
                    return StatusCode(StatusCodes.Status409Conflict, ErrorCode.TodoItemIDInUse.ToString());
                }
                _todoRepository.Insert(item);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest($"{ErrorCode.CouldNotCreateItem}: {ex.Message}");
            }
            catch (ArgumentException ex)
            {
                return BadRequest($"{ErrorCode.CouldNotCreateItem}: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Log the exception here in production
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    $"{ErrorCode.CouldNotCreateItem}: An unexpected error occurred");
            }
            return Ok(item);
        }
        #endregion

        #region snippetEdit
        [HttpPut]
        public IActionResult Edit([FromBody] TodoItem item)
        {
            try
            {
                if (item == null || !ModelState.IsValid)
                {
                    return BadRequest(ErrorCode.TodoItemNameAndNotesRequired.ToString());
                }
                var existingItem = _todoRepository.Find(item.ID);
                if (existingItem == null)
                {
                    return NotFound(ErrorCode.RecordNotFound.ToString());
                }
                _todoRepository.Update(item);
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest($"{ErrorCode.CouldNotUpdateItem}: {ex.Message}");
            }
            catch (ArgumentException ex)
            {
                return BadRequest($"{ErrorCode.CouldNotUpdateItem}: {ex.Message}");
            }
            catch (InvalidOperationException ex)
            {
                return NotFound($"{ErrorCode.RecordNotFound}: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Log the exception here in production
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    $"{ErrorCode.CouldNotUpdateItem}: An unexpected error occurred");
            }
            return NoContent();
        }
        #endregion
        
        #region snippetDelete
        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            try
            {
                var item = _todoRepository.Find(id);
                if (item == null)
                {
                    return NotFound(ErrorCode.RecordNotFound.ToString());
                }
                _todoRepository.Delete(id);
            }
            catch (ArgumentException ex)
            {
                return BadRequest($"{ErrorCode.CouldNotDeleteItem}: {ex.Message}");
            }
            catch (InvalidOperationException ex)
            {
                return NotFound($"{ErrorCode.RecordNotFound}: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Log the exception here in production
                return StatusCode(StatusCodes.Status500InternalServerError, 
                    $"{ErrorCode.CouldNotDeleteItem}: An unexpected error occurred");
            }
            return NoContent();
        }
        #endregion
    }
}
