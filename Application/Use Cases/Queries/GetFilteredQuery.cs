using Application.Utils;
using Domain.Common;
using MediatR;
using System.Linq.Expressions;

namespace Application.Use_Cases.Queries
{
    public class GetFilteredQuery<T, TDTO> : IRequest<Result<PagedResult<TDTO>>>
    {
        public int Page { get; set; }
        public int PageSize { get; set; } 
        public Expression<Func<T, bool>>? Filter { get; set; }

    }
}
