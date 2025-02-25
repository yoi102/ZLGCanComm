using Moq;
using ZLGCanComm.Api;
using ZLGCanComm.Devices;
using ZLGCanComm.Interfaces;
using ZLGCanComm.Records;

namespace ZLGCanComm.Test;

public class CanDeviceBaseTests
{
    private Mock<IZLGApi> _mockZLGApi;
    private Mock<ICanDevice> _mockDevice;

    public CanDeviceBaseTests()
    {
        _mockZLGApi = new Mock<IZLGApi>();
        _mockDevice = new Mock<ICanDevice>();
        //ZLGApiProvider.Instance = _mockZLGApi.Object;
    }

    //[Fact]
    //public void Connect_Should_Set_IsConnected_True_When_Successful()
    //{
    //    _mockZLGApi.Setup(x => x.StartCAN(It.IsAny<uint>(), It.IsAny<uint>(), It.IsAny<uint>()));

    //    _mockDevice.Object.Connect();

    //    Assert.Equal(_mockDevice.Object.IsConnected, true);
    //}

    //[Fact]
    //public void Dispose_Should_Set_IsConnected_False()
    //{
    //    _mockDevice.Object.Dispose();

    //    Assert.IsFalse(_mockDevice.Object.IsConnected);
    //}

    //[Fact]
    //public void DeviceIndex_Should_Be_Set_Correctly()
    //{
    //    Assert.AreEqual(1, _mockDevice.Object.DeviceIndex);
    //}

    //[Fact]
    //public void TryReadBoardInfo_Should_Update_BoardInfo()
    //{
    //    BoardInfo? boardInfo;
    //    _mockZLGApi.Setup(x => x.ReadBoardInfo(It.IsAny<uint>(), It.IsAny<uint>(), out boardInfo)).Returns(true);
    //    _mockDevice.SetupProperty(x => x.IsConnected, true);


    //    var result = _mockDevice.Object.ReadBoardInfo();

    //    Assert.NotNull(result);
    //}

    //[Fact]
    //public void TryReadStatus_Should_Update_Status()
    //{
    //    var status = new CanControllerStatus();
    //    _mockDevice.Setup(d => d.TryReadStatus(out status)).Returns(true);

    //    bool result = _mockDevice.Object.TryReadStatus(out var retrievedStatus);

    //    Assert.IsTrue(result);
    //    Assert.AreEqual(status, retrievedStatus);
    //}

    //[Fact]
    //public void TryReadErrorInfo_Should_Update_ErrorInfo()
    //{
    //    var errorInfo = new ErrorInfo();
    //    _mockDevice.Setup(d => d.TryReadErrorInfo(out errorInfo)).Returns(true);

    //    bool result = _mockDevice.Object.TryReadErrorInfo(out var retrievedError);

    //    Assert.IsTrue(result);
    //    Assert.AreEqual(errorInfo, retrievedError);
    //}
}






