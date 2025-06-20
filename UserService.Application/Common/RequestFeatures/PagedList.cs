using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace UserService.Application.Common.RequestFeatures;

public class PagedList<T> 
{
	public List<T> Items { get; } = [];
	public MetaData? MetaData { get; set; }

	public PagedList(List<T> items, int count, int pageNumber, int pageSize)
	{
		Items = items;
		MetaData = new MetaData
		{
			TotalCount = count,
			PageSize = pageSize,
			CurrentPage = pageNumber,
		};
	}

	[JsonConstructor]
	public PagedList(List<T> items, MetaData metaData)
	{
		Items = items;
		MetaData = metaData;
	}

	protected PagedList() { }

	public static PagedList<T> ToPagedList(IEnumerable<T> source, int pageNumber, int pageSize)
	{
		var count = source.Count();
		var items = source
			.Skip((pageNumber - 1) * pageSize)
			.Take(pageSize).ToList();
		return new PagedList<T>(items, count, pageNumber, pageSize);
	}

	public static async Task<PagedList<T>> ToPagedListAsync(IQueryable<T> source, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
	{
		ArgumentNullException.ThrowIfNull(source);

		var count = await source.CountAsync(cancellationToken);
		var items = await source
			.Skip((pageNumber - 1) * pageSize)
			.Take(pageSize)
			.ToListAsync(cancellationToken);

		return new PagedList<T>(items, count, pageNumber, pageSize);
	}
}

