using Microsoft.AspNetCore.Mvc;
using NetCoreStack.Contracts;
using NetCoreStack.Domain.Contracts;
using NetCoreStack.Domain.Contracts.ApiContracts;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace NetCoreStack.WebClient.Hosting.Controllers
{
    public class Cached
    {
        public static List<IdTextPair> Artists { get; set; }
        public static List<IdTextPair> Genres { get; set; }

        static Cached()
        {
            Artists = new List<IdTextPair>();
            Genres = new List<IdTextPair>();
        }
    }

    public class HomeController : Controller
    {
        private readonly IAlbumApi _albumApi;
        public HomeController(IAlbumApi albumApi)
        {
            _albumApi = albumApi;
        }

        private async Task IndexInitializer()
        {
            if (!Cached.Artists.Any())
            {
                var initContext = await _albumApi.GetInitialContext();
                Cached.Artists = initContext.Artists;
                Cached.Genres = initContext.Genres;
            }

            ViewBag.Artists = Cached.Artists;
            ViewBag.Genres = Cached.Genres;
        }

        public async Task<IActionResult> Index()
        {
            await IndexInitializer();
            return View();
        }

        public async Task<IActionResult> IndexSubmit()
        {
            await IndexInitializer();
            return View();
        }

        public async Task<IActionResult> IndexMongo()
        {
            await IndexInitializer();
            return View();
        }

        public async Task<IActionResult> GetArtistList(AutoCompleteRequest request)
        {
            var artistCollection = await _albumApi.GetArtistListAsync(request);
            return Json(artistCollection);
        }

        public async Task<IActionResult> GetAlbums(CollectionRequest request)
        {
            var albumCollection = await _albumApi.GetAlbums(request);
            return Json(albumCollection);
        }

        public async Task<IActionResult> GetBsonAlbums(CollectionRequest request)
        {
            var albumMongoApi = HttpContext.RequestServices.GetService<IAlbumMongoApi>();
            var albumCollection = await albumMongoApi.GetAlbums(request);
            return Json(albumCollection);
        }

        [HttpPost]
        public async Task<IActionResult> SaveAlbum([FromBody]AlbumViewModel model)
        {
            var albumCollection = await _albumApi.SaveAlbum(model);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> SaveAlbumSubmit(AlbumViewModelSubmit model)
        {
            var albumCollection = await _albumApi.SaveAlbumSubmit(model);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAlbum()
        {
            var albumCollection = await _albumApi.UpdateAlbum(1, new AlbumViewModel { Id = 1, Title = "Test" });
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAlbum()
        {
            await _albumApi.DeleteAlbum(1);
            return Ok();
        }
    }
}
