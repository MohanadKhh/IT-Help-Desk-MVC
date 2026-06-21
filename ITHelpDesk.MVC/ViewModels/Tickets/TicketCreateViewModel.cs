using ITHelpDesk.Application.DTOs.Tickets;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ITHelpDesk.MVC.ViewModels.Tickets
{
    public class TicketCreateViewModel
    {
        public CreateTicketDto Input { get; set; }
        public List<SelectListItem> Categories { get; set; } = new();
        public List<SelectListItem> AssignableUsers { get; set; } = new();
    }
}
