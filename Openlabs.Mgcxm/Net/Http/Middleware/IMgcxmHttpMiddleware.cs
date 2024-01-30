using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Openlabs.Mgcxm.Net.Http.Middleware
{
    /// <summary>
    /// Represents the result of a middleware execution.
    /// </summary>
    public enum MiddlewareResult
    {
        /// <summary>
        /// Indicates that the middleware processing should proceed to the next step.
        /// </summary>
        PROCEED,

        /// <summary>
        /// Indicates that the middleware processing should stop, and the request should not proceed further.
        /// </summary>
        DONT_PROCEED,

        /// <summary>
        /// Indicates an error occurred during middleware processing.
        /// </summary>
        ERROR
    }

    /// <summary>
    /// Represents an interface for defining HTTP middlewares.
    /// </summary>
    public interface IMgcxmHttpMiddleware
    {
        /// <summary>
        /// Called when an HTTP request is made, allowing the middleware to process the request and response.
        /// </summary>
        /// <param name="request">The HTTP request object.</param>
        /// <param name="response">The HTTP response object.</param>
        /// <returns>The result of the middleware processing.</returns>
        MiddlewareResult OnRequestMade(MgcxmHttpRequest request, MgcxmHttpResponse response);
    }
}
