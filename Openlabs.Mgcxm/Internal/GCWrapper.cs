using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Openlabs.Mgcxm.Internal
{
    /// <summary>
    /// Provides utility methods for interacting with the Garbage Collector.
    /// </summary>
    public static class GCWrapper
    {
        /// <summary>
        /// Forces an immediate garbage collection of all generations.
        /// </summary>
        public static void Collect() => GC.Collect();

        /// <summary>
        /// Forces a garbage collection of objects in a specified generation, using the default collection mode.
        /// </summary>
        /// <param name="generation">The generation to be collected.</param>
        public static void Collect(int generation) => Collect(generation, GCCollectionMode.Default, false);

        /// <summary>
        /// Forces a garbage collection of objects in a specified generation, using the specified collection mode.
        /// </summary>
        /// <param name="generation">The generation to be collected.</param>
        /// <param name="mode">The collection mode to use.</param>
        public static void Collect(int generation, GCCollectionMode mode) => Collect(generation, mode, false);

        /// <summary>
        /// Forces a garbage collection of objects in a specified generation, using the specified collection mode and optional blocking behavior.
        /// </summary>
        /// <param name="generation">The generation to be collected.</param>
        /// <param name="mode">The collection mode to use.</param>
        /// <param name="blocking">A value indicating whether the collection should block the calling thread until the collection finishes.</param>
        public static void Collect(int generation, GCCollectionMode mode, bool blocking) => GC.Collect(generation, mode, blocking);

        /// <summary>
        /// Requests that the runtime does not call the finalizer for the specified object.
        /// </summary>
        /// <param name="obj">The object for which to suppress the finalizer.</param>
        public static void SuppressFinalize(in object obj)
        {
            if (obj != null) GC.SuppressFinalize(obj);
        }

        /// <summary>
        /// Requests that the runtime re-enables the finalizer for the specified object.
        /// </summary>
        /// <param name="obj">The object for which to re-register the finalizer.</param>
        public static void ReRegisterForFinalize(in object obj)
        {
            if (obj != null) GC.ReRegisterForFinalize(obj);
        }

        /// <summary>
        /// Gets the total number of bytes allocated in the managed heap thought to be collected.
        /// </summary>
        /// <returns>The best approximation for the total number of bytes allocated.</returns>
        public static long GetTotalMemory()
        {
            return GC.GetTotalMemory(false);
        }

        /// <summary>
        /// Gets the highest generation that the garbage collector currently supports.
        /// </summary>
        public static int MaxGeneration => GC.MaxGeneration;
    }
}
