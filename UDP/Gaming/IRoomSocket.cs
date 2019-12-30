
// Copyright 2019 Nikita Fediuchin (QuantumBranch)
//
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//
//     http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using OpenSharedLibrary.Gaming.Players;
using OpenSharedLibrary.Gaming.Rooms;
using OpenSharedLibrary.Timing;

namespace OpenNetworkLibrary.UDP.Gaming
{
    /// <summary>
    /// Room UDP socket interface
    /// </summary>
    public interface IRoomSocket<TPlayer, TPlayerFactory> : ITaskedSocket, IRoom
        where TPlayer : IPlayer
        where TPlayerFactory : IPlayerFactory<TPlayer>
    {
        /// <summary>
        /// Room clock
        /// </summary>
        IClock Clock { get; }
        /// <summary>
        /// Player factory
        /// </summary>
        TPlayerFactory PlayerFactory { get; }
        /// <summary>
        /// Player database
        /// </summary>
        IPlayerDatabase<TPlayer, TPlayerFactory> PlayerDatabase { get; }
        /// <summary>
        /// Player concurrent dictionary
        /// </summary>
        PlayerDictionary<TPlayer> Players { get; }
    }
}
