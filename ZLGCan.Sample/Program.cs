using ZLGCan.Devices;
using ZLGCan.Structs;

TcpCanDevice tcpCanDevice = new TcpCanDevice("192.168.1.123", "4001");

tcpCanDevice.TryConnect();

tcpCanDevice.TryReadMessage(out var canObject);
Console.WriteLine(canObject.Id);
Console.WriteLine(canObject.Data);

tcpCanDevice.Listen(Listener);

canObject.Data = new byte[8];

tcpCanDevice.TryWriteMessage(canObject.Id, canObject.Data);




void Listener(CanObject canObject)
{
    Console.WriteLine(canObject.Id);
    Console.WriteLine(canObject.Data);
}