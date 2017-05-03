using NetCoreStack.Contracts;
using System.Threading.Tasks;

namespace NetCoreStack.Domain.Contracts.ApiContracts
{
    [ApiRoute("api/[controller]", regionKey: "Main")]
    public interface IAlbumApi : IApiContract
    {
        Task<CollectionResult<AlbumViewModel>> GetAlbums(CollectionRequest request);
        Task<CollectionResult<IdTextPair>> GetArtistListAsync(AutoCompleteRequest request);
        Task<InitRequestContext> GetInitialContext();

        [HttpPostMarker]
        Task<AlbumViewModel> SaveAlbum(AlbumViewModel model);

        [HttpPutMarker]
        Task<AlbumViewModel> UpdateAlbum(long id, AlbumViewModel model);

        [HttpDeleteMarker]
        Task DeleteAlbum(long id);
    }
}