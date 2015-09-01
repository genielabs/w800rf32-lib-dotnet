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

using NLog;
using SerialPortLib;
using System.Threading;

namespace W800Rf32Lib
{
    /// <summary>
    /// W800RF32 receiver class.
    /// </summary>
    public class RfReceiver
    {

        #region Private Fields

        internal static Logger logger = LogManager.GetCurrentClassLogger();

        private SerialPortInput serialPort;
        private string portName = "/dev/ttyUSB0";
        private bool receiverOnline = false;

        private byte[] ackRequest = new byte[] { 0xF0, 0x29 };
        private byte ackReply = 0x29;
        private Timer receiverAckWaitTimeout;

        // Variables used for preventing duplicated messages coming from RF
        private DateTime lastRfReceivedTs = DateTime.Now;
        private string lastRfMessage = "";
        private uint minRfRepeatDelayMs = 500;

        #endregion

        #region Public Events

        /// <summary>
        /// Connected state changed event.
        /// </summary>
        public delegate void ConnectionStatusChangedEventHandler(object sender, ConnectionStatusChangedEventArgs args);

        /// <summary>
        /// Occurs when connected state changed.
        /// </summary>
        public event ConnectionStatusChangedEventHandler ConnectionStatusChanged;

        /// <summary>
        /// Raw data received event.
        /// </summary>
        public delegate void RawDataReceivedEventHandler(object sender, RfDataReceivedEventArgs args);

        /// <summary>
        /// Occurs when raw data is received.
        /// </summary>
        public event RawDataReceivedEventHandler RfDataReceived;

        /// <summary>
        /// X10 command received event.
        /// </summary>
        public delegate void X10CommandReceivedEventHandler(object sender, RfCommandReceivedEventArgs args);

        /// <summary>
        /// Occurs when x10 command received.
        /// </summary>
        public event X10CommandReceivedEventHandler RfCommandReceived;

        /// <summary>
        /// X10 security data received event.
        /// </summary>
        public delegate void X10SecurityReceivedEventHandler(object sender, RfSecurityReceivedEventArgs args);

        /// <summary>
        /// Occurs when x10 security data is received.
        /// </summary>
        public event X10SecurityReceivedEventHandler RfSecurityReceived;

        #endregion

        #region Instance management

        /// <summary>
        /// Initializes a new instance of the <see cref="W800Rf32Lib.RfReceiver"/> class.
        /// </summary>
        public RfReceiver()
        {
            serialPort = new SerialPortInput();
            serialPort.SetPort(portName, 4800);
            serialPort.ConnectionStatusChanged += SerialPort_ConnectionStatusChanged;
            serialPort.MessageReceived += SerialPort_MessageReceived;
        }

        #endregion

        #region Public members

        /// <summary>
        /// Connect to the W800Rf32 receiver.
        /// </summary>
        public bool Connect()
        {
            Disconnect();
            if (receiverAckWaitTimeout == null)
            {
                receiverAckWaitTimeout = new Timer(delegate(object target)
                {
                    if (!receiverOnline)
                        Connect();
                });
            }
            receiverAckWaitTimeout.Change(3000, Timeout.Infinite);
            return serialPort.Connect();
        }

        /// <summary>
        /// Disconnect from the W800Rf32 receiver.
        /// </summary>
        public void Disconnect()
        {
            if (receiverAckWaitTimeout != null)
            {
                receiverAckWaitTimeout.Dispose();
                receiverAckWaitTimeout = null;
            }
            serialPort.Disconnect();
            OnConnectionStatusChanged(new ConnectionStatusChangedEventArgs(false));
        }

        /// <summary>
        /// Gets a value indicating whether the RF receiver hardware is connected.
        /// </summary>
        /// <value><c>true</c> if connected; otherwise, <c>false</c>.</value>
        public bool IsConnected
        {
            get { return receiverOnline; }
        }

        /// <summary>
        /// Gets or sets the name of the serial port where the receiver is connected to.
        /// </summary>
        /// <value>The name of the port.</value>
        public string PortName
        {
            get { return portName; }
            set
            {
                if (portName != value)
                {
                    serialPort.SetPort(value, 4800);
                }
                portName = value;
            }
        }

        #endregion

        #region Private members

        #region Serial Port events handling

