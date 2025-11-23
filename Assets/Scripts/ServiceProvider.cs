using System;
using System.Collections.Generic;

namespace Core
{
    public static class ServiceProvider
    {
        private static readonly Dictionary<Type, IProvidable> RegisterDictionary = new();

        public static T GetManager<T>() where T : class, IProvidable
        {
            if (RegisterDictionary.ContainsKey(typeof(T))) return (T)RegisterDictionary[typeof(T)];

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

        public static ItemFallSystem GetItemFallSystem => GetManager<ItemFallSystem>();

        public static CubeSpawner GetCubeSpawner => GetManager<CubeSpawner>();

        public static GameConfig GetGameConfig => GetManager<GameConfig>();
    }
}