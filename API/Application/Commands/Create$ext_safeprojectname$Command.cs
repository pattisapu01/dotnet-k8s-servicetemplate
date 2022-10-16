using MediatR;
using Cloud.$ext_safeprojectname$.API.Application.DTO;

namespace Cloud.$ext_safeprojectname$.API.Application.Commands
{
    /// <summary>
    /// Marker class that inherits from DTO so that the types can be re-used
    /// </summary>
    public class Create$ext_safeprojectname$Command : Application.DTO.$ext_safeprojectname$, IRequest<Application.DTO.$ext_safeprojectname$>
    {
    }
}
