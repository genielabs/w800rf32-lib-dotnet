# W800RF32 RF Receiver library for X10 Home Automation (.NET / Mono)

## Features

- Decoding of both standard and security X10 messages
- Event driven
- Hot plug
- Automatically restabilish connection on error/disconnect
- Compatible with Mono

## NuGet Package

W800Rf32Lib  is available as a [NuGet package](https://www.nuget.org/packages/W800Rf32Lib)

Run `Install-Package W800Rf32Lib` in the [Package Manager Console](http://docs.nuget.org/docs/start-here/using-the-package-manager-console) or search for “W800Rf32Lib” in your IDE’s package management plug-in.

## Example usage

```csharp
using W800Rf32Lib;
//...

RfReceiver x10rf = new RfReceiver();

// Listen to events
x10rf.ConnectionStatusChanged += delegate(object sender, ConnectionStatusChangedEventArgs args)
{
    Console.WriteLine("Receiver connected state {0}", args.Connected);
};
x10rf.RfDataReceived += delegate(object sender, RfDataReceivedEventArgs args)
{
    Console.WriteLine("Received RF raw data: {0}", BitConverter.ToString(args.Data));
};
x10rf.RfCommandReceived += delegate(object sender, RfCommandReceivedEventArgs args)
{
    Console.WriteLine("Received X10 command {0} House Code {1} Unit {2}",
        args.Command, args.HouseCode, args.UnitCode);
};
x10rf.RfSecurityReceived += delegate(object sender, RfSecurityReceivedEventArgs args)
{
    Console.WriteLine("Received X10 Security event {0} from address {1}",
        args.Event, args.Address.ToString("X2"));
};

// Set the serial port which the W800RF32 is connected to
x10rf.PortName = "/dev/ttyUSB0";

// Connect to the receiver
x10rf.Connect();
```

## License

W800Rf32Lib is open source software, licensed under the terms of GNU GPLV3 license. See the [LICENSE](LICENSE) file for details.
