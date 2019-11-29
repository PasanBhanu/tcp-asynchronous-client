<p align="center">
  <img src="https://i.ibb.co/YD7yF8g/image.png">
</p>

<div align="center">
 
[![contributions welcome](https://img.shields.io/badge/contributions-welcome-brightgreen.svg?style=flat)](https://github.com/PasanBhanu/tcp-asynchronous-client/issues)
[![Nuget](https://img.shields.io/nuget/v/TCPAsynchronousClient)](https://www.nuget.org/packages/TCPAsynchronousClient)
[![HitCount](http://hits.dwyl.com/dwyl/start-here.svg)](https://github.com/PasanBhanu/tcp-asynchronous-client)
[![Nuget](https://img.shields.io/nuget/dt/TCPAsynchronousClient)](https://www.nuget.org/packages/TCPAsynchronousClient)

</div>

# TCP Asynchronous Client for C#.NET
TCP Asynchronous Client DLL for C#. Communicate with TCP Server in C# Applications. Simple and less resource usage model without multithreading or waitOne().

## How To Use
Add the DLL to your project as a reference. Create a TCPAsyncClient Object and use the functions. Complete workable example is given in Sample Application

## Constructors
+ public AsynchronousClient(string _ip, int _port)

## Functions Available
+ void Connect() - Connect to the server
+ bool Write(string _data) - Send data to server
+ bool Write(byte[] _data) - Send data to server
+ void Dispose() - Dispose socket/ connection
+ void Disconnect() - Disconnect connection
+ bool IsConnected() - Check is the socket connected

## Callbacks
+ Connection Status - **OnConnectEventHandler** -> void OnConnect(bool status)
+ Recieve Data - **DataReceivedEventHandler** -> void OnRecieved(string data)

## How To Use

### Create Object
````c#
AsynchronousClient tcp = new AsynchronousClient(txtIpAddress.Text, int.Parse(txtPort.Text));
tcp.OnConnectEvent += new AsynchronousClient.OnConnectEventHandler(OnConnect);
tcp.OnDataRecievedEvent += new AsynchronousClient.DataReceivedEventHandler(OnRecieved);
tcp.Connect();
````

#### Setup Connection Status Callback Function
````c#
private void OnConnect(bool status)
{
    Console.WriteLine("Connection Status : " + status.ToString());
}
````

#### Setup Data Recieved Callback Function
````c#
private void OnRecieved(string data)
{
    Console.WriteLine("Recieved Data : " + data);
}
````

### Write to TCP
````c#
private void WriteData(string data)
{
    try
      {
          if (tcp.Write(data))
          {
              Console.WriteLine("Write OK");
          }
          else
          {
              Console.WriteLine("Write Failed");
          }
      }catch(Exception ex)
      {
          // Catch errors in Sending Data
          Console.WriteLine("Error : " + ex.ToString());
      }
}
````

### Create TCP Server for Testing
You can use [Hercules Setup Utility](https://hercules-setup.soft32.com/) to create TCP server in Local Machine with few clicks. Here is an example.

![Hercules TCP Server Sample](https://i.ibb.co/Dt241Fw/image.png)

## Support
This application is developed using .NET Framework 4.6

## Download
Please go to release page to download the stable version. We recommend not to use alpha or beta releases for your projects.

## Contributing
Contributors are WELCOME!

## License
The MIT License (MIT). Please see [License File](https://github.com/PasanBhanu/TCPAsynchronousClient/blob/master/LICENSE) for more information.
