using System.Diagnostics;
using ZLGCanComm.Devices;
using ZLGCanComm.Extensions;
using ZLGCanComm.Records;


//必须设置为X86！！！

/////Tcp示例子
{
    TcpCanDevice tcpCanDevice = new TcpCanDevice("192.168.1.123", "4001");
    //连接设备
    var connected = tcpCanDevice.TryConnect();
    if (!connected)
    {
        //检查设备是否连接、CanIndex是否设置正确。ip端口是否正确！！
        Debugger.Break();
        return;
    }

    //读取设备信息
    tcpCanDevice.TryReadMessage(out var canObject);
    if (canObject is null)
    {
        Debugger.Break();
        return;
    }
    Console.WriteLine(canObject.Id);
    Console.WriteLine(canObject.Data);
    //监听设备
    tcpCanDevice.RegisterListener(Listener);

    canObject.Data = new byte[8];
    //写入设备
    tcpCanDevice.TryWriteMessage(canObject.Id, canObject.Data);
    //如果不用必须Disposable当前实例
    tcpCanDevice.Dispose();
}

/////Usb示例子
{
    var usbCan1Device = new UsbCan1Device();

    var usbConnected = usbCan1Device.TryConnect();
    if (!usbConnected)
    {
        //检查设备是否连接、CanIndex是否设置正确。ip端口是否正确！！
        Debugger.Break();
        return;
    }

    //读取设备信息
    usbCan1Device.TryReadMessage(out var canObject);

    if (canObject is null)
    {
        Debugger.Break();
        return;
    }
    Console.WriteLine(canObject.Id);
    Console.WriteLine(canObject.Data);
    //监听设备
    usbCan1Device.RegisterListener(Listener);

    canObject.Data = new byte[8];
    //写入设备
    usbCan1Device.TryWriteMessage(canObject.Id, canObject.Data);
    //如果不用必须Disposable当前实例
    usbCan1Device.Dispose();
}

void Listener(CanObject canObject)
{
    Console.WriteLine(canObject.Id);
    Console.WriteLine(canObject.Data);
}