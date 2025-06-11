using UserService.Domain.Common;

namespace UserService.Application.Common.Interfaces;

public interface IDataShapeService<T>
{
	IEnumerable<ShapedEntity> ShapeData(IEnumerable<T> entities, string fieldsString);
	ShapedEntity ShapeData(T entity, string fieldsString);
}
