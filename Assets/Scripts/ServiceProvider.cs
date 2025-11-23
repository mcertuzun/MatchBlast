using System;
using System.Collections.Generic;
namespace Core
{
    public static class ServiceProvider
    {
        private static readonly Dictionary<Type, IProvidable> RegisterDictionary = new Dictionary<Type, IProvidable>();

        public static T GetManager<T>() where T : class, IProvidable
        {
            if (RegisterDictionary.ContainsKey(typeof(T)))
            {
                return (T) RegisterDictionary[typeof(T)];
            }

            return null;
        }

        public static T Register<T>(T target) where T : class, IProvidable
        {
            RegisterDictionary.Add(typeof(T), target);
            return target;
        }
        
        /*
        public static ParticleManager GetParticleManager
        {
            get { return GetManager<ParticleManager>(); }
        }*/

        public static ItemFallSystem GetItemFallSystem
        {
            get { return GetManager<ItemFallSystem>(); }
        }

        public static CubeSpawner GetCubeSpawner
        {
            get { return GetManager<CubeSpawner>(); }
        }

        public static Core.GameConfig GetGameConfig
        {
            get { return GetManager<Core.GameConfig>(); }
        }


    }
}