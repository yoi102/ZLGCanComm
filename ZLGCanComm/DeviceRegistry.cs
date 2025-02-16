using ZLGCanComm.Enums;

namespace ZLGCanComm;
internal class DeviceRegistry
{
    //DeviceType - deviceIndex 
    internal static List<KeyValuePair<DeviceType, uint>> DeviceTypeIndexTracker { get; } = [];


    internal static uint GetUniqueDeviceIndex(DeviceType deviceType)
    {
        var indices = DeviceTypeIndexTracker.Where(x => x.Key == deviceType)
                                                .Select(x => x.Value).ToArray();

        return FindMissingNumber(indices);
    }


    private static uint FindMissingNumber(uint[] numbers)
    {
        uint n = (uint)numbers.Length;
        uint totalSum = n * (n + 1) / 2;
        uint currentSum = (uint)numbers.Select(x => (int)x).Sum();
        return totalSum - currentSum;
    }




}
