using ZLGCanComm.Structs;

namespace ZLGCanComm;

internal class StructConverter
{
    public static VCI_CAN_OBJ CanObjectToVCI_CAN_OBJ(CanObject canObject)
    {
        VCI_CAN_OBJ result = new VCI_CAN_OBJ();
        result.Init();
        result.ID = canObject.Id;
        result.TimeStamp = canObject.TimeStamp;
        result.TimeFlag = canObject.TimeFlag;
        result.SendType = canObject.SendType;
        result.RemoteFlag = canObject.RemoteFlag;
        result.ExternFlag = canObject.ExternFlag;
        Array.Copy(canObject.Data, result.Data!, 8);
        result.DataLen = (byte)result.Data!.Length;
        result.Reserved = canObject.Reserved;
        return result;
    }

    public static CanObject VCI_CAN_OBJToCanObject(VCI_CAN_OBJ obj)
    {
        CanObject result = new CanObject();
        result.Id = obj.ID;
        result.TimeStamp = obj.TimeStamp;
        result.TimeFlag = obj.TimeFlag;
        result.SendType = obj.SendType;
        result.RemoteFlag = obj.RemoteFlag;
        result.ExternFlag = obj.ExternFlag;
        result.Data = obj.Data;
        result.DataLength = obj.DataLen;
        result.Reserved = obj.Reserved;
        return result;
    }

    public static CanControllerStatus VCI_CAN_STATUSToCanControllerStatus(VCI_CAN_STATUS status)
    {
        CanControllerStatus result = new CanControllerStatus();
        result.ErrorInterrupt = status.ErrInterrupt;
        result.ModeRegister = status.regMode;
        result.StatusRegister = status.regStatus;
        result.AutoWakeupCapture = status.regALCapture;
        result.ErrorCounterCapture = status.regECCapture;
        result.ErrorWarningLimit = status.regEWLimit;
        result.ReceiveErrorCounter = status.regRECounter;
        result.TransmitErrorCounter = status.regTECounter;
        result.Reserved = status.Reserved;

        return result;
    }

    public static ErrorInfo VCI_ERR_INFOToErrorInfo(VCI_ERR_INFO info)
    {
        ErrorInfo result = new ErrorInfo();
        result.ErrorCode = info.ErrCode;
        result.PassiveErrorData1 = info.Passive_ErrData1;
        result.PassiveErrorData2 = info.Passive_ErrData2;
        result.PassiveErrorData3 = info.Passive_ErrData3;
        result.ArbitrationLostErrorData = info.ArLost_ErrData;

        return result;
    }
}