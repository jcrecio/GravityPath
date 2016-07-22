namespace GravityPath.Services
{
    using System;
    using System.Collections.Generic;

    public class GeneralContainer
    {
        private static GeneralContainer _generalContainer;

        private IDictionary<Type, object> container;

        public static GeneralContainer GetInstance()
        {
            return _generalContainer ?? (_generalContainer = new GeneralContainer());
        }

        private GeneralContainer()
        {
            this.container = new Dictionary<Type, object>();
        }

        public void Register<T>(object implementation)
        {
            if (!this.container.ContainsKey(typeof(T)))
            {
                this.container.Add(typeof(T), implementation);
            }
        }

        public T GetServiceInstance<T>()
        {
            return (T)this.container[typeof(T)];
        }
    }
}
