using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Stickler.Engine;

namespace Stickler
{
    internal sealed class ComponentFactory : IComponentFactory
    {
        private static readonly Lazy<ComponentFactory>  _lazy
            = new Lazy<ComponentFactory>(() => new ComponentFactory());

        internal static ComponentFactory Instance => _lazy.Value;

        private readonly ConcurrentBag<Type> _exportedTypes;
        private readonly ConcurrentDictionary<Type, Type> _registeredComponentTypes; 

        private ComponentFactory()
        {
            _exportedTypes = new ConcurrentBag<Type>(
                AppDomain
                    .CurrentDomain
                    .GetAssemblies()
                    .SelectMany(a => a.ExportedTypes));

            _registeredComponentTypes = new ConcurrentDictionary<Type, Type>();
        }

        public dynamic Create<TComponent>()
            where TComponent : class
        {
            if (typeof (TComponent) != typeof (ILexer)
                && typeof (TComponent) != typeof (IParser)
                && typeof (TComponent) != typeof (IInterpreter)
                && typeof (TComponent) != typeof (IEvaluator)
                && typeof (TComponent) != typeof (ICompiledRuleStore)
                && typeof (TComponent) != typeof (IRuleStore)
                && typeof (TComponent) != typeof (IRuleMediator)
                && typeof (TComponent) != typeof (IResultAggregator))
                throw new ArgumentOutOfRangeException(typeof(TComponent).FullName);

                return Create(typeof(TComponent));
        }

        private dynamic Create(Type type)
        {
            var typeToCreate = GetRegisteredType(type);

            object[] constructorParameters = GetConstructorParameters(typeToCreate);

            if (constructorParameters.Any())
                return Activator.CreateInstance(
                    typeToCreate,
                    constructorParameters);

            return Activator.CreateInstance(typeToCreate);
        } 
        
        private dynamic[] GetConstructorParameters(Type type)
        {
            return GetConstructor(type)
                .GetParameters()
                .Select(parameter => Create(parameter.ParameterType)).ToArray();
        }

        private static ConstructorInfo GetConstructor(Type type)
        {
            var constructors = type.GetConstructors();
            return constructors
                .Where(
                    c =>
                        !c.GetParameters().Any() ||
                        c.GetParameters().All(p => p.ParameterType.IsAssignableFrom(typeof(ILexer))
                                                   || p.ParameterType.IsAssignableFrom(typeof(IParser))
                                                   || p.ParameterType.IsAssignableFrom(typeof(IInterpreter))
                                                   || p.ParameterType.IsAssignableFrom(typeof(IEvaluator))
                                                   || p.ParameterType.IsAssignableFrom(typeof(IRuleStore))
                                                   || p.ParameterType.IsAssignableFrom(typeof(ICompiledRuleStore))
                                                   || p.ParameterType.IsAssignableFrom(typeof(IRuleMediator))
                                                   || p.ParameterType.IsAssignableFrom(typeof(IResultAggregator))))
                .OrderByDescending(c => c.GetParameters().Length)
                .First();
        }

        private Type GetRegisteredType(Type type)
        {
            Type typeToReturn;
            _registeredComponentTypes.TryGetValue(type, out typeToReturn);

            if (typeToReturn != null)
                return typeToReturn;

            var implementedTypes = GetImplementedTypes(type, _exportedTypes).ToArray();

            var clientImplementedTypes =
                implementedTypes.Where(
                    t => !t.FullName.StartsWith("Stickler.Engine.", StringComparison.CurrentCultureIgnoreCase))
                    .ToArray();

            if (!clientImplementedTypes.Any())
                return implementedTypes.FirstOrDefault();
            
            if (clientImplementedTypes.Count() > 1)
                return null;

            typeToReturn = clientImplementedTypes.FirstOrDefault();

            if (typeToReturn == null)
                return null;

            if (!typeToReturn.IsClass)
                return null;

            if (typeToReturn.IsGenericType)
                return null;

            RegisterType(type, typeToReturn);

            return typeToReturn;
        }

        private void RegisterType(Type componentType, Type registeredType)
        {
            if (registeredType.GetInterfaces().Contains(componentType))
                _registeredComponentTypes.TryAdd(componentType, registeredType);
        }
        
        private static IEnumerable<Type> GetImplementedTypes(Type type, IEnumerable<Type> exportedTypes)
        {
            var implementedTypes = new List<Type>();

            implementedTypes.AddRange(exportedTypes
                .Where(t => t.Name.EndsWith(type.Name.ToString().TrimStart('I'), StringComparison.CurrentCultureIgnoreCase))
                .Where(t => t.GetInterfaces().Contains(type)));

            return implementedTypes;
        }
    }
}
