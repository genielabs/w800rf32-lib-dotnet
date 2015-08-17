/*
    This file is part of W800Rf32Lib source code.

    W800Rf32Lib is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    W800Rf32Lib is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with W800Rf32Lib.  If not, see <http://www.gnu.org/licenses/>.  
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
        /// Initializes a new instance of the <see cref="SerialPortLib.ConnectionStatusChangedEventArgs"/> class.
        /// </summary>
        /// <param name="state">State of the connection (true = connected, false = not connected).</param>
        public ConnectionStatusChangedEventArgs(bool state)
        {
            Connected = state;
        }
    }

    /// <summary>
    /// Raw data received event arguments.
    /// </summary>
    public class RawDataReceivedEventArgs
    {
        /// <summary>
        /// The raw data.
        /// </summary>
        public readonly byte[] Data;

        /// <summary>
        /// Initializes a new instance of the <see cref="W800Rf32Lib.RawDataReceivedEventArgs"/> class.
        /// </summary>
        /// <param name="data">Data.</param>
        public RawDataReceivedEventArgs(byte[] data)
        {
            Data = data;
        }
    }

    /// <summary>
    /// X10 command received event arguments.
    /// </summary>
    public class X10CommandReceivedEventArgs
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
        /// Initializes a new instance of the <see cref="W800Rf32Lib.X10CommandReceivedEventArgs"/> class.
        /// </summary>
        /// <param name="function">Function.</param>
        /// <param name="housecode">Housecode.</param>
        /// <param name="unitcode">Unitcode.</param>
        public X10CommandReceivedEventArgs(X10RfFunction function, X10HouseCode housecode, X10UnitCode unitcode)
        {
            Command = function;
            HouseCode = housecode;
            UnitCode = unitcode;
        }
    }

    /// <summary>
    /// X10 security received event arguments.
    /// </summary>
    public class X10SecurityReceivedEventArgs
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
        /// Initializes a new instance of the <see cref="W800Rf32Lib.X10SecurityReceivedEventArgs"/> class.
        /// </summary>
        /// <param name="evt">Evt.</param>
        /// <param name="addr">Address.</param>
        public X10SecurityReceivedEventArgs(X10RfSecurityEvent evt, uint addr)
        {
            Event = evt;
            Address = addr;
        }
    }

}

