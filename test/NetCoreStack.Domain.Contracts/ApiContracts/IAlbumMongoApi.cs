using NetCoreStack.Contracts;
using System.Threading.Tasks;

namespace NetCoreStack.Domain.Contracts.ApiContracts
{
    [ApiRoute("api/[controller]", regionKey: "Main")]
    public interface IAlbumMongoApi : IApiContract
    {
        Task<CollectionResult<AlbumBsonViewModel>> GetAlbums(CollectionRequest request);
        Task<CollectionResult<IdTextPair>> GetArtistListAsync(AutoCompleteRequest request);
        Task<InitRequestContext> GetInitialContext();

        [HttpPostMarker]
        void SaveAlbum(AlbumBsonViewModel model);
    }
}