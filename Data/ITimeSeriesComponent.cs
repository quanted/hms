
namespace Data
{
    /// <summary>
    /// Base timeseries parameter object
    /// </summary>
    public interface ITimeSeriesComponent
    {
        /// <summary>
        /// Output variable object for a timeseries component
        /// </summary>
        ITimeSeriesOutput Output { get; set; }

        /// <summary>
        /// Input variable object for a timeseries component.
        /// </summary>
        ITimeSeriesInput Input { get; set; }

    }
}
