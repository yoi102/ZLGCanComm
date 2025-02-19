using ZLGCanComm.Structs;

namespace ZLGCanComm;

internal class StructConverter
{
    public static VCI_CAN_OBJ Converter(CanObject canObject)
    {
        VCI_CAN_OBJ result = new()
        {
            ID = canObject.Id,
            TimeStamp = canObject.TimeStamp,
            TimeFlag = canObject.TimeFlag,
            SendType = canObject.SendType,
            RemoteFlag = canObject.RemoteFlag,
            ExternFlag = canObject.ExternFlag,
            // 初始化 result.Data 为长度为 8 的数组
            Data = new byte[8]
        };
        // 根据 canObject.Data 的长度，确定要复制的长度（最多 8 个）
        int lengthToCopy = Math.Min(canObject.Data.Length, 8);
        // 复制 canObject.Data 的前 lengthToCopy 个元素到 result.Data
        Array.Copy(canObject.Data, result.Data, lengthToCopy);
        result.DataLen = (byte)result.Data!.Length;
        result.Reserved = canObject.Reserved;
        return result;
    }

    public static CanObject Converter(VCI_CAN_OBJ obj)
    {
        CanObject result = new()
        {
            Id = obj.ID,
            TimeStamp = obj.TimeStamp,
            TimeFlag = obj.TimeFlag,
            SendType = obj.SendType,
            RemoteFlag = obj.RemoteFlag,
            ExternFlag = obj.ExternFlag,
            Data = obj.Data,
            DataLength = obj.DataLen,
            Reserved = obj.Reserved
        };
        return result;
    }

    public static CanControllerStatus Converter(VCI_CAN_STATUS status)
    {
        CanControllerStatus result = new()
        {
            ErrorInterrupt = status.ErrInterrupt,
            ModeRegister = status.regMode,
            StatusRegister = status.regStatus,
            AutoWakeupCapture = status.regALCapture,
            ErrorCounterCapture = status.regECCapture,
            ErrorWarningLimit = status.regEWLimit,
            ReceiveErrorCounter = status.regRECounter,
            TransmitErrorCounter = status.regTECounter,
            Reserved = status.Reserved
        };

        return result;
    }

    public static ErrorInfo Converter(VCI_ERR_INFO info)
    {
        ErrorInfo result = new()
        {
            ErrorCode = info.ErrCode,
            PassiveErrorData1 = info.Passive_ErrData1,
            PassiveErrorData2 = info.Passive_ErrData2,
            PassiveErrorData3 = info.Passive_ErrData3,
            ArbitrationLostErrorData = info.ArLost_ErrData
        };

        return result;
    }

    public static VCI_INIT_CONFIG Converter(InitConfig config)
    {
        VCI_INIT_CONFIG result = new();
        result.AccCode = config.AcceptanceCode;
        result.AccMask = config.AcceptanceMask;
        result.Reserved = config.Reserved;
        result.Filter = config.Filter;
        result.Timing0 = config.Timing0;
        result.Timing1 = config.Timing1;
        result.Mode = config.Mode;

        return result;
    }

    public static InitConfig Converter(VCI_INIT_CONFIG config)
    {
        InitConfig result = new()
        {
            AcceptanceCode = config.AccCode,
            AcceptanceMask = config.AccMask,
            Reserved = config.Reserved,
            Filter = config.Filter,
            Timing0 = config.Timing0,
            Timing1 = config.Timing1,
            Mode = config.Mode
        };

        return result;
    }
}