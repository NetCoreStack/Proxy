﻿using Microsoft.AspNetCore.Mvc;
using NetCoreStack.Contracts;
using NetCoreStack.Data.Interfaces;
using NetCoreStack.Domain.Contracts;
using NetCoreStack.Domain.Contracts.ApiContracts;
using NetCoreStack.Mvc.Extensions;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace NetCoreStack.WebClient.Hosting.Controllers
{
    [Route("api/[controller]")]
    public class AlbumController : ControllerBase, IAlbumApi
    {
        private readonly ISqlUnitOfWork _unitOfWork;

        public AlbumController(ISqlUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet(nameof(GetInitialContext))]
        public async Task<InitRequestContext> GetInitialContext()
        {
            await Task.CompletedTask;

            var initRequestContext = new InitRequestContext();
            initRequestContext.Artists = _unitOfWork.Repository<Artist>().Select(a => new IdTextPair { Id = a.Id.ToString(), Text = a.Name }).ToList();
            initRequestContext.Genres = _unitOfWork.Repository<Genre>().Select(a => new IdTextPair { Id = a.Id.ToString(), Text = a.Name }).ToList();

            return initRequestContext;
        }

        [HttpGet(nameof(GetArtistListAsync))]
        public async Task<CollectionResult<IdTextPair>> GetArtistListAsync([FromQuery]AutoCompleteRequest request)
        {
            await Task.CompletedTask;

            var query = _unitOfWork.Repository<Artist>()
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
        public async Task<CollectionResult<AlbumViewModel>> GetAlbums([FromQuery]CollectionRequest request)
        {
            await Task.CompletedTask;

            var query = _unitOfWork.Repository<Album>().Select(x => new AlbumViewModel
            {
                Id = x.Id,
                AlbumArtUrl = x.AlbumArtUrl,
                Title = x.Title,
                Genre = x.Genre.Name,
                Artist = x.Artist.Name,
                ArtistId = x.ArtistId,
                GenreId = x.GenreId,
                Price = x.Price
            });

            return query.ToCollectionResult(request);
        }

        [HttpPost(nameof(SaveAlbumAsync))]
        public async Task<AlbumViewModel> SaveAlbumAsync([FromBody]AlbumViewModel model)
        {
            await Task.CompletedTask;

            long id = 0;
            var objectState = ObjectState.Added;
            if (!model.IsNew)
            {
                id = model.Id;
                objectState = ObjectState.Modified;
            }

            var album = new Album
            {
                AlbumArtUrl = model.AlbumArtUrl,
                ArtistId = model.ArtistId,
                GenreId = model.GenreId,
                Id = id,
                ObjectState = objectState,
                Price = model.Price,
                Title = model.Title
            };

            _unitOfWork.Repository<Album>().SaveAllChanges(album);
            return model;
        }

        [HttpPost(nameof(SaveAlbumSubmitAsync))]
        public async Task<AlbumViewModel> SaveAlbumSubmitAsync(AlbumViewModelSubmit model)
        {
            await Task.CompletedTask;

            long id = 0;
            var objectState = ObjectState.Added;
            if (!model.IsNew)
            {
                id = model.Id;
                objectState = ObjectState.Modified;
            }

            var album = new Album
            {
                AlbumArtUrl = model.AlbumArtUrl,
                ArtistId = model.ArtistId,
                GenreId = model.GenreId,
                Id = id,
                ObjectState = objectState,
                Price = model.Price,
                Title = model.Title
            };

            _unitOfWork.Repository<Album>().SaveAllChanges(album);

            if (model.Image != null && model.Image.Length > 0)
            {
                // full path to file in temp location
                var filePath = Path.GetTempFileName();

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await model.Image.CopyToAsync(stream);
                }
            }

            return model;
        }

        [HttpPut(nameof(UpdateAlbumAsync))]
        public async Task<AlbumViewModel> UpdateAlbumAsync(long id, [FromBody]AlbumViewModel model)
        {
            await Task.CompletedTask;
            return model;
        }

        [HttpDelete(nameof(DeleteAlbum))]
        public async Task DeleteAlbum(long id)
        {
            await Task.CompletedTask;
        }
    }
}
