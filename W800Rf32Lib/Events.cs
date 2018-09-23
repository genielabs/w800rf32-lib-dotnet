/*
  This file is part of W800Rf32Lib (https://github.com/genielabs/w800rf32-lib-dotnet)

  Copyright (2012-2018) G-Labs (https://github.com/genielabs)

  Licensed under the Apache License, Version 2.0 (the "License");
  you may not use this file except in compliance with the License.
  You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

  Unless required by applicable law or agreed to in writing, software
  distributed under the License is distributed on an "AS IS" BASIS,
  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
  See the License for the specific language governing permissions and
  limitations under the License.
*/

/*
 *     Author: Generoso Martello <gene@homegenie.it>
 *     Project Homepage: https://github.com/genielabs/w800rf32-lib-dotnet
 */

using System;

namespace W800Rf32Lib
{

    /// <summary>
    /// Connected state changed event arguments.
    /// </summary>
    public class ConnectionStatusChangedEventArgs
    {
        /// <summary>
        /// The connected state.
        /// </summary>
        public readonly bool Connected;

        /// <summary>
        /// Initializes a new instance of the <see cref="W800Rf32Lib.ConnectionStatusChangedEventArgs"/> class.
        /// </summary>
        /// <param name="state">State of the connection (true = connected, false = not connected).</param>
        public ConnectionStatusChangedEventArgs(bool state)
        {
            Connected = state;
        }
    }

    /// <summary>
    /// Rf data received event arguments.
    /// </summary>
    public class RfDataReceivedEventArgs
    {
        /// <summary>
        /// The raw data.
        /// </summary>
        public readonly byte[] Data;

        /// <summary>
        /// Initializes a new instance of the <see cref="W800Rf32Lib.RfDataReceivedEventArgs"/> class.
        /// </summary>
        /// <param name="data">Data.</param>
        public RfDataReceivedEventArgs(byte[] data)
        {
            Data = data;
        }
    }

    /// <summary>
    /// RF command received event arguments.
    /// </summary>
    public class RfCommandReceivedEventArgs
    {
        /// <summary>
        /// The command.
        /// </summary>
        public readonly X10RfFunction Command;
        /// <summary>
        /// The house code.
        /// </summary>
        public readonly X10HouseCode HouseCode;
        /// <summary>
        /// The unit code.
        /// </summary>
        public readonly X10UnitCode UnitCode;

        /// <summary>
        /// Initializes a new instance of the <see cref="W800Rf32Lib.RfCommandReceivedEventArgs"/> class.
        /// </summary>
        /// <param name="function">Function.</param>
        /// <param name="housecode">Housecode.</param>
        /// <param name="unitcode">Unitcode.</param>
        public RfCommandReceivedEventArgs(X10RfFunction function, X10HouseCode housecode, X10UnitCode unitcode)
        {
            Command = function;
            HouseCode = housecode;
            UnitCode = unitcode;
        }
    }

    /// <summary>
    /// RF security received event arguments.
    /// </summary>
    public class RfSecurityReceivedEventArgs
    {
        /// <summary>
        /// The event.
        /// </summary>
        public readonly X10RfSecurityEvent Event;
        /// <summary>
        /// The address.
        /// </summary>
        public readonly uint Address;

        /// <summary>
        /// Initializes a new instance of the <see cref="W800Rf32Lib.RfSecurityReceivedEventArgs"/> class.
        /// </summary>
        /// <param name="evt">Evt.</param>
        /// <param name="addr">Address.</param>
        public RfSecurityReceivedEventArgs(X10RfSecurityEvent evt, uint addr)
        {
            Event = evt;
            Address = addr;
        }
    }

}

