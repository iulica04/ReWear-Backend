using Application.Models;

namespace Application.Services
{
    public interface IOLXServices
    {
        Task<bool> PostAnuntAsync(AnuntModel anunt);
    }
}
