namespace Hawqs
{
    public static class HawqsDataProcessor
    {
        public static ProcessedHawqsData ProcessData(HawqsData hawqsData)
        {
            // HUC14_initiated_flow = OutflowHUC14 - InflowHUC14
            // COMID_flowHUC14 = HUC14_initiated_flow * (HUC14_DrainageCOMID / HUC14_Area)
            // If on mainstem, COMID_flowTotal = COMID_flowHUC14 + InflowHUC14
            ProcessedHawqsData processedHawqsData = new ProcessedHawqsData();
            processedHawqsData.processedData = hawqsData;
            return processedHawqsData;
        }
    }
}