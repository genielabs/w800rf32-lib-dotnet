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

            RfReceiver x10rf = new RfReceiver();
            x10rf.ConnectionStatusChanged += X10rf_ConnectionStatusChanged;
            x10rf.RawDataReceived += X10rf_RawDataReceived;
            x10rf.X10CommandReceived += X10rf_X10CommandReceived;
            x10rf.X10SecurityReceived += X10rf_X10SecurityReceived;
            x10rf.PortName = "/dev/ttyUSB0";
            x10rf.Connect();

            // Prevent the program from quitting
            while (true)
            {
                Thread.Sleep(1000);
            }

        }

        static void X10rf_ConnectionStatusChanged(object sender, ConnectionStatusChangedEventArgs args)
        {
            Console.WriteLine("Receiver connected state {0}", args.Connected);
        }

        static void X10rf_RawDataReceived(object sender, RawDataReceivedEventArgs args)
        {
            Console.WriteLine("Received RF raw data: {0}", BitConverter.ToString(args.Data));
        }

        static void X10rf_X10CommandReceived(object sender, X10CommandReceivedEventArgs args)
        {
            Console.WriteLine("Received X10 command {0} House Code {1} Unit {2}", args.Command, args.HouseCode, args.UnitCode);
        }

        static void X10rf_X10SecurityReceived(object sender, X10SecurityReceivedEventArgs args)
        {
            Console.WriteLine("Received X10 Security event {0} from address {1}", args.Event, args.Address.ToString("X2"));
        }

    }
}
