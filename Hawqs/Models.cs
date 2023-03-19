using System;
using System.Collections.Generic;

using Newtonsoft.Json;

namespace Hawqs
{
    public class HawqsProject
    {
        public string inputData { get; set; }

        public HawqsProject() { }
    }

    public class HawqsInput
    {
        public string name { get; set; }
        public string description { get; set; }
        public List<string> options { get; set; }
        public string defaultValue { get; set; }
        public string type { get; set; }
        public bool required { get; set; }
        public HawqsInputChildParameters[] childParameters { get; set; }
    }

    public class HawqsInputChildParameters
    {
        public string name { get; set; }
        public string description { get; set; }
        public HawqsInputChildParameterOption[] options { get; set; }
        public string defaultValue { get; set; }
        public string type { get; set; }
        public bool required { get; set; }

    }

    public class HawqsInputChildParameterOption
    {
        public string value { get; set; }
        public string description { get; set; }
        public string metricUnits { get; set; }
        public string imperialUnits { get; set; }
    }

    public class HawqsData
    {
        public string projectData { get; set; }

        public HawqsData() { }
    }

    public class ProcessedHawqsData
    {
        public HawqsData processedData { get; set; }

        public ProcessedHawqsData() { }
    }

    public class HawqsSubmitRequest
    {
        public string apiKey { get; set; }
        public string inputData { get; set; }

        public HawqsSubmitRequest() { }
    }

    public class HawqsStatusRequest
    {
        public string apiKey { get; set; }

        public HawqsStatusRequest() { }
    }

    public class HawqsDataRequest
    {
        public string apiKey { get; set; }
        public bool process { get; set; }

        public HawqsDataRequest() { }
    }

    public class HawqsCancelRequest
    {
        public string apiKey { get; set; }

        public HawqsCancelRequest() { }
    }

    public class HawqsDefaultSubmitRequest
    {
        public string apiKey = "YOUR-HAWQS-API-KEY-HERE";
        public string inputData = JsonConvert.SerializeObject(new HawqsDefaultInputData());
        
        public HawqsDefaultSubmitRequest() { }
    }

    public class HawqsDefaultInputData
    {
        public string dataset = "HUC8";
        public string downstreamSubbasin = "07100009";
        public HawqsHruOpts setHrus = new HawqsHruOpts()
        {
            method = "area",
            target = 2,
            units = "km2"
        };
        public string weatherDataset = "PRISM";
        public string startingSimulationDate = "1981-01-01";
        public string endingSimulationDate = "1985-12-31";
        public int warmupYears = 2;
        public string outputPrintScaling = "daily";
        public HawqsReportDataOpts reportData = new HawqsReportDataOpts()
        {
            formats = new List<string>() { "csv", "netcdf" },
            units = "metric",
            outputs = new Dictionary<string, Dictionary<string, String[]>>()
            {
                { "rch", new Dictionary<string, String[]>()
                    {
                        { "statistics", new String[]{ "daily_avg" } }
                    }
                }
            }
        };

        public HawqsDefaultInputData() { }
    }

    public class HawqsHruOpts
    {
        public string method { get; set; }
        public int target { get; set; }
        public string units { get; set; }

        public HawqsHruOpts() { }
    }

    public class HawqsReportDataOpts
    {
        public List<string> formats { get; set; }
        public string units { get; set; }
        public Dictionary<string, Dictionary<string, string[]>> outputs { get; set; }

        public HawqsReportDataOpts() { }
    }

    public class HawqsDefaultStatusRequest
    {
        public string apiKey = "YOUR-HAWQS-API-KEY-HERE";

        public HawqsDefaultStatusRequest() { }
    }

    public class HawqsDefaultDataRequest
    {
        public string apiKey = "YOUR-HAWQS-API-KEY-HERE";
        public bool process = false;

        public HawqsDefaultDataRequest() { }
    }

    public class HawqsDefaultCancelRequest
    {
        public string apiKey = "YOUR-HAWQS-API-KEY-HERE";

        public HawqsDefaultCancelRequest() { }
    }

    public class HawqsStatusResponse
    {
        public HawqsStatus status { get; set; }
        public List<HawqsOutput> output { get; set; }

        public HawqsStatusResponse() { }
    }

    public class HawqsStatus
    {
        public int progress { get; set; }
        public string message { get; set; }
        public string errorStackTrace { get; set; }

        public HawqsStatus() { }
    }

    public class HawqsOutput
    {
        public string name { get; set; }
        public string url { get; set; }
        public string format { get; set; }

        public HawqsOutput() { }
    }

    public class HawqsExampleInputsResponse
    {
        public List<HawqsInput> inputsExample = new List<HawqsInput>()
        {
            { new HawqsInput() 
                {
                    name = "dataset",
                    description = "HAWQS dataset name",
                    options = new List<string>() {
                        { "HUC8" },
                        { "HUC10" },
                        { "HUC12" },
                        { "HUC14" }
                    },
                    defaultValue = "HUC8",
                    type = "string",
                    required = true,
                    childParameters = null
                }
            },
            { new HawqsInput()
                {
                    name = "downstreamSubbasin",
                    description = "The downstream subbasin name...",
                    options = null,
                    defaultValue = null,
                    type = "string",
                    required = true,
                    childParameters = null
                }
            }
        };
    }

    public class HawqsExampleSubmitResponse
    {
        public int id = 123;
        public string url = "https://dev-api.hawqs.tamu.edu/projects/123";
    }

    public class HawqsExampleStatusResponse
    {
        public HawqsStatus status = new HawqsStatus()
        {
            progress = 100,
            message = "Complete.",
            errorStackTrace = null
        };
    }

    public class HawqsExampleDataResponse
    {
        public List<HawqsOutput> output = new List<HawqsOutput>() {
                { new HawqsOutput()
                    {
                        name = "Project input/output files",
                        url = "link-to-file.url",
                        format = "7zip"
                    }
                },
                { new HawqsOutput()
                    {
                        name = "Metadata for daily averages by month (output.rch)",
                        url = "link-to-file.url",
                        format = "csv"
                    }
                },
                { new HawqsOutput()
                    {
                        name = "Daily averages by month (output.rch)",
                        url = "link-to-file.url",
                        format = "csv"
                    }
                },
                { new HawqsOutput()
                    {
                        name = "Daily averages by month (output.rch)",
                        url = "link-to-file.url",
                        format = "netcdf"
                    }
                }
        };
    }

    public class HawqsExampleCancelResponse 
    {
        public string message = "hawqs-api cancel project execution endpoint not yet implemented";    
    }
}