        private void SerialPort_MessageReceived(object sender, MessageReceivedEventArgs args)
        {
            byte[] message = args.Data;
            bool isSecurityCode = (message.Length == 4 && ((message[1] ^ message[0]) == 0x0F) && ((message[3] ^ message[2]) == 0xFF));
            bool isCodeValid = isSecurityCode || (message.Length == 4 && ((message[1] & ~message[0]) == message[1] && (message[3] & ~message[2]) == message[3]));

            // Repeated messages check
            if (isCodeValid)
            {
                if (lastRfMessage == BitConverter.ToString(message) && (DateTime.Now - lastRfReceivedTs).TotalMilliseconds < minRfRepeatDelayMs)
                {
                    logger.Warn("Ignoring repeated message within {0}ms", minRfRepeatDelayMs);
                    return;
                }
                lastRfMessage = BitConverter.ToString(message);
                lastRfReceivedTs = DateTime.Now;
            }

            OnRfDataReceived(new RfDataReceivedEventArgs(message));

            if (isSecurityCode)
            {
                var securityEvent = X10RfSecurityEvent.NotSet;
                Enum.TryParse<X10RfSecurityEvent>(message[2].ToString(), out securityEvent);
                uint securityAddress = message[0]; //BitConverter.ToUInt32(new byte[] { message[0] }, 0);
                if (securityEvent != X10RfSecurityEvent.NotSet)
                {
                    logger.Debug("Security Event {0} Address {1}", securityEvent, securityAddress);
                    OnRfSecurityReceived(new RfSecurityReceivedEventArgs(securityEvent, securityAddress));
                }
                else
                {
                    logger.Warn("Could not parse security event");
                }
            }
            else if (isCodeValid)
            {
                // Decode received 32 bit message
                // house code + 4th bit of unit code
                // unit code (3 bits) + function code

                // Parse function code
                var hf = X10RfFunction.NotSet;
                Enum.TryParse<X10RfFunction>(message[2].ToString(), out hf);
                // House code (4bit) + unit code (4bit)
                byte hu = message[0];
                // Parse house code
                var houseCode = X10HouseCode.NotSet;
                Enum.TryParse<X10HouseCode>((ReverseByte((byte)(hu >> 4)) >> 4).ToString(), out houseCode);
                switch (hf)
                {
                case X10RfFunction.Dim:
                case X10RfFunction.Bright:
                    logger.Debug("Command {0}", hf);
                    OnRfCommandReceived(new RfCommandReceivedEventArgs(hf, X10HouseCode.NotSet, X10UnitCode.Unit_NotSet));
                    break;
                case X10RfFunction.AllLightsOn:
                case X10RfFunction.AllLightsOff:
                    if (houseCode != X10HouseCode.NotSet)
                    {
                        logger.Debug("Command {0} HouseCode {1}", hf, houseCode);
                        OnRfCommandReceived(new RfCommandReceivedEventArgs(hf, houseCode, X10UnitCode.Unit_NotSet));
                    }
                    break;
                case X10RfFunction.NotSet:
                    logger.Warn("Unable to decode function value");
                    break;
                default:
                    // Parse unit code
                    string houseUnit = Convert.ToString(hu, 2).PadLeft(8, '0');
                    string unitFunction = Convert.ToString(message[2], 2).PadLeft(8, '0');
                    string uc = (Convert.ToInt16(houseUnit.Substring(5, 1) + unitFunction.Substring(1, 1) + unitFunction.Substring(4, 1) + unitFunction.Substring(3, 1), 2) + 1).ToString();
                    // Parse module function
                    var fn = X10RfFunction.NotSet;
                    Enum.TryParse<X10RfFunction>(unitFunction[2].ToString(), out fn);
                    switch (fn)
                    {
                    case X10RfFunction.On:
                    case X10RfFunction.Off:
                        var unitCode = X10UnitCode.Unit_NotSet;
                        Enum.TryParse<X10UnitCode>("Unit_" + uc.ToString(), out unitCode);
                        if (unitCode != X10UnitCode.Unit_NotSet)
                        {
                            logger.Debug("Command {0} HouseCode {1} UnitCode {2}", fn, houseCode, unitCode.Value());
                            OnRfCommandReceived(new RfCommandReceivedEventArgs(fn, houseCode, unitCode));
                        }
                        else
                        {
                            logger.Warn("Could not parse unit code");
                        }
                        break;
                    }
                    break;
                }
            }
            else if (message.Length == 1 && message[0] == ackReply)
            {
                logger.Debug("W800Rf32 is online");
                OnConnectionStatusChanged(new ConnectionStatusChangedEventArgs(true));
            }
            else
            {
                logger.Warn("Bad message received");
            }
        }

        private void SerialPort_ConnectionStatusChanged(object sender, SerialPortLib.ConnectionStatusChangedEventArgs args)
        {
            logger.Debug("Serial Port Connected = {0}", args.Connected);
            if (args.Connected)
            {
                serialPort.SendMessage(ackRequest);
            }
            else
            {
                logger.Debug("W800Rf32 is offline");
                OnConnectionStatusChanged(new ConnectionStatusChangedEventArgs(false));
            }
        }

        #endregion

        #region Utility Methods

        private byte ReverseByte(byte originalByte)
        {
            int result = 0;
            for (int i = 0; i < 8; i++)
            {
                result = result << 1;
                result += originalByte & 1;
                originalByte = (byte)(originalByte >> 1);
            }
            return (byte)result;
        }

        #endregion

        #region Events Raising

        /// <summary>
        /// Raises the connected state changed event.
        /// </summary>
        /// <param name="args">Arguments.</param>
        protected virtual void OnConnectionStatusChanged(ConnectionStatusChangedEventArgs args)
        {
            bool wasOnline = receiverOnline;
            receiverOnline = args.Connected;
            if (ConnectionStatusChanged != null && wasOnline != receiverOnline)
                ConnectionStatusChanged(this, args);
        }

        /// <summary>
        /// Raises the rf data received event.
        /// </summary>
        /// <param name="args">Arguments.</param>
        protected virtual void OnRfDataReceived(RfDataReceivedEventArgs args)
        {
            if (RfDataReceived != null)
                RfDataReceived(this, args);
        }

        /// <summary>
        /// Raises the RF command received event.
        /// </summary>
        /// <param name="args">Arguments.</param>
        protected virtual void OnRfCommandReceived(RfCommandReceivedEventArgs args)
        {
            if (RfCommandReceived != null)
                RfCommandReceived(this, args);
        }

        /// <summary>
        /// Raises the RF security received event.
        /// </summary>
        /// <param name="args">Arguments.</param>
        protected virtual void OnRfSecurityReceived(RfSecurityReceivedEventArgs args)
        {
            if (RfSecurityReceived != null)
                RfSecurityReceived(this, args);
        }

        #endregion

        #endregion

    }

}

