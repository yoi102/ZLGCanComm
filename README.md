


ZLGCan设备的通讯库   
编译时，必须配置为X86   
当前仅能连接TCP、USBCan1类型的设备   

 使用例子
``` C#
using System.Diagnostics;
using ZLGCanComm.Devices;
using ZLGCanComm.Structs;

//必须设置为X86！！！
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
Console.WriteLine(canObject.Id);
Console.WriteLine(canObject.Data);
//监听设备
tcpCanDevice.RegisterListener(Listener);

canObject.Data = new byte[8];
//写入设备
tcpCanDevice.TryWriteMessage(canObject.Id, canObject.Data);
//如果不用必须Disposable当前实例
tcpCanDevice.Dispose();


void Listener(CanObject canObject)
{
    Console.WriteLine(canObject.Id);
    Console.WriteLine(canObject.Data);
}
```
