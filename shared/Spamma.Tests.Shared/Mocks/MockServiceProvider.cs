using System.Collections;
using MaybeMonad;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace Spamma.Shared.Tests.Mocks
{
    public class MockServiceProvider : Mock<IServiceProvider>
    {
        private IServiceCollection _serviceCollection = new ServiceCollection();

        public MockServiceProvider()
        {
            this.Setup(x => x.GetService(It.IsAny<Type>()))
                .Returns((Type t) =>
                {
                    if (IsEnumerable(t))
                    {
                        var list = this.DiscoverServices(t.GenericTypeArguments[0], this.Object);
                        if (list.Count <= 0)
                        {
                            return null;
                        }

                        var listType = typeof(List<>);
                        var constructedListType = listType.MakeGenericType(t.GenericTypeArguments[0]);
                        var instance = Activator.CreateInstance(constructedListType);
                        if (instance == null)
                        {
                            return list;
                        }

                        var addMethod = instance.GetType().GetMethod("Add");
                        if (addMethod == null)
                        {
                            return list;
                        }

                        foreach (var item in list)
                        {
                            addMethod.Invoke(instance, new[] { item });
                        }

                        return instance;
                    }
                    else
                    {
                        var result = this.DiscoverService(t, this.Object);
                        if (result.HasValue)
                        {
                            return result.Value;
                        }
                    }

                    return null;
                });
        }

        public void SetServiceCollection(IServiceCollection serviceCollection)
        {
            this.ProcessServiceCollection(serviceCollection);
        }

        private static bool IsEnumerable(Type type)
        {
            return type.GetInterface(nameof(IEnumerable)) != null;
        }

        private Maybe<object> DiscoverService(Type serviceType, IServiceProvider serviceProvider)
        {
            foreach (var serviceDescriptor in this._serviceCollection)
            {
                if (serviceDescriptor.ServiceType != serviceType)
                {
                    continue;
                }

                if (serviceDescriptor.ImplementationFactory != null)
                {
                    return serviceDescriptor.ImplementationFactory(serviceProvider);
                }
                else if (serviceDescriptor.ImplementationType != null)
                {
                    var instance = Activator.CreateInstance(serviceDescriptor.ImplementationType);
                    if (instance != null)
                    {
                        return instance;
                    }
                }
                else if (serviceDescriptor.ImplementationInstance != null)
                {
                    return serviceDescriptor.ImplementationInstance;
                }
            }

            return Maybe<object>.Nothing;
        }

        private List<object> DiscoverServices(Type serviceType, IServiceProvider serviceProvider)
        {
            var list = new List<object>();
            foreach (var serviceDescriptor in this._serviceCollection)
            {
                if (serviceDescriptor.ServiceType != serviceType)
                {
                    continue;
                }

                if (serviceDescriptor.ImplementationFactory != null)
                {
                    list.Add(serviceDescriptor.ImplementationFactory(serviceProvider));
                }
                else if (serviceDescriptor.ImplementationType != null)
                {
                    var instance = Activator.CreateInstance(serviceDescriptor.ImplementationType);
                    if (instance != null)
                    {
                        list.Add(instance);
                    }
                }
                else if (serviceDescriptor.ImplementationInstance != null)
                {
                    list.Add(serviceDescriptor.ImplementationInstance);
                }
            }

            return list;
        }

        private void ProcessServiceCollection(IServiceCollection serviceCollection)
        {
            var serviceScopeFactory = new Mock<IServiceScopeFactory>();
            var serviceScope = new Mock<IServiceScope>();
            serviceScope.Setup(x => x.ServiceProvider)
                .Returns(this.Object);
            serviceScopeFactory.Setup(x => x.CreateScope())
                .Returns(serviceScope.Object);
            serviceCollection.AddScoped<IServiceScopeFactory>((_) => serviceScopeFactory.Object);
            this._serviceCollection = serviceCollection;
        }
    }
}