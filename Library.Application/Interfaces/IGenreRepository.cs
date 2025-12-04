using Library.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Library.Domain.Common;

namespace Library.Application.Interfaces
{
    public interface IGenreRepository
    {
        Task<PaginetedList<Genre>> GetGenres(int page, int pageSize, CancellationToken ct = default);

        Task<Genre> GetGenreById(int id,CancellationToken ct = default);

        Task CreateGenre(Genre book, CancellationToken ct = default);
        Task UpdateGenre(Genre book, CancellationToken ct = default);
        Task DeleteGenre(int id , CancellationToken ct = default);
      
    }
}
