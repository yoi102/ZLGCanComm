using ZLGCanComm.Api;
using ZLGCanComm.Records;

namespace ZLGCanComm;

internal class DataConverter
{
    public static VCI_CAN_OBJ Convert(CanObject value)
    {
        VCI_CAN_OBJ result = new()
        {
            ID = value.Id,
            TimeStamp = value.TimeStamp,
            TimeFlag = value.TimeFlag,
            SendType = value.SendType,
            RemoteFlag = value.RemoteFlag,
            ExternFlag = value.ExternFlag,
            DataLen = 8,
            Data = value.Data.Length == 8 ? value.Data : new byte[8], // 直接赋值
            Reserved = value.Reserved.Length == 3 ? value.Reserved : new byte[3] // 直接赋值
        };

        if (value.Data.Length != 8)
        {
            Array.Copy(value.Data, result.Data, Math.Min(value.Data.Length, 8));
            result.DataLen = (byte)Math.Min(value.Data.Length, 8);
        }

        if (value.Reserved.Length != 3)
        {
            Array.Copy(value.Reserved, result.Reserved, Math.Min(value.Reserved.Length, 3));
        }

        return result;
    }

    public static CanObject Convert(VCI_CAN_OBJ value)
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
            Reserved = value.Reserved
        };
        return result;
    }

    public static CanObject[] Convert(VCI_CAN_OBJ[] value)
    {
        return value.Select(Convert).ToArray();
    }

    public static VCI_CAN_OBJ[] Convert(CanObject[] value)
    {
        return value.Select(Convert).ToArray();
    }

    public static CanControllerStatus Convert(VCI_CAN_STATUS value)
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

    public static VCI_CAN_STATUS Convert(CanControllerStatus value)
    {
        VCI_CAN_STATUS result = new()
        {
            ErrInterrupt = value.ErrorInterrupt,
            regMode = value.ModeRegister,
            regStatus = value.StatusRegister,
            regALCapture = value.AutoWakeupCapture,
            regECCapture = value.ErrorCounterCapture,
            regEWLimit = value.ErrorWarningLimit,
            regRECounter = value.ReceiveErrorCounter,
            regTECounter = value.TransmitErrorCounter,
            Reserved = value.Reserved.Length == 4 ? value.Reserved : new byte[4] // 直接赋值
        };

        if (value.Reserved.Length != 4)
        {
            Array.Copy(value.Reserved, result.Reserved, Math.Min(value.Reserved.Length, 4));
        }

        return result;
    }

    public static ErrorInfo Convert(VCI_ERR_INFO value)
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

    public static VCI_ERR_INFO Convert(ErrorInfo value)
    {
        VCI_ERR_INFO result = new()
        {
            ErrCode = value.ErrorCode,
            Passive_ErrData1 = value.PassiveErrorData1,
            Passive_ErrData2 = value.PassiveErrorData2,
            Passive_ErrData3 = value.PassiveErrorData3,
            ArLost_ErrData = value.ArbitrationLostErrorData
        };

        return result;
    }

    public static VCI_INIT_CONFIG Convert(InitConfig value)
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

    public static InitConfig Convert(VCI_INIT_CONFIG value)
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

    public static BoardInfo Convert(VCI_BOARD_INFO value)
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

    public static VCI_BOARD_INFO Convert(BoardInfo value)
    {
        VCI_BOARD_INFO result = new()
        {
            hw_Version = value.HardwareVersion,
            fw_Version = value.FirmwareVersion,
            dr_Version = value.DriverVersion,
            in_Version = value.InterfaceVersion,
            irq_Num = value.IrqNumber,
            can_Num = value.CanChannelCount,
            str_Serial_Num = value.SerialNumber.Length == 20 ? value.SerialNumber : new byte[20], // 直接赋值
            str_hw_Type = value.HardwareType.Length == 40 ? value.HardwareType : new byte[40], // 直接赋值
            Reserved = value.Reserved.Length == 8 ? value.Reserved : new byte[8] // 直接赋值
        };
        if (value.SerialNumber.Length != 20)
        {
            Array.Copy(value.SerialNumber, result.str_Serial_Num, Math.Min(value.SerialNumber.Length, 20));
        }
        if (value.HardwareType.Length != 40)
        {
            Array.Copy(value.HardwareType, result.str_hw_Type, Math.Min(value.SerialNumber.Length, 40));
        }
        if (value.Reserved.Length != 8)
        {
            Array.Copy(value.Reserved, result.Reserved, Math.Min(value.SerialNumber.Length, 8));
        }
        return result;
    }
}