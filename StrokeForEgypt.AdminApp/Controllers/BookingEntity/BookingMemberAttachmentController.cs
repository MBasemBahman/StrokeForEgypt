using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StrokeForEgypt.AdminApp.Filters;
using StrokeForEgypt.AdminApp.ViewModel;
using StrokeForEgypt.Common;
using StrokeForEgypt.Entity.BookingEntity;
using StrokeForEgypt.Repository;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static StrokeForEgypt.Common.EnumData;

namespace StrokeForEgypt.AdminApp.Controllers.BookingEntity
{
    public class BookingMemberAttachmentController : Controller
    {
        private readonly UnitOfWork _UnitOfWork;
        private readonly IMapper _Mapper;
        private readonly ISession _Session;

        public BookingMemberAttachmentController(UnitOfWork UnitOfWork, IMapper Mapper, IHttpContextAccessor HttpContextAccessor)
        {
            _UnitOfWork = UnitOfWork;
            _Mapper = Mapper;
            _Session = HttpContextAccessor.HttpContext.Session;
        }

        [Authorize((int)AccessLevelEnum.ReadAccess)]
        public IActionResult Index(int Id, int Fk_BookingMember, bool ProfileLayOut = false)
        {
            BookingMemberAttachmentFilter Filter = new BookingMemberAttachmentFilter
            {
                Id = Id,
                Fk_BookingMember = Fk_BookingMember
            };

            ViewData["ProfileLayOut"] = ProfileLayOut;

            return View("~/Views/BookingEntity/BookingMemberAttachment/Index.cshtml", Filter);

        }

        [HttpPost]
        public async Task<IActionResult> LoadTable([FromBody] BookingMemberAttachmentFilter dtParameters)
        {
            string searchBy = dtParameters.Search?.Value;

            List<BookingMemberAttachment> result = await _UnitOfWork.BookingMemberAttachment.GetAll(a => (dtParameters.Id == 0 || a.Id == dtParameters.Id)
                                                                                            && (dtParameters.Fk_BookingMember == 0 || a.Fk_BookingMember == dtParameters.Fk_BookingMember));

            if (!string.IsNullOrEmpty(searchBy))
            {
                result = result.Where(a => a.Id.ToString().Contains(searchBy.ToLower())
                                        || a.FileURL == null || a.FileURL.ToLower().Contains(searchBy.ToLower())
                                        || a.CreatedAtstring.Contains(searchBy.ToLower())
                                        || a.Id.ToString().ToLower().Contains(searchBy.ToLower()))
                               .ToList();
            }

            DataTableManager<BookingMemberAttachment> DataTableManager = new DataTableManager<BookingMemberAttachment>();

            DataTableResult<BookingMemberAttachment> DataTableResult = DataTableManager.LoadTable(dtParameters, result, _UnitOfWork.BookingMemberAttachment.Count());

            return Json(new
            {
                draw = DataTableResult.DtParameters.Draw,
                recordsTotal = DataTableResult.TotalResultsCount,
                recordsFiltered = DataTableResult.FilteredResultsCount,
                data = DataTableResult.Data
                                      .Skip(DataTableResult.DtParameters.Start)
                                      .Take(DataTableResult.DtParameters.Length)
                                      .ToList()
            });
        }

        [Authorize((int)AccessLevelEnum.ReadAccess)]
        public async Task<IActionResult> Details(int id)
        {
            BookingMemberAttachment BookingMemberAttachment = await _UnitOfWork.BookingMemberAttachment.GetByID(id);

            if (BookingMemberAttachment == null)
            {
                return NotFound();
            }

            return View("~/Views/BookingEntity/BookingMemberAttachment/Details.cshtml", BookingMemberAttachment);
        }


        [Authorize((int)AccessLevelEnum.CreateOrUpdateAccess)]
        public async Task<IActionResult> Uploud(int Id)
        {
            ImgManager ImgManager = new ImgManager(AppMainData.WebRootPath);

            IFormFile Images = HttpContext.Request.Form.Files["file"];
            if (Images != null)
            {
                BookingMemberAttachment BookingMemberAttachment = new BookingMemberAttachment
                {
                    Fk_BookingMember = Id,
                    FileType = Images.ContentType,
                    FileName = Images.FileName,
                    FileLength = Images.Length,
                    FileURL = AppMainData.DomainName

                };
                BookingMemberAttachment.CreatedBy = _Session.GetString("FullName");
                _UnitOfWork.BookingMemberAttachment.CreateEntity(BookingMemberAttachment);
                await _UnitOfWork.BookingMemberAttachment.Save();

                string ImageURL = await ImgManager.UploudImage(AppMainData.DomainName, BookingMemberAttachment.Id.ToString(), Images, "Uploud/BookingMemberAttachment");

                if (!string.IsNullOrEmpty(ImageURL))
                {
                    BookingMemberAttachment.FileURL = ImageURL;
                    _UnitOfWork.BookingMemberAttachment.UpdateEntity(BookingMemberAttachment);
                    await _UnitOfWork.BookingMemberAttachment.Save();
                }
            }
            return Ok();
        }



        [Authorize((int)AccessLevelEnum.FullAccess)]
        public async Task<IActionResult> Delete(int id)
        {
            BookingMemberAttachment BookingMemberAttachment = await _UnitOfWork.BookingMemberAttachment.GetByID(id);

            if (BookingMemberAttachment == null)
            {
                return NotFound();
            }

            return View("~/Views/BookingEntity/BookingMemberAttachment/Delete.cshtml", BookingMemberAttachment);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize((int)AccessLevelEnum.FullAccess)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            BookingMemberAttachment BookingMemberAttachment = await _UnitOfWork.BookingMemberAttachment.GetByID(id);

            if (!string.IsNullOrEmpty(BookingMemberAttachment.FileURL))
            {
                ImgManager ImgManager = new ImgManager(AppMainData.WebRootPath);
                ImgManager.DeleteImage(BookingMemberAttachment.FileURL, AppMainData.DomainName);
            }

            _UnitOfWork.BookingMemberAttachment.DeleteEntity(BookingMemberAttachment);
            await _UnitOfWork.BookingMemberAttachment.Save();

            return RedirectToAction("Index", "BookingMemberAttachment", new { Fk_BookingMember = BookingMemberAttachment.Fk_BookingMember });
        }
    }
}
