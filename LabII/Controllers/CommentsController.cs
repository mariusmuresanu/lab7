using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LabII.DTOs;
using LabII.Models;
using LabII.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LabII.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private ICommentService commentService;
        private IUsersService usersService;

        public CommentsController(ICommentService commentService, IUsersService usersService)
        {
            this.commentService = commentService;
        }
        ///<remarks>
        ///{
        ///"id": 1,
        ///"text": "What?! 777.55",
        ///"important": true,
        ///"expenseId": 7
        ///}
        ///</remarks>
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filter">Optional, filtered by text</param>
        /// <param name="page"></param>
        /// <returns>List of comments</returns>

        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[Authorize(Roles = "Regular, Admin")]
        // GET: api/Comments
        [HttpGet]
        public PaginatedList<CommentGetModel> Get([FromQuery]string filter, [FromQuery]int page = 1)
        {
            page = Math.Max(page, 1);
            return commentService.GetAll(filter, page);
        }
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin,Regular")]
        public IActionResult Get(int id)
        {
            var found = commentService.GetById(id);
            if (found == null)
            {
                return NotFound();
            }

            return Ok(found);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Regular")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        public void Post([FromBody] CommentPostModel comment)
        {
            User addedBy = usersService.GetCurrentUser(HttpContext);
            commentService.Create(comment, addedBy);
        }

        [Authorize(Roles = "Admin,Regular")]
        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Comment comment)
        {
            var result = commentService.Upsert(id, comment);
            return Ok(result);
        }

        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Regular")]
        public IActionResult Delete(int id)
        {
            var result = commentService.Delete(id);
            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }
    }


}