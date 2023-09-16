using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shop.Domain.DomainModels;
using Shop.Domain.DTO;
using Shop.Service.Interface;
using System.Security.Claims;

namespace Shop.Web.Controllers
{
    public class ComicsController : Controller
    {
        private readonly IComicService _comicService;
        private readonly ILogger<ComicsController> _logger;

        public ComicsController(ILogger<ComicsController> logger, IComicService comicService)
        {
            _logger = logger;
            _comicService = comicService;
        }

        // GET: Comics
        [AllowAnonymous]
        public IActionResult Index(string? search, string? genre)
        {
            _logger.LogInformation("User Request -> Get All Comics!");

            List<Comic> result = new List<Comic>();

            if (search == null)
            { 
                if (genre == null)
                    result = this._comicService.GetAllComics().OrderBy(comic => comic.ComicPrice).ToList();
                else
                    result = this._comicService.GetAllComics().Where(comic => comic.ComicGenre.Equals(genre)).OrderBy(comic => comic.ComicPrice).ToList();
            }
            else
                result = this._comicService.GetAllComics().Where(comic => comic.ComicName.ToLower().Contains(search)).OrderBy(comic => comic.ComicPrice).ToList();

            return View(result);
        }

        // GET: Comics/Details/5
        [AllowAnonymous]
        public IActionResult Details(Guid? id)
        {
            _logger.LogInformation("User Request -> Get Details For Comic");

            if (id == null)
            {
                return NotFound();
            }

            var comic = this._comicService.GetDetailsForComic(id);

            if (comic == null)
            {
                return NotFound();
            }

            return View(comic);
        }

        // GET: Comics/Create
        [Authorize(Roles = "Administrator")]
        public IActionResult Create()
        {
            _logger.LogInformation("User Request -> Get create form for Comic!");

            return View();
        }

        // POST: Comics/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public IActionResult Create([Bind("Id,ComicName,ComicCover,ComicDescription,ComicGenre,ComicAuthor,ComicPublisher,ComicPrice")] Comic comic)
        {
            _logger.LogInformation("User Request -> Insert Comic in DataBase!");

            if (ModelState.IsValid)
            {
                comic.Id = Guid.NewGuid();
                this._comicService.CreateNewComic(comic);
                return RedirectToAction(nameof(Index));
            }

            return View(comic);
        }

        // GET: Comics/Edit/5
        [Authorize(Roles = "Administrator")]
        public IActionResult Edit(Guid? id)
        {
            _logger.LogInformation("User Request -> Get edit form for Comic!");
            if (id == null)
            {
                return NotFound();
            }

            var comic = this._comicService.GetDetailsForComic(id);

            if (comic == null)
            {
                return NotFound();
            }

            return View(comic);
        }

        // POST: Comics/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public IActionResult Edit(Guid id, [Bind("Id,ComicName,ComicCover,ComicDescription,ComicGenre,ComicAuthor,ComicPublisher,ComicPrice")] Comic comic)
        {
            _logger.LogInformation("User Request -> Update Comic in DataBase!");

            if (id != comic.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    this._comicService.UpdateExistingComic(comic);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ComicExists(comic.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(comic);
        }

        // GET: Comics/Delete/5
        [Authorize(Roles = "Administrator")]
        public IActionResult Delete(Guid? id)
        {
            _logger.LogInformation("User Request -> Get delete form for Comic!");

            if (id == null)
            {
                return NotFound();
            }

            var comic = this._comicService.GetDetailsForComic(id);

            if (comic == null)
            {
                return NotFound();
            }
            return View(comic);
        }

        // POST: Comics/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrator")]
        public IActionResult DeleteConfirmed(Guid id)
        {
            _logger.LogInformation("User Request -> Delete Comic in DataBase!");

            this._comicService.DeleteComic(id);

            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        public IActionResult AddComicToCart(Guid id)
        {
            var result = this._comicService.GetShoppingCartInfo(id);

            return View(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult AddComicToCart(AddToShoppingCartDto model)
        {
            _logger.LogInformation("User Request -> Add Comic in ShoppingCart and save changes in DataBase!");

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var result = this._comicService.AddToShoppingCart(model, userId);

            if (result)
            {
                return RedirectToAction("Index", "Comics");
            }

            return View(model);
        }

        private bool ComicExists(Guid id)
        {
            return this._comicService.GetDetailsForComic(id) != null;
        }

        [Authorize(Roles = "Administrator")]
        public FileContentResult ExportComics(string? genre)
        {
            HttpClient client = new HttpClient();

            string URL = "https://localhost:7060/api/Admin/GetComics";

            HttpResponseMessage response = client.GetAsync(URL).Result;

            var result = response.Content.ReadAsAsync<List<Comic>>().Result;

            List<Comic> filtered = new List<Comic>();

            if (genre.Equals("All"))
                filtered = result.ToList();
            else
                filtered = result.Where(comic => comic.ComicGenre.Equals(genre)).ToList();


            string fileName = "Comics.xlsx";
            string contentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";

            using (var workBook = new XLWorkbook())
            {
                IXLWorksheet worksheet = workBook.Worksheets.Add("All Comics");

                worksheet.Cell(1, 1).Value = "Comic Name";
                worksheet.Cell(1, 2).Value = "Comic Genre";
                worksheet.Cell(1, 3).Value = "Comic Price";

                for (int i = 1; i <= filtered.Count(); i++)
                {

                    var item = filtered[i - 1];

                    worksheet.Cell(i + 1, 1).Value = item.ComicName;
                    worksheet.Cell(i + 1, 2).Value = item.ComicGenre;
                    worksheet.Cell(i + 1, 3).Value = item.ComicPrice;

                }

                using (var stream = new MemoryStream())
                {
                    workBook.SaveAs(stream);

                    var content = stream.ToArray();

                    return File(content, contentType, fileName);
                }
            }

        }
    }
}
