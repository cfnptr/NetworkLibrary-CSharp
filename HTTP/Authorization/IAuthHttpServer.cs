using InjectorGames.SharedLibrary.Credentials.Accounts;
using System;

namespace InjectorGames.NetworkLibrary.HTTP.Authorization
{
    /// <summary>
    /// Authorization HTTP server interface
    /// </summary>
    public interface IAuthHttpServer<TAccount, TAccountFactory> : IHttpServer
        where TAccount : IAccount
        where TAccountFactory : IAccountFactory<TAccount>
    {
        /// <summary>
        /// Server version
        /// </summary>
        Version Version { get; }
        /// <summary>
        /// Account factory
        /// </summary>
        TAccountFactory AccountFactory { get; }
        /// <summary>
        /// Account database
        /// </summary>
        IAccountDatabase<TAccount, TAccountFactory> AccountDatabase { get; }
    }
}
