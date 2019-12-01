
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

using System.Threading;

namespace OpenNetworkLibrary
{
    /// <summary>
    /// Time tick handler class
    /// </summary>
    public class TimeTickHandler : ITimeTickHandler
    {
        /// <summary>
        /// Time tick update thread instance
        /// </summary>
        protected readonly Thread thread;

        /// <summary>
        /// Time tick value
        /// </summary>
        protected long tick;

        /// <summary>
        /// Time tick value
        /// </summary>
        public long Tick
        {
            get { return tick; }
        }

        /// <summary>
        /// Creates a new time tick handler class instance
        /// </summary>
        public TimeTickHandler()
        {
            tick = 0;
            thread = new Thread(TimeTickThreadLogic);
            thread.Start();
        }
        /// <summary>
        /// Creates a new time tick handler class instance
        /// </summary>
        public TimeTickHandler(long value)
        {
            tick = value;
            thread = new Thread(TimeTickThreadLogic);
            thread.Start();
        }

        /// <summary>
        /// Time tick handler destructor
        /// </summary>
        ~TimeTickHandler()
        {
            thread.Abort();
        }

        /// <summary>
        /// Time tick thread delegate method
        /// </summary>
        protected void TimeTickThreadLogic()
        {
            while (true)
            {
                tick++;
                Thread.Sleep(1);
            }
        }
    }
}
