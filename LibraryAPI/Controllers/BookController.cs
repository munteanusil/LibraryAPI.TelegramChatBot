using AutoMapper;
using FluentValidation;
using FluentValidation.Results;
using Library.Application.DTOs;
using Library.Application.DTOs.Authors;
using Library.Application.DTOs.Books;
using Library.Application.Interfaces;
using Library.Domain.Common;
using Library.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.ComponentModel.DataAnnotations;

namespace LibraryAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
   
    public class BookController : Controller
    {
        private readonly ILogger<BookController> _logger;
        private readonly IBookRepository _repository;
        private readonly IMapper _mapper;
      
        public BookController(IBookRepository repository,
            IMapper mapper)
        {
            _repository= repository;
            _mapper = mapper;
        }


        [HttpPost]
        public async Task<IActionResult> Post(CreateBookDto bookDto, CancellationToken cancellationToken)
        {
               var bookToCreate = _mapper.Map<Book>(bookDto); 
                await _repository.CreateBook(bookToCreate, cancellationToken);
                return CreatedAtAction(nameof(Get),new {id = bookToCreate.Id},_mapper.Map<BookDto>(bookToCreate));
            

        }

        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] PaginatedDto dto, CancellationToken cancellationToken)
        {
          
               
            var books = await _repository.GetBooks(dto.Page, dto.PageSize);
            var booksDto = _mapper.Map<List<BookDto>>(books.Items);
            var result = new PaginatedList<BookDto>(booksDto, dto.Page, dto.PageSize);
            return Ok(result);
        }


        [HttpGet("{id}")]
        public async Task<ActionResult> Get(int id, CancellationToken cancellationToken)
        {
            var book = await _repository.GetBookById(id, cancellationToken);
            if(book == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<BookDto>(book));
        }

        [HttpPut]
        public async Task<IActionResult> Put(BookDto bookdto, CancellationToken cancellationToken)
        {
           
                await _repository.UpdateBook(_mapper.Map<Book>(bookdto),cancellationToken);
                return Ok();
            
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id,CancellationToken cancellationToken)
        {
            await _repository.DeleteBook(id,cancellationToken);
            return Ok();
        }

    }
}
