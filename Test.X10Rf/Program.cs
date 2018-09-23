﻿/*
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

using System;
using System.Threading;

using W800Rf32Lib;
using NLog;

namespace Test.X10Rf
{
    class MainClass
    {

        public static void Main(string[] args)
        {
            // NOTE: To disable debug output uncomment the following two lines
            //LogManager.Configuration.LoggingRules.RemoveAt(0);
            //LogManager.Configuration.Reload();

            Console.WriteLine("W800RF32 Test Program");

            var x10rf = new RfReceiver();
            // Listen to W800RF32 events
            x10rf.ConnectionStatusChanged += X10rf_ConnectionStatusChanged;
            x10rf.RfDataReceived += X10rf_RfDataReceived;
            x10rf.RfCommandReceived += X10rf_RfCommandReceived;
            x10rf.RfSecurityReceived += X10rf_RfSecurityReceived;
            // Set the serial port to use
            x10rf.PortName = "/dev/ttyUSB0";
            // Connect to the receiver
            x10rf.Connect();

            // Prevent the program from quitting with a noop loop
            while (true)
            {
                Thread.Sleep(1000);
            }

        }

        static void X10rf_ConnectionStatusChanged(object sender, ConnectionStatusChangedEventArgs args)
        {
            Console.WriteLine("Receiver connection status {0}", args.Connected);
        }

        static void X10rf_RfDataReceived(object sender, RfDataReceivedEventArgs args)
        {
            Console.WriteLine("Received RF raw data: {0}", BitConverter.ToString(args.Data));
        }

        static void X10rf_RfCommandReceived(object sender, RfCommandReceivedEventArgs args)
        {
            Console.WriteLine("Received X10 command {0} House Code {1} Unit {2}", args.Command, args.HouseCode, args.UnitCode);
        }

        static void X10rf_RfSecurityReceived(object sender, RfSecurityReceivedEventArgs args)
        {
            Console.WriteLine("Received X10 Security event {0} from address {1}", args.Event, args.Address.ToString("X2"));
        }

    }
}
