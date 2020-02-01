using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using JetBrains.Annotations;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using SkriptInsight.Core.Utils;

namespace SkriptInsight.Core
{
    /// <summary>
    /// Represents a SkriptInsight Host Editor
    /// </summary>
    public interface ISkriptInsightHost
    {
        /// <summary>
        /// Log a message to the host
        /// </summary>
        /// <param name="message">The message to be logged</param>
        void LogInfo(string message);

        /// <summary>
        /// Publish diagnostics to this host
        /// </summary>
        /// <param name="url">The URL of this file</param>
        /// <param name="diagnostics">The diagnostics to show</param>
        void PublishDiagnostics(Uri url, List<Diagnostic> diagnostics);
     
        /// <summary>
        /// Whether this host support raw LSP requests
        /// </summary>
        bool SupportsRawRequests { get; }
        
        
        /// <summary>
        /// Send raw notification to LSP host
        /// </summary>
        /// <param name="name">The name of the method for this notification</param>
        void SendRawNotification(string name);
        
        /// <summary>
        /// Send raw notification with parameters to LSP host 
        /// </summary>
        /// <param name="name">The name of the method for this notification</param>
        /// <param name="params">The parameter object for this notification</param>
        /// <typeparam name="T">The type of the parameter</typeparam>
        void SendRawNotification<T>(string name, T @params);
        
        /// <summary>
        /// Send a raw request with parameters
        /// </summary>
        /// <param name="method">The name of the method for this request</param>
        /// <param name="params">The parameter object for this notification</param>
        /// <typeparam name="T">The type of the request parameters</typeparam>
        /// <typeparam name="TResponse">The type of the request response</typeparam>
        /// <returns>The response given from the LSP host</returns>
        Task<TResponse> SendRawRequest<T, TResponse>(string method, T @params);
        
        /// <summary>
        /// Send a raw request with parameters
        /// </summary>
        /// <param name="method">The name of the method for this request</param>
        /// <typeparam name="TResponse">The type of the request response</typeparam>
        /// <returns>The response given from the LSP host</returns>
        Task<TResponse> SendRawRequest<TResponse>(string method);

        /// <summary>
        /// Whether this host supports extended capabilities
        /// </summary>
        public bool SupportsExtendedCapabilities { get; }
        
        /// <summary>
        /// The extended capabilities of this Host
        /// </summary>
        [CanBeNull]
        ExtendedHostCapabilities ExtendedCapabilities { get; set; }
    }
}