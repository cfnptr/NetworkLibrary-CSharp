
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

namespace QuantumBranch.OpenNetworkLibrary
{
    /// <summary>
    /// Logger class interface
    /// </summary>
    public interface ILogger
    {
        /// <summary>
        /// Logger logging level
        /// </summary>
        LogType Level { get; set; }

        /// <summary>
        /// Returns true if logger should log this level
        /// </summary>
        bool Log(LogType level);

        /// <summary>
        /// Logs a new message at fatal log level
        /// </summary>
        void Fatal(object message);
        /// <summary>
        /// Logs a new message at error log level
        /// </summary>
        void Error(object message);
        /// <summary>
        /// Logs a new message at info log level
        /// </summary>
        void Info(object message);
        /// <summary>
        /// Logs a new message at fatal log level
        /// </summary>
        void Debug(object message);
        /// <summary>
        /// Logs a new message at fatal log level
        /// </summary>
        void Trace(object message);

        /// <summary>
        /// Logs a new message with the prefix at fatal log level
        /// </summary>
        void Fatal(object message, object prefix);
        /// <summary>
        /// Logs a new message with the prefix at error log level
        /// </summary>
        void Error(object message, object prefix);
        /// <summary>
        /// Logs a new message with the prefix at info log level
        /// </summary>
        void Info(object message, object prefix);
        /// <summary>
        /// Logs a new message with the prefix at fatal log level
        /// </summary>
        void Debug(object message, object prefix);
        /// <summary>
        /// Logs a new message with the prefix at fatal log level
        /// </summary>
        void Trace(object message, object prefix);
    }
}
