using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StrokeForEgypt.AdminApp.Filters;
using StrokeForEgypt.AdminApp.ViewModel;
using StrokeForEgypt.Common;
using StrokeForEgypt.Entity.SponsorEntity;
using StrokeForEgypt.Repository;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static StrokeForEgypt.Common.EnumData;

namespace StrokeForEgypt.AdminApp.Controllers.SponsorEntity
{
    public class SponsorController : Controller
    {
        private readonly UnitOfWork _UnitOfWork;
        private readonly IMapper _Mapper;
        private readonly ISession _Session;

        public SponsorController(UnitOfWork UnitOfWork, IMapper Mapper, IHttpContextAccessor HttpContextAccessor)
        {
            _UnitOfWork = UnitOfWork;
            _Mapper = Mapper;
            _Session = HttpContextAccessor.HttpContext.Session;
        }

        [Authorize((int)AccessLevelEnum.ReadAccess)]
        public IActionResult Index(int Id, int Fk_Event,int Fk_SponsorType, bool ProfileLayOut = false)
        {
            SponsorFilter Filter = new SponsorFilter
            {
                Id = Id,
                Fk_Event = Fk_Event,
                Fk_SponsorType = Fk_SponsorType
            };
            ViewData["ProfileLayOut"] = ProfileLayOut;
            return View("~/Views/SponsorEntity/Sponsor/Index.cshtml", Filter);
        }

        [HttpPost]
        [Authorize((int)AccessLevelEnum.ReadAccess)]
        public async Task<IActionResult> LoadTable([FromBody] SponsorFilter dtParameters)
        {
            string searchBy = dtParameters.Search?.Value;

            List<Sponsor> result = await _UnitOfWork.Sponsor.GetAll(a => (dtParameters.Id == 0 || a.Id == dtParameters.Id)
                                                                                         && (dtParameters.Fk_Event == 0 || a.Fk_Event == dtParameters.Fk_Event)
                                                                                         &&(dtParameters.Fk_SponsorType == 0 || a.Fk_SponsorType == dtParameters.Fk_SponsorType)
                                                                                         , new List<string> { "SponsorType" });


            if (!string.IsNullOrEmpty(searchBy))
            {
                result = result.Where(a => a.Name.ToLower().Contains(searchBy.ToLower())
                                        || a.IsActive.ToString().ToLower().Contains(searchBy.ToLower())
                                        || a.SponsorType.Name.ToLower().ToLower().Contains(searchBy.ToLower())
                                        || (a.ImageURL!=null&&a.ImageURL.ToLower().Contains(searchBy.ToLower()))
                                        || a.Id.ToString().ToLower().Contains(searchBy.ToLower()))
                               .ToList();
            }

            result.ForEach(a => a.SponsorType.Sponsors = null);

            DataTableManager<Sponsor> DataTableManager = new DataTableManager<Sponsor>();

            DataTableResult<Sponsor> DataTableResult = DataTableManager.LoadTable(dtParameters, result, _UnitOfWork.Sponsor.Count());

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
            Sponsor Sponsor = await _UnitOfWork.Sponsor.GetByID(id);

            if (Sponsor == null)
            {
                return NotFound();
            }

            return View("~/Views/SponsorEntity/Sponsor/Details.cshtml", Sponsor);
        }

        [Authorize((int)AccessLevelEnum.CreateOrUpdateAccess)]
        public async Task<IActionResult> CreateOrEdit(int id = 0, int Fk_Event = 0)
        {
            Sponsor Sponsor = new Sponsor();

            if (id > 0)
            {
                Sponsor = await _UnitOfWork.Sponsor.GetByID(id);
                if (Sponsor == null)
                {
                    return NotFound();
                }
            }
           
            return View("~/Views/SponsorEntity/Sponsor/CreateOrEdit.cshtml", Sponsor);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize((int)AccessLevelEnum.CreateOrUpdateAccess)]
        public async Task<IActionResult> CreateOrEdit(int id, Sponsor Sponsor)
        {
            if (id != Sponsor.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (id == 0)
                    {
                        Sponsor.CreatedBy = _Session.GetString("FullName");
                        _UnitOfWork.Sponsor.CreateEntity(Sponsor);
                        await _UnitOfWork.Sponsor.Save();
                    }
                    else
                    {
                        Sponsor Data = await _UnitOfWork.Sponsor.GetByID(id);

                        Sponsor.LastModifiedBy = _Session.GetString("FullName");

                        _Mapper.Map(Sponsor, Data);


                        _UnitOfWork.Sponsor.UpdateEntity(Data);
                        await _UnitOfWork.Sponsor.Save();

                        Sponsor = Data;
                    }

                    IFormFile ImageFile = HttpContext.Request.Form.Files["ImageFile"];

                    if (ImageFile != null)
                    {
                        ImgManager ImgManager = new ImgManager(AppMainData.WebRootPath);

                        string FileURL = await ImgManager.UploudImage(AppMainData.DomainName, Sponsor.Id.ToString(), ImageFile, "Uploud/Sponsor");

                        if (!string.IsNullOrEmpty(FileURL))
                        {
                            if (!string.IsNullOrEmpty(Sponsor.ImageURL))
                            {
                                ImgManager.DeleteImage(Sponsor.ImageURL, AppMainData.DomainName);
                            }
                            Sponsor.ImageURL = FileURL;
                            _UnitOfWork.Sponsor.UpdateEntity(Sponsor);
                            await _UnitOfWork.Sponsor.Save();
                        }
                    }
                }

                catch (DbUpdateConcurrencyException)
                {
                    if (!_UnitOfWork.Sponsor.Any(a => a.Id == id))
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

            return View("~/Views/SponsorEntity/Sponsor/CreateOrEdit.cshtml", Sponsor);

        }

        [Authorize((int)AccessLevelEnum.FullAccess)]
        public async Task<IActionResult> Delete(int id)
        {
            Sponsor Sponsor = await _UnitOfWork.Sponsor.GetByID(id);

            if (Sponsor == null)
            {
                return NotFound();
            }
          

            return View("~/Views/SponsorEntity/Sponsor/Delete.cshtml", Sponsor);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize((int)AccessLevelEnum.FullAccess)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Sponsor Sponsor = await _UnitOfWork.Sponsor.GetByID(id);
           
                if (!string.IsNullOrEmpty(Sponsor.ImageURL))
                {
                    ImgManager ImgManager = new ImgManager(AppMainData.WebRootPath);
                    ImgManager.DeleteImage(Sponsor.ImageURL, AppMainData.DomainName);
                }

                _UnitOfWork.Sponsor.DeleteEntity(Sponsor);
                await _UnitOfWork.Sponsor.Save();

            return RedirectToAction(nameof(Index));
        }
    }
}
