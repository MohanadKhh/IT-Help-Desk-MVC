using ITHelpDesk.Application.Common.Helpers;
using ITHelpDesk.Application.DTOs.TicketComments;
using ITHelpDesk.Application.Interfaces.Identity;
using ITHelpDesk.Application.Interfaces.Services;
using ITHelpDesk.MVC.Helpers;
using ITHelpDesk.MVC.ViewModels.Tickets;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ITHelpDesk.MVC.Controllers
{
    public class TicketsController : Controller
    {
        private readonly ITicketService _ticketService;
        private readonly IUserService _userService;
        private readonly ICategoryService _categoryService;
        private readonly ICommentService _commentService;

        public TicketsController(ITicketService ticketService, IUserService userService, ICategoryService categoryService, ICommentService commentService)
        {
            _ticketService = ticketService;
            _userService = userService;
            _categoryService = categoryService;
            _commentService = commentService;
        }

        // GET /Tickets
        [HttpGet]
        [Authorize(Policy = "UserOrAdmin")]
        public async Task<IActionResult> Index()
        {
            var result = await _ticketService.GetAllTicketsAsync();
            return View(result.Data);
        }

        // GET /Tickets/Details/5
        [HttpGet("{id:int}")]
        [Authorize(Policy = "UserOrAdmin")]
        public async Task<IActionResult> Details(int id)
        {
            var result = await _ticketService.GetTicketByIdAsync(id);
            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                return RedirectToAction(nameof(Index));
            }
            return View(result.Data);
        }

        // POST /Tickets/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "UserOrAdmin")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _ticketService.DeleteTicketAsync(id);

            TempData[result.Success ? "Success" : "Error"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        // GET /Tickets/Create
        [HttpGet]
        [Authorize(Policy = "UserOrAdmin")]
        public async Task<IActionResult> Create()
        {
            var users = await _userService.GetAllUsersDropdownAsync();
            var categories = await _categoryService.GetCategoriesDropdownAsync();
            var vm = new TicketCreateViewModel
            {
                Categories = categories.Select(u => new SelectListItem
                {
                    Value = u.Id.ToString(),
                    Text = u.DisplayName
                }).ToList(),
                AssignableUsers = users.Select(u => new SelectListItem
                {
                    Value = u.Id.ToString(),
                    Text = u.DisplayName
                }).ToList()
            };
            return View(vm);
        }

        // POST /Tickets/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Policy = "UserOrAdmin")]
        public async Task<IActionResult> Create(TicketCreateViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdownsAsync(vm);
                return View(vm);
            }

            var result = await _ticketService.CreateTicketAsync(vm.Input);
            if (!result.Success)
            {
                ModelState.AddValidationErrors(result.Errors, "Input");
                ModelState.AddModelError(string.Empty, result.Message);
                await PopulateDropdownsAsync(vm);
                return View(vm);
            }

            TempData["Success"] = result.Message;
            return RedirectToAction(nameof(Index));
        }

        // GET /Tickets/Edit/5
        [HttpGet]
        [Authorize(Policy = "UserOrAdmin")]
        public async Task<IActionResult> Edit(int id)
        {
            var result = await _ticketService.GetTicketForEditAsync(id);
            if (!result.Success)
            {
                TempData["Error"] = result.Message;
                return RedirectToAction(nameof(Details), new { id });
            }

            var users = await _userService.GetAllUsersDropdownAsync();
            var categories = await _categoryService.GetCategoriesDropdownAsync();

            var vm = new TicketEditViewModel
            {
                Input = result.Data with
                {
                    DueDate = DateHelper.ToCairoDate(result.Data.DueDate)
                },
                Categories = categories.Select(u => new SelectListItem
                {
                    Value = u.Id.ToString(),
                    Text = u.DisplayName
                }).ToList(),
                AssignableUsers = users.Select(u => new SelectListItem
                {
                    Value = u.Id.ToString(),
                    Text = u.DisplayName
                }).ToList()
            };

            return View(vm);
        }

        // POST /Tickets/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TicketEditViewModel vm)
        {
            if (!ModelState.IsValid)
            {
                await PopulateDropdownsAsync(vm);
                return View(vm);
            }

            var result = await _ticketService.UpdateTicketAsync(vm.Input);
            if (!result.Success)
            {
                ModelState.AddValidationErrors(result.Errors, "Input");
                ModelState.AddModelError(string.Empty, result.Message);
                await PopulateDropdownsAsync(vm);
                return View(vm);
            }

            TempData["Success"] = result.Message;
            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpPost]
        public async Task<IActionResult> AddComment(CreateTicketCommentDto dto)
        {
            var result = await _commentService.CreateCommentAsync(dto);

            if (!result.Success)
            {
                TempData["Error"] = result.Message;
            }
            else
            {
                TempData["Success"] = result.Message;
            }
            return RedirectToAction(nameof(Details), new { id = dto.TicketId });
        }

        [HttpPost]
        public async Task<IActionResult> EditComment(EditTicketCommentDto dto)
        {
            var result = await _commentService.EditCommentAsync(dto);
            if (!result.Success)
            {
                TempData["Error"] = result.Message;
            }
            else
            {
                TempData["Success"] = result.Message;
            }
            return RedirectToAction(nameof(Details), new { id = result.Data });
        }

        [HttpPost]
        public async Task<IActionResult> DeleteComment(int commentId, int ticketId)
        {
            var result = await _commentService.DeleteCommentAsync(commentId);
            if (!result.Success)
            {
                TempData["Error"] = result.Message;
            }
            else
            {
                TempData["Success"] = result.Message;
            }
            return RedirectToAction(nameof(Details), new { id = ticketId });
        }


        // ── Private Helpers ──────────────────────────────────────
        private async Task PopulateDropdownsAsync(TicketCreateViewModel vm)
        {
            var users = await _userService.GetAllUsersDropdownAsync();
            var categories = await _categoryService.GetCategoriesDropdownAsync();

            vm.Categories = categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.DisplayName
            }).ToList();

            vm.AssignableUsers = users.Select(u => new SelectListItem
            {
                Value = u.Id.ToString(),
                Text = u.DisplayName
            }).ToList();
        }

        private async Task PopulateDropdownsAsync(TicketEditViewModel vm)
        {
            var users = await _userService.GetAllUsersDropdownAsync();
            var categories = await _categoryService.GetCategoriesDropdownAsync();

            vm.Categories = categories.Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.DisplayName
            }).ToList();

            vm.AssignableUsers = users.Select(u => new SelectListItem
            {
                Value = u.Id.ToString(),
                Text = u.DisplayName
            }).ToList();
        }
    }
}