using System.Reflection;
using Spamma.Api.Web.Infrastructure.Contracts.Domain;

namespace Spamma.Shared.Tests
{
    public static class EntityHelpers
    {
        public static TEntity CreateEntity<TEntity>()
            where TEntity : Entity
        {
            return CreateEntity<TEntity>(new { });
        }

        public static TEntity CreateEntity<TEntity>(object props)
            where TEntity : Entity
        {
            var type = typeof(TEntity);
            var entity = Activator.CreateInstance(type, true);

            foreach (var prop in props.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public))
            {
                if (prop.PropertyType.Name == "List`1")
                {
                    type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                        .Single(x => x.Name.ToLower().Contains(prop.Name.ToLower()))
                        .SetValue(entity, prop.GetValue(props));
                }
                else
                {
                    var propertyInfo = type.GetProperties(BindingFlags.Instance | BindingFlags.Public)
                        .SingleOrDefault(propInfo => string.Equals(propInfo.Name, prop.Name,
                            StringComparison.CurrentCultureIgnoreCase));
                    if (propertyInfo != null && propertyInfo.CanWrite)
                    {
                        propertyInfo.SetValue(entity, prop.GetValue(props));
                    }
                    else
                    {
                        type.GetFields(BindingFlags.Instance | BindingFlags.NonPublic)
                            .Single(x => x.Name.ToLower().Contains(prop.Name.ToLower()))
                            .SetValue(entity, prop.GetValue(props));
                    }
                }
            }

            return (TEntity)entity!;
        }

        public static IEnumerable<KeyValuePair<string, bool>> CheckLists<TEntity>(TEntity entity)
            where TEntity : Entity
        {
            foreach (var prop in entity.GetType().GetProperties().Where(x => x.PropertyType.Name == "IReadOnlyList`1"))
            {
                var val = prop.GetValue(entity, null);
                yield return new KeyValuePair<string, bool>(prop.Name, val != null);
            }
        }
    }
}