using ZLGCanComm.Records;

namespace ZLGCanComm;

internal class StructConverter
{
    public static VCI_CAN_OBJ Converter(CanObject value)
    {
        VCI_CAN_OBJ result = new()
        {
            ID = value.Id,
            TimeStamp = value.TimeStamp,
            TimeFlag = value.TimeFlag,
            SendType = value.SendType,
            RemoteFlag = value.RemoteFlag,
            ExternFlag = value.ExternFlag,
            // 初始化 result.Data 为长度为 8 的数组
            Data = new byte[8]
        };
        // 根据 value.Data 的长度，确定要复制的长度（最多 8 个）
        int lengthToCopy = Math.Min(value.Data.Length, 8);
        // 复制 value.Data 的前 lengthToCopy 个元素到 result.Data
        Array.Copy(value.Data, result.Data, lengthToCopy);
        result.DataLen = (byte)result.Data!.Length;
        result.Reserved = value.Reserved;
        return result;
    }

    public static CanObject Converter(VCI_CAN_OBJ value)
    {
        CanObject result = new()
        {
            Id = value.ID,
            TimeStamp = value.TimeStamp,
            TimeFlag = value.TimeFlag,
            SendType = value.SendType,
            RemoteFlag = value.RemoteFlag,
            ExternFlag = value.ExternFlag,
            Data = value.Data,
            DataLength = value.DataLen,
            Reserved = value.Reserved
        };
        return result;
    }

    public static CanControllerStatus Converter(VCI_CAN_STATUS value)
    {
        CanControllerStatus result = new()
        {
            ErrorInterrupt = value.ErrInterrupt,
            ModeRegister = value.regMode,
            StatusRegister = value.regStatus,
            AutoWakeupCapture = value.regALCapture,
            ErrorCounterCapture = value.regECCapture,
            ErrorWarningLimit = value.regEWLimit,
            ReceiveErrorCounter = value.regRECounter,
            TransmitErrorCounter = value.regTECounter,
            Reserved = value.Reserved
        };

        return result;
    }

    public static ErrorInfo Converter(VCI_ERR_INFO value)
    {
        ErrorInfo result = new()
        {
            ErrorCode = value.ErrCode,
            PassiveErrorData1 = value.Passive_ErrData1,
            PassiveErrorData2 = value.Passive_ErrData2,
            PassiveErrorData3 = value.Passive_ErrData3,
            ArbitrationLostErrorData = value.ArLost_ErrData
        };

        return result;
    }

    public static VCI_INIT_CONFIG Converter(InitConfig value)
    {
        VCI_INIT_CONFIG result = new();
        result.AccCode = value.AcceptanceCode;
        result.AccMask = value.AcceptanceMask;
        result.Reserved = value.Reserved;
        result.Filter = value.Filter;
        result.Timing0 = value.Timing0;
        result.Timing1 = value.Timing1;
        result.Mode = value.Mode;

        return result;
    }

    public static InitConfig Converter(VCI_INIT_CONFIG value)
    {
        InitConfig result = new()
        {
            AcceptanceCode = value.AccCode,
            AcceptanceMask = value.AccMask,
            Reserved = value.Reserved,
            Filter = value.Filter,
            Timing0 = value.Timing0,
            Timing1 = value.Timing1,
            Mode = value.Mode
        };

        return result;
    }
    public static BoardInfo Converter(VCI_BOARD_INFO value)
    {
        BoardInfo result = new()
        {
            HardwareVersion = value.hw_Version,
            FirmwareVersion = value.fw_Version,
            DriverVersion = value.dr_Version,
            InterfaceVersion = value.in_Version,
            IrqNumber = value.irq_Num,
            CanChannelCount = value.can_Num,
            SerialNumber = value.str_Serial_Num,
            HardwareType = value.str_hw_Type,
            Reserved = value.Reserved
        };

        return result;
    }
    public static VCI_BOARD_INFO Converter(BoardInfo value)
    {
        VCI_BOARD_INFO result = new()
        {
            hw_Version = value.HardwareVersion,
            fw_Version = value.FirmwareVersion,
            dr_Version = value.DriverVersion,
            in_Version = value.InterfaceVersion,
            irq_Num = value.IrqNumber,
            can_Num = value.CanChannelCount,
            str_Serial_Num = value.SerialNumber,
            str_hw_Type = value.HardwareType,
            Reserved = value.Reserved
        };

        return result;
    }




}