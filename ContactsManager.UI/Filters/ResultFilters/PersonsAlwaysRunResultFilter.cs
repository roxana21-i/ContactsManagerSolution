using Microsoft.AspNetCore.Mvc.Filters;

namespace CRUD_Application.Filters.ResultFilters
{
	public class PersonsAlwaysRunResultFilter : IAlwaysRunResultFilter
	{
		public void OnResultExecuted(ResultExecutedContext context)
		{
			
		}

		public void OnResultExecuting(ResultExecutingContext context)
		{
			if(context.Filters.OfType<SkipFilter>().Any())
			{
				return;
			}
			context.HttpContext.Response.Headers["Always-Run-Filter"] = "Executed";
		}
	}
}
