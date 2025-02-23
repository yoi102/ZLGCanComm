using System.Diagnostics;
using ZLGCanComm.Devices;
using ZLGCanComm.Extensions;
using ZLGCanComm.Records;

//必须设置为X86！！！

const uint _NODE_ = 0X400;//CAN节点的下行命令帧ID的前缀;

/////Tcp示例
{
    var tcpCanDevice = new TcpCanDevice("192.168.1.123", "4001");
    //连接设备
    var connected = tcpCanDevice.TryConnect();
    if (!connected)
    {
        //检查设备是否连接、CanIndex是否设置正确。ip端口是否正确！！
        Debugger.Break();
        return;
    }

    //读取设备信息
    var canObjects = tcpCanDevice.Receive();

    foreach (var item in canObjects)
    {
        Console.WriteLine(item.Id);
        Console.WriteLine(item.Data);
    }

    //监听设备
    tcpCanDevice.Subscribe(Received);

    //发送单帧数据
    tcpCanDevice.Transmit(_NODE_ | 11, [1, 2, 3, 4, 5, 6, 7, 8]);

    //发送多帧数据
    CanObject[] sendObjects = [new CanObject(), new CanObject(), new CanObject()];
    tcpCanDevice.Transmit(sendObjects);

    //如果不用必须Disposable当前实例
    tcpCanDevice.Dispose();
}

/////Usb示例
{
    var usbCan1Device = new UsbCan1Device();

    var usbConnected = usbCan1Device.TryConnect();
    //读取设备信息
    var canObjects = usbCan1Device.Receive();

    foreach (var item in canObjects)
    {
        Console.WriteLine(item.Id);
        Console.WriteLine(item.Data);
    }

    //监听设备
    usbCan1Device.Subscribe(Received);

    //发送单帧数据
    usbCan1Device.Transmit(_NODE_ | 11, [1, 2, 3, 4, 5, 6, 7, 8]);

    //发送多帧数据
    CanObject[] sendObjects = [new CanObject(), new CanObject(), new CanObject()];
    usbCan1Device.Transmit(sendObjects);

    //如果不用必须Disposable当前实例
    usbCan1Device.Dispose();
}

void Received(CanObject[] canObjects)
{
    foreach (var item in canObjects)
    {
        Console.WriteLine(item.Id);
        Console.WriteLine(item.Data);
    }
}