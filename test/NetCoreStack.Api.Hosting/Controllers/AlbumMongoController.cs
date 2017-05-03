using Microsoft.AspNetCore.Mvc;
using NetCoreStack.Contracts;
using NetCoreStack.Data.Interfaces;
using NetCoreStack.Domain.Contracts;
using NetCoreStack.Mvc.Extensions;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreStack.WebClient.Hosting.Controllers
{
    [Route("api/[controller]")]
    public class AlbumMongoController : ControllerBase
    {
        private readonly IMongoUnitOfWork _unitOfWork;

        public AlbumMongoController(IMongoUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet(nameof(GetInitialContext))]
        public async Task<InitRequestContext> GetInitialContext()
        {
            await Task.CompletedTask;

            var initRequestContext = new InitRequestContext();
            initRequestContext.Artists = _unitOfWork.Repository<ArtistBson>().Select(a => new IdTextPair { Id = a.Id.ToString(), Text = a.Name }).ToList();
            initRequestContext.Genres = _unitOfWork.Repository<GenreBson>().Select(a => new IdTextPair { Id = a.Id.ToString(), Text = a.Name }).ToList();

            return initRequestContext;
        }

        [HttpGet(nameof(GetArtistListAsync))]
        public async Task<CollectionResult<IdTextPair>> GetArtistListAsync([FromQuery]AutoCompleteRequest request)
        {
            await Task.CompletedTask;

            var query = _unitOfWork.Repository<ArtistBson>()
                .Select(x => new IdTextPair
                {
                    Id = x.Id.ToString(),
                    Text = x.Name
                });

            if (request?.SelectedIds != null && request.SelectedIds.Any())
            {
                return query.Where(x => request.SelectedIds.Contains(x.Id)).ToCollectionResult();
            }

            return query.ToCollectionResult(request);
        }

        [HttpGet(nameof(GetAlbums))]
        public async Task<CollectionResult<AlbumBsonViewModel>> GetAlbums([FromQuery]CollectionRequest request)
        {
            await Task.CompletedTask;

            var query = _unitOfWork.Repository<AlbumBson>().Select(x => new AlbumBsonViewModel
            {
                Id = x.Id,
                AlbumArtUrl = x.AlbumArtUrl,
                Title = x.Title,
                Genre = x.Genre.Name,
                Artist = x.Artist.Name,
                Price = x.Price
            });

            return query.ToCollectionResult(request);
        }

        [HttpPost(nameof(SaveAlbum))]
        public void SaveAlbum([FromBody]AlbumViewModel model)
        {
        }
    }
}
