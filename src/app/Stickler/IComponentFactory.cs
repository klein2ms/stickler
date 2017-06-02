using System;

namespace Stickler
{
    internal interface IComponentFactory
    {
        dynamic Create<TComponent>() where TComponent : class;
    }
}
