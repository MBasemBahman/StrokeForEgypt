using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StrokeForEgypt.AdminApp.Filters;
using StrokeForEgypt.AdminApp.ViewModel;
using StrokeForEgypt.Common;
using StrokeForEgypt.Entity.EventEntity;
using StrokeForEgypt.Repository;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using static StrokeForEgypt.Common.EnumData;

namespace StrokeForEgypt.AdminApp.Controllers.EventEntity
{
    public class EventAgendaController : Controller
    {
        private readonly UnitOfWork _UnitOfWork;
        private readonly IMapper _Mapper;
        private readonly ISession _Session;

        public EventAgendaController(UnitOfWork UnitOfWork, IMapper Mapper, IHttpContextAccessor HttpContextAccessor)
        {
            _UnitOfWork = UnitOfWork;
            _Mapper = Mapper;
            _Session = HttpContextAccessor.HttpContext.Session;
        }

        [Authorize((int)AccessLevelEnum.ReadAccess)]
        public IActionResult Index(int Id, int Fk_Event, bool ProfileLayOut = false)
        {
            EventAgendaFilter Filter = new EventAgendaFilter
            {
                Id = Id,
                Fk_Event = Fk_Event,
            };
            ViewData["ProfileLayOut"] = ProfileLayOut;
            return View("~/Views/EventEntity/EventAgenda/Index.cshtml", Filter);
        }

        [HttpPost]
        [Authorize((int)AccessLevelEnum.ReadAccess)]
        public async Task<IActionResult> LoadTable([FromBody] EventAgendaFilter dtParameters)
        {
            string searchBy = dtParameters.Search?.Value;

            List<EventAgenda> result = await _UnitOfWork.EventAgenda.GetAll(a => (dtParameters.Id == 0 || a.Id == dtParameters.Id)
                                                                                         && (dtParameters.Fk_Event == 0 || a.Fk_Event == dtParameters.Fk_Event));


            if (!string.IsNullOrEmpty(searchBy))
            {
                result = result.Where(a => a.Title.ToLower().Contains(searchBy.ToLower())
                                        || a.FromDate.ToString().ToLower().Contains(searchBy.ToLower())
                                        || a.FromTime.ToString().ToLower().Contains(searchBy.ToLower())
                                        || a.IsActive.ToString().ToLower().Contains(searchBy.ToLower())
                                        || a.Id.ToString().ToLower().Contains(searchBy.ToLower()))
                               .ToList();
            }

            DataTableManager<EventAgenda> DataTableManager = new DataTableManager<EventAgenda>();

            DataTableResult<EventAgenda> DataTableResult = DataTableManager.LoadTable(dtParameters, result, _UnitOfWork.EventAgenda.Count());

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
            EventAgenda EventAgenda = await _UnitOfWork.EventAgenda.GetByID(id);

            if (EventAgenda == null)
            {
                return NotFound();
            }

            return View("~/Views/EventEntity/EventAgenda/Details.cshtml", EventAgenda);
        }

        [Authorize((int)AccessLevelEnum.CreateOrUpdateAccess)]
        public async Task<IActionResult> CreateOrEdit(int id = 0, int Fk_Event = 0)
        {
            EventAgenda EventAgenda = new EventAgenda();

            if (id == 0 && !_UnitOfWork.Event.Any(a => a.Id == Fk_Event))
            {
                return NotFound();
            }

            if (id > 0)
            {
                EventAgenda = await _UnitOfWork.EventAgenda.GetByID(id);
                if (EventAgenda == null)
                {
                    return NotFound();
                }
            }
            else
            {
                EventAgenda.Fk_Event = Fk_Event;
            }

            return View("~/Views/EventEntity/EventAgenda/CreateOrEdit.cshtml", EventAgenda);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize((int)AccessLevelEnum.CreateOrUpdateAccess)]
        public async Task<IActionResult> CreateOrEdit(int id, EventAgenda EventAgenda)
        {
            if (id != EventAgenda.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (id == 0)
                    {
                        EventAgenda.CreatedBy = _Session.GetString("FullName");
                        _UnitOfWork.EventAgenda.CreateEntity(EventAgenda);
                        await _UnitOfWork.EventAgenda.Save();
                    }
                    else
                    {
                        EventAgenda Data = await _UnitOfWork.EventAgenda.GetByID(id);

                        EventAgenda.LastModifiedBy = _Session.GetString("FullName");

                        _Mapper.Map(EventAgenda, Data);


                        _UnitOfWork.EventAgenda.UpdateEntity(Data);
                        await _UnitOfWork.EventAgenda.Save();
                    }
                }

                catch (DbUpdateConcurrencyException)
                {
                    if (!_UnitOfWork.EventAgenda.Any(a => a.Id == id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Profile", "Event", new { id = EventAgenda.Fk_Event, returnItem = (int)EventProfileItems.EventAgenda });
            }

            return View("~/Views/EventEntity/EventAgenda/CreateOrEdit.cshtml", EventAgenda);

        }

        [Authorize((int)AccessLevelEnum.FullAccess)]
        public async Task<IActionResult> Delete(int id)
        {
            EventAgenda EventAgenda = await _UnitOfWork.EventAgenda.GetByID(id);

            if (EventAgenda == null)
            {
                return NotFound();
            }

            return View("~/Views/EventEntity/EventAgenda/Delete.cshtml", EventAgenda);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize((int)AccessLevelEnum.FullAccess)]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            EventAgenda EventAgenda = await _UnitOfWork.EventAgenda.GetByID(id);

            _UnitOfWork.EventAgenda.DeleteEntity(EventAgenda);
            await _UnitOfWork.EventAgenda.Save();

            return RedirectToAction("Profile", "Event", new { id = EventAgenda.Fk_Event, returnItem = (int)EventProfileItems.EventAgenda });
        }
    }
}
