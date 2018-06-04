using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Data.Source
{
    public class PRISMData {
        public class Metainfo
        {
            public string status { get; set; }
            public string suid { get; set; }
            public string cloud_node { get; set; }
            public string request_ip { get; set; }
            public string service_url { get; set; }
            public string tstamp { get; set; }
            public int cpu_time { get; set; }
            public string expiration_date { get; set; }
        }

        public class Parameter
        {
            public string name { get; set; }
            public object value { get; set; }
        }

        public class Value
        {
            public string cell_index { get; set; }
            public string center_of_cell { get; set; }
            public List<List<object>> data { get; set; }
            public string name { get; set; }
        }

        public class Result
        {
            public string name { get; set; }
            public List<Value> value { get; set; }
        }

        public class PRISM
        {
            public Metainfo metainfo { get; set; }
            public List<Parameter> parameter { get; set; }
            public List<Result> result { get; set; }

        }
    }


    public class PRISM
    {

        public string GetData(out string errorMsg, string dataset, ITimeSeriesInput componentInput)
        {
            errorMsg = "";
            string url = componentInput.BaseURL[0];
            string parameters = ConstructParameterString(out errorMsg, dataset, componentInput);
            if (errorMsg.Contains("ERROR")) { return null; }

            string data = DownloadData(url, parameters).Result;
            if (data.Contains("ERROR")) { errorMsg = data; return null; }

            // TESTING
            // string data = "{\n  \"metainfo\": {\n    \"status\": \"Finished\",\n    \"suid\": \"5a2cc2cd-64f8-11e8-b880-795c8873a957\",\n    \"cloud_node\": \"10.1.104.29\",\n    \"request_ip\": \"73.118.95.127\",\n    \"service_url\": \"http://csip.engr.colostate.edu:8083/csip-climate/m/prism/1.0\",\n    \"csip-climate.version\": \"$version: 3.0.5 58c98b472106 2018-05-21 Jakob23, built at 2018-05-21 11:02 by jenkins$\",\n    \"csip.version\": \"$version: 2.2.15 df7e0a422edd 2018-03-29 lyaege, built at 2018-05-21 11:02 by jenkins$\",\n    \"tstamp\": \"2018-05-31 11:30:46\",\n    \"cpu_time\": 20,\n    \"expiration_date\": \"2018-05-31 11:31:16\"\n  },\n  \"parameter\": [\n    {\n      \"name\": \"input_zone_features\",\n      \"value\": {\n        \"type\": \"FeatureCollection\",\n        \"features\": [{\n          \"type\": \"Feature\",\n          \"properties\": {\n            \"name\": \"pt one\",\n            \"gid\": 1\n          },\n          \"geometry\": {\n            \"type\": \"Point\",\n            \"coordinates\": [\n              -83.355723,\n              33.925673\n            ],\n            \"crs\": {\n              \"type\": \"name\",\n              \"properties\": {\"name\": \"EPSG:4326\"}\n            }\n          }\n        }]\n      }\n    },\n    {\n      \"name\": \"units\",\n      \"value\": \"metric\"\n    },\n    {\n      \"name\": \"climate_data\",\n      \"value\": [\"ppt\"]\n    },\n    {\n      \"name\": \"start_date\",\n      \"value\": \"2015-01-01\"\n    },\n    {\n      \"name\": \"end_date\",\n      \"value\": \"2015-12-31\"\n    }\n  ],\n  \"result\": [{\n    \"name\": \"output\",\n    \"value\": [{\n      \"cell_index\": \"[999, 384]\",\n      \"center_of_cell\": \"[[-83.37499999999666, 33.91666666666742]]\",\n      \"data\": [\n        [\n          \"date\",\n          \"ppt (mm)\"\n        ],\n        [\n          \"2015-01-01\",\n          0\n        ],\n        [\n          \"2015-01-02\",\n          1.46\n        ],\n        [\n          \"2015-01-03\",\n          15.35\n        ],\n        [\n          \"2015-01-04\",\n          17.16\n        ],\n        [\n          \"2015-01-05\",\n          12.98\n        ],\n        [\n          \"2015-01-06\",\n          0\n        ],\n        [\n          \"2015-01-07\",\n          0\n        ],\n        [\n          \"2015-01-08\",\n          0\n        ],\n        [\n          \"2015-01-09\",\n          0\n        ],\n        [\n          \"2015-01-10\",\n          0\n        ],\n        [\n          \"2015-01-11\",\n          0\n        ],\n        [\n          \"2015-01-12\",\n          3.99\n        ],\n        [\n          \"2015-01-13\",\n          1.19\n        ],\n        [\n          \"2015-01-14\",\n          1.62\n        ],\n        [\n          \"2015-01-15\",\n          0\n        ],\n        [\n          \"2015-01-16\",\n          2.86\n        ],\n        [\n          \"2015-01-17\",\n          0\n        ],\n        [\n          \"2015-01-18\",\n          0\n        ],\n        [\n          \"2015-01-19\",\n          0\n        ],\n        [\n          \"2015-01-20\",\n          0\n        ],\n        [\n          \"2015-01-21\",\n          0\n        ],\n        [\n          \"2015-01-22\",\n          0\n        ],\n        [\n          \"2015-01-23\",\n          7.17\n        ],\n        [\n          \"2015-01-24\",\n          16.37\n        ],\n        [\n          \"2015-01-25\",\n          0\n        ],\n        [\n          \"2015-01-26\",\n          0.13\n        ],\n        [\n          \"2015-01-27\",\n          0\n        ],\n        [\n          \"2015-01-28\",\n          0\n        ],\n        [\n          \"2015-01-29\",\n          0\n        ],\n        [\n          \"2015-01-30\",\n          0\n        ],\n        [\n          \"2015-01-31\",\n          0\n        ],\n        [\n          \"2015-02-01\",\n          0\n        ],\n        [\n          \"2015-02-02\",\n          16.7\n        ],\n        [\n          \"2015-02-03\",\n          0\n        ],\n        [\n          \"2015-02-04\",\n          0\n        ],\n        [\n          \"2015-02-05\",\n          0\n        ],\n        [\n          \"2015-02-06\",\n          0\n        ],\n        [\n          \"2015-02-07\",\n          0\n        ],\n        [\n          \"2015-02-08\",\n          0\n        ],\n        [\n          \"2015-02-09\",\n          0.45\n        ],\n        [\n          \"2015-02-10\",\n          12.75\n        ],\n        [\n          \"2015-02-11\",\n          0\n        ],\n        [\n          \"2015-02-12\",\n          0\n        ],\n        [\n          \"2015-02-13\",\n          0\n        ],\n        [\n          \"2015-02-14\",\n          0\n        ],\n        [\n          \"2015-02-15\",\n          0\n        ],\n        [\n          \"2015-02-16\",\n          0\n        ],\n        [\n          \"2015-02-17\",\n          30.79\n        ],\n        [\n          \"2015-02-18\",\n          0\n        ],\n        [\n          \"2015-02-19\",\n          0\n        ],\n        [\n          \"2015-02-20\",\n          0\n        ],\n        [\n          \"2015-02-21\",\n          0\n        ],\n        [\n          \"2015-02-22\",\n          16.82\n        ],\n        [\n          \"2015-02-23\",\n          10.52\n        ],\n        [\n          \"2015-02-24\",\n          1.81\n        ],\n        [\n          \"2015-02-25\",\n          2.63\n        ],\n        [\n          \"2015-02-26\",\n          17.07\n        ],\n        [\n          \"2015-02-27\",\n          0\n        ],\n        [\n          \"2015-02-28\",\n          0\n        ],\n        [\n          \"2015-03-01\",\n          2.77\n        ],\n        [\n          \"2015-03-02\",\n          6.12\n        ],\n        [\n          \"2015-03-03\",\n          0\n        ],\n        [\n          \"2015-03-04\",\n          5.04\n        ],\n        [\n          \"2015-03-05\",\n          0.61\n        ],\n        [\n          \"2015-03-06\",\n          3.34\n        ],\n        [\n          \"2015-03-07\",\n          0\n        ],\n        [\n          \"2015-03-08\",\n          0\n        ],\n        [\n          \"2015-03-09\",\n          0\n        ],\n        [\n          \"2015-03-10\",\n          0\n        ],\n        [\n          \"2015-03-11\",\n          0\n        ],\n        [\n          \"2015-03-12\",\n          0.39\n        ],\n        [\n          \"2015-03-13\",\n          0.75\n        ],\n        [\n          \"2015-03-14\",\n          15.78\n        ],\n        [\n          \"2015-03-15\",\n          0.69\n        ],\n        [\n          \"2015-03-16\",\n          0\n        ],\n        [\n          \"2015-03-17\",\n          0\n        ],\n        [\n          \"2015-03-18\",\n          0\n        ],\n        [\n          \"2015-03-19\",\n          8.78\n        ],\n        [\n          \"2015-03-20\",\n          13.59\n        ],\n        [\n          \"2015-03-21\",\n          0\n        ],\n        [\n          \"2015-03-22\",\n          2.21\n        ],\n        [\n          \"2015-03-23\",\n          16.39\n        ],\n        [\n          \"2015-03-24\",\n          0\n        ],\n        [\n          \"2015-03-25\",\n          0\n        ],\n        [\n          \"2015-03-26\",\n          0.67\n        ],\n        [\n          \"2015-03-27\",\n          0\n        ],\n        [\n          \"2015-03-28\",\n          0\n        ],\n        [\n          \"2015-03-29\",\n          0\n        ],\n        [\n          \"2015-03-30\",\n          4.23\n        ],\n        [\n          \"2015-03-31\",\n          1.54\n        ],\n        [\n          \"2015-04-01\",\n          0.25\n        ],\n        [\n          \"2015-04-02\",\n          0\n        ],\n        [\n          \"2015-04-03\",\n          3.81\n        ],\n        [\n          \"2015-04-04\",\n          5.81\n        ],\n        [\n          \"2015-04-05\",\n          0\n        ],\n        [\n          \"2015-04-06\",\n          0\n        ],\n        [\n          \"2015-04-07\",\n          13.75\n        ],\n        [\n          \"2015-04-08\",\n          0\n        ],\n        [\n          \"2015-04-09\",\n          0\n        ],\n        [\n          \"2015-04-10\",\n          0\n        ],\n        [\n          \"2015-04-11\",\n          8.87\n        ],\n        [\n          \"2015-04-12\",\n          0\n        ],\n        [\n          \"2015-04-13\",\n          0.85\n        ],\n        [\n          \"2015-04-14\",\n          44.98\n        ],\n        [\n          \"2015-04-15\",\n          2.89\n        ],\n        [\n          \"2015-04-16\",\n          33.07\n        ],\n        [\n          \"2015-04-17\",\n          1.02\n        ],\n        [\n          \"2015-04-18\",\n          22.27\n        ],\n        [\n          \"2015-04-19\",\n          7.81\n        ],\n        [\n          \"2015-04-20\",\n          31.28\n        ],\n        [\n          \"2015-04-21\",\n          3.73\n        ],\n        [\n          \"2015-04-22\",\n          0\n        ],\n        [\n          \"2015-04-23\",\n          0\n        ],\n        [\n          \"2015-04-24\",\n          0\n        ],\n        [\n          \"2015-04-25\",\n          6.52\n        ],\n        [\n          \"2015-04-26\",\n          7.95\n        ],\n        [\n          \"2015-04-27\",\n          0\n        ],\n        [\n          \"2015-04-28\",\n          0\n        ],\n        [\n          \"2015-04-29\",\n          8.46\n        ],\n        [\n          \"2015-04-30\",\n          10.28\n        ],\n        [\n          \"2015-05-01\",\n          0\n        ],\n        [\n          \"2015-05-02\",\n          0\n        ],\n        [\n          \"2015-05-03\",\n          0\n        ],\n        [\n          \"2015-05-04\",\n          0\n        ],\n        [\n          \"2015-05-05\",\n          0\n        ],\n        [\n          \"2015-05-06\",\n          0\n        ],\n        [\n          \"2015-05-07\",\n          0\n        ],\n        [\n          \"2015-05-08\",\n          0\n        ],\n        [\n          \"2015-05-09\",\n          0\n        ],\n        [\n          \"2015-05-10\",\n          0\n        ],\n        [\n          \"2015-05-11\",\n          0\n        ],\n        [\n          \"2015-05-12\",\n          0\n        ],\n        [\n          \"2015-05-13\",\n          0\n        ],\n        [\n          \"2015-05-14\",\n          0\n        ],\n        [\n          \"2015-05-15\",\n          0\n        ],\n        [\n          \"2015-05-16\",\n          0\n        ],\n        [\n          \"2015-05-17\",\n          0\n        ],\n        [\n          \"2015-05-18\",\n          0\n        ],\n        [\n          \"2015-05-19\",\n          3.8\n        ],\n        [\n          \"2015-05-20\",\n          0\n        ],\n        [\n          \"2015-05-21\",\n          0.65\n        ],\n        [\n          \"2015-05-22\",\n          0\n        ],\n        [\n          \"2015-05-23\",\n          0\n        ],\n        [\n          \"2015-05-24\",\n          0\n        ],\n        [\n          \"2015-05-25\",\n          0\n        ],\n        [\n          \"2015-05-26\",\n          3.17\n        ],\n        [\n          \"2015-05-27\",\n          18.25\n        ],\n        [\n          \"2015-05-28\",\n          0.25\n        ],\n        [\n          \"2015-05-29\",\n          3.23\n        ],\n        [\n          \"2015-05-30\",\n          0\n        ],\n        [\n          \"2015-05-31\",\n          0\n        ],\n        [\n          \"2015-06-01\",\n          8.94\n        ],\n        [\n          \"2015-06-02\",\n          5.55\n        ],\n        [\n          \"2015-06-03\",\n          22.01\n        ],\n        [\n          \"2015-06-04\",\n          14.77\n        ],\n        [\n          \"2015-06-05\",\n          0\n        ],\n        [\n          \"2015-06-06\",\n          0\n        ],\n        [\n          \"2015-06-07\",\n          0\n        ],\n        [\n          \"2015-06-08\",\n          0\n        ],\n        [\n          \"2015-06-09\",\n          2.56\n        ],\n        [\n          \"2015-06-10\",\n          10.57\n        ],\n        [\n          \"2015-06-11\",\n          0\n        ],\n        [\n          \"2015-06-12\",\n          2.73\n        ],\n        [\n          \"2015-06-13\",\n          0.03\n        ],\n        [\n          \"2015-06-14\",\n          0\n        ],\n        [\n          \"2015-06-15\",\n          0\n        ],\n        [\n          \"2015-06-16\",\n          0.05\n        ],\n        [\n          \"2015-06-17\",\n          0\n        ],\n        [\n          \"2015-06-18\",\n          0\n        ],\n        [\n          \"2015-06-19\",\n          4.77\n        ],\n        [\n          \"2015-06-20\",\n          0\n        ],\n        [\n          \"2015-06-21\",\n          7.94\n        ],\n        [\n          \"2015-06-22\",\n          0\n        ],\n        [\n          \"2015-06-23\",\n          0\n        ],\n        [\n          \"2015-06-24\",\n          0\n        ],\n        [\n          \"2015-06-25\",\n          0.36\n        ],\n        [\n          \"2015-06-26\",\n          0\n        ],\n        [\n          \"2015-06-27\",\n          0\n        ],\n        [\n          \"2015-06-28\",\n          12.88\n        ],\n        [\n          \"2015-06-29\",\n          0\n        ],\n        [\n          \"2015-06-30\",\n          0\n        ],\n        [\n          \"2015-07-01\",\n          20.84\n        ],\n        [\n          \"2015-07-02\",\n          8.92\n        ],\n        [\n          \"2015-07-03\",\n          32.74\n        ],\n        [\n          \"2015-07-04\",\n          32.64\n        ],\n        [\n          \"2015-07-05\",\n          5.23\n        ],\n        [\n          \"2015-07-06\",\n          0\n        ],\n        [\n          \"2015-07-07\",\n          0\n        ],\n        [\n          \"2015-07-08\",\n          0\n        ],\n        [\n          \"2015-07-09\",\n          0\n        ],\n        [\n          \"2015-07-10\",\n          0\n        ],\n        [\n          \"2015-07-11\",\n          0\n        ],\n        [\n          \"2015-07-12\",\n          0\n        ],\n        [\n          \"2015-07-13\",\n          0.4\n        ],\n        [\n          \"2015-07-14\",\n          0\n        ],\n        [\n          \"2015-07-15\",\n          27.08\n        ],\n        [\n          \"2015-07-16\",\n          0\n        ],\n        [\n          \"2015-07-17\",\n          0\n        ],\n        [\n          \"2015-07-18\",\n          0\n        ],\n        [\n          \"2015-07-19\",\n          0\n        ],\n        [\n          \"2015-07-20\",\n          0\n        ],\n        [\n          \"2015-07-21\",\n          2.66\n        ],\n        [\n          \"2015-07-22\",\n          6.43\n        ],\n        [\n          \"2015-07-23\",\n          0\n        ],\n        [\n          \"2015-07-24\",\n          0.99\n        ],\n        [\n          \"2015-07-25\",\n          0\n        ],\n        [\n          \"2015-07-26\",\n          0\n        ],\n        [\n          \"2015-07-27\",\n          0\n        ],\n        [\n          \"2015-07-28\",\n          0\n        ],\n        [\n          \"2015-07-29\",\n          0\n        ],\n        [\n          \"2015-07-30\",\n          0\n        ],\n        [\n          \"2015-07-31\",\n          0\n        ],\n        [\n          \"2015-08-01\",\n          0\n        ],\n        [\n          \"2015-08-02\",\n          0.53\n        ],\n        [\n          \"2015-08-03\",\n          0\n        ],\n        [\n          \"2015-08-04\",\n          0\n        ],\n        [\n          \"2015-08-05\",\n          0\n        ],\n        [\n          \"2015-08-06\",\n          0\n        ],\n        [\n          \"2015-08-07\",\n          19.63\n        ],\n        [\n          \"2015-08-08\",\n          0\n        ],\n        [\n          \"2015-08-09\",\n          0\n        ],\n        [\n          \"2015-08-10\",\n          0\n        ],\n        [\n          \"2015-08-11\",\n          53.13\n        ],\n        [\n          \"2015-08-12\",\n          0.88\n        ],\n        [\n          \"2015-08-13\",\n          0\n        ],\n        [\n          \"2015-08-14\",\n          0\n        ],\n        [\n          \"2015-08-15\",\n          0\n        ],\n        [\n          \"2015-08-16\",\n          0\n        ],\n        [\n          \"2015-08-17\",\n          0.05\n        ],\n        [\n          \"2015-08-18\",\n          49.36\n        ],\n        [\n          \"2015-08-19\",\n          11.53\n        ],\n        [\n          \"2015-08-20\",\n          29.77\n        ],\n        [\n          \"2015-08-21\",\n          0.52\n        ],\n        [\n          \"2015-08-22\",\n          0.45\n        ],\n        [\n          \"2015-08-23\",\n          5.85\n        ],\n        [\n          \"2015-08-24\",\n          10.25\n        ],\n        [\n          \"2015-08-25\",\n          0\n        ],\n        [\n          \"2015-08-26\",\n          0\n        ],\n        [\n          \"2015-08-27\",\n          0\n        ],\n        [\n          \"2015-08-28\",\n          0\n        ],\n        [\n          \"2015-08-29\",\n          0\n        ],\n        [\n          \"2015-08-30\",\n          0\n        ],\n        [\n          \"2015-08-31\",\n          11.74\n        ],\n        [\n          \"2015-09-01\",\n          0\n        ],\n        [\n          \"2015-09-02\",\n          0\n        ],\n        [\n          \"2015-09-03\",\n          0\n        ],\n        [\n          \"2015-09-04\",\n          0\n        ],\n        [\n          \"2015-09-05\",\n          1.02\n        ],\n        [\n          \"2015-09-06\",\n          4.62\n        ],\n        [\n          \"2015-09-07\",\n          0\n        ],\n        [\n          \"2015-09-08\",\n          0\n        ],\n        [\n          \"2015-09-09\",\n          17.53\n        ],\n        [\n          \"2015-09-10\",\n          20.18\n        ],\n        [\n          \"2015-09-11\",\n          11.44\n        ],\n        [\n          \"2015-09-12\",\n          0\n        ],\n        [\n          \"2015-09-13\",\n          0\n        ],\n        [\n          \"2015-09-14\",\n          0\n        ],\n        [\n          \"2015-09-15\",\n          0\n        ],\n        [\n          \"2015-09-16\",\n          0\n        ],\n        [\n          \"2015-09-17\",\n          0\n        ],\n        [\n          \"2015-09-18\",\n          0\n        ],\n        [\n          \"2015-09-19\",\n          0\n        ],\n        [\n          \"2015-09-20\",\n          0\n        ],\n        [\n          \"2015-09-21\",\n          0\n        ],\n        [\n          \"2015-09-22\",\n          0.04\n        ],\n        [\n          \"2015-09-23\",\n          0\n        ],\n        [\n          \"2015-09-24\",\n          0\n        ],\n        [\n          \"2015-09-25\",\n          37.71\n        ],\n        [\n          \"2015-09-26\",\n          5.51\n        ],\n        [\n          \"2015-09-27\",\n          2.36\n        ],\n        [\n          \"2015-09-28\",\n          0.58\n        ],\n        [\n          \"2015-09-29\",\n          6.44\n        ],\n        [\n          \"2015-09-30\",\n          6.53\n        ],\n        [\n          \"2015-10-01\",\n          1.11\n        ],\n        [\n          \"2015-10-02\",\n          15.44\n        ],\n        [\n          \"2015-10-03\",\n          10.35\n        ],\n        [\n          \"2015-10-04\",\n          55.18\n        ],\n        [\n          \"2015-10-05\",\n          0.35\n        ],\n        [\n          \"2015-10-06\",\n          3.58\n        ],\n        [\n          \"2015-10-07\",\n          0\n        ],\n        [\n          \"2015-10-08\",\n          0\n        ],\n        [\n          \"2015-10-09\",\n          0\n        ],\n        [\n          \"2015-10-10\",\n          19.89\n        ],\n        [\n          \"2015-10-11\",\n          12.06\n        ],\n        [\n          \"2015-10-12\",\n          0\n        ],\n        [\n          \"2015-10-13\",\n          4.74\n        ],\n        [\n          \"2015-10-14\",\n          0\n        ],\n        [\n          \"2015-10-15\",\n          0\n        ],\n        [\n          \"2015-10-16\",\n          0\n        ],\n        [\n          \"2015-10-17\",\n          0\n        ],\n        [\n          \"2015-10-18\",\n          0\n        ],\n        [\n          \"2015-10-19\",\n          0\n        ],\n        [\n          \"2015-10-20\",\n          0\n        ],\n        [\n          \"2015-10-21\",\n          0\n        ],\n        [\n          \"2015-10-22\",\n          0\n        ],\n        [\n          \"2015-10-23\",\n          0\n        ],\n        [\n          \"2015-10-24\",\n          0\n        ],\n        [\n          \"2015-10-25\",\n          0\n        ],\n        [\n          \"2015-10-26\",\n          2.59\n        ],\n        [\n          \"2015-10-27\",\n          14.49\n        ],\n        [\n          \"2015-10-28\",\n          9.41\n        ],\n        [\n          \"2015-10-29\",\n          2.87\n        ],\n        [\n          \"2015-10-30\",\n          0\n        ],\n        [\n          \"2015-10-31\",\n          0\n        ],\n        [\n          \"2015-11-01\",\n          18.41\n        ],\n        [\n          \"2015-11-02\",\n          42.65\n        ],\n        [\n          \"2015-11-03\",\n          28.66\n        ],\n        [\n          \"2015-11-04\",\n          1.21\n        ],\n        [\n          \"2015-11-05\",\n          1.48\n        ],\n        [\n          \"2015-11-06\",\n          8.78\n        ],\n        [\n          \"2015-11-07\",\n          20.47\n        ],\n        [\n          \"2015-11-08\",\n          27.51\n        ],\n        [\n          \"2015-11-09\",\n          39.31\n        ],\n        [\n          \"2015-11-10\",\n          4.64\n        ],\n        [\n          \"2015-11-11\",\n          0\n        ],\n        [\n          \"2015-11-12\",\n          0\n        ],\n        [\n          \"2015-11-13\",\n          0\n        ],\n        [\n          \"2015-11-14\",\n          0\n        ],\n        [\n          \"2015-11-15\",\n          0\n        ],\n        [\n          \"2015-11-16\",\n          0\n        ],\n        [\n          \"2015-11-17\",\n          0\n        ],\n        [\n          \"2015-11-18\",\n          4.74\n        ],\n        [\n          \"2015-11-19\",\n          44.68\n        ],\n        [\n          \"2015-11-20\",\n          0\n        ],\n        [\n          \"2015-11-21\",\n          0\n        ],\n        [\n          \"2015-11-22\",\n          0\n        ],\n        [\n          \"2015-11-23\",\n          0\n        ],\n        [\n          \"2015-11-24\",\n          0\n        ],\n        [\n          \"2015-11-25\",\n          0\n        ],\n        [\n          \"2015-11-26\",\n          0\n        ],\n        [\n          \"2015-11-27\",\n          0\n        ],\n        [\n          \"2015-11-28\",\n          0\n        ],\n        [\n          \"2015-11-29\",\n          0\n        ],\n        [\n          \"2015-11-30\",\n          0\n        ],\n        [\n          \"2015-12-01\",\n          0\n        ],\n        [\n          \"2015-12-02\",\n          1.02\n        ],\n        [\n          \"2015-12-03\",\n          0.67\n        ],\n        [\n          \"2015-12-04\",\n          0\n        ],\n        [\n          \"2015-12-05\",\n          0\n        ],\n        [\n          \"2015-12-06\",\n          0\n        ],\n        [\n          \"2015-12-07\",\n          0\n        ],\n        [\n          \"2015-12-08\",\n          0\n        ],\n        [\n          \"2015-12-09\",\n          0\n        ],\n        [\n          \"2015-12-10\",\n          0\n        ],\n        [\n          \"2015-12-11\",\n          0\n        ],\n        [\n          \"2015-12-12\",\n          0\n        ],\n        [\n          \"2015-12-13\",\n          0\n        ],\n        [\n          \"2015-12-14\",\n          0.95\n        ],\n        [\n          \"2015-12-15\",\n          2.89\n        ],\n        [\n          \"2015-12-16\",\n          0\n        ],\n        [\n          \"2015-12-17\",\n          46.82\n        ],\n        [\n          \"2015-12-18\",\n          12.78\n        ],\n        [\n          \"2015-12-19\",\n          0\n        ],\n        [\n          \"2015-12-20\",\n          0\n        ],\n        [\n          \"2015-12-21\",\n          0\n        ],\n        [\n          \"2015-12-22\",\n          50.66\n        ],\n        [\n          \"2015-12-23\",\n          4.73\n        ],\n        [\n          \"2015-12-24\",\n          47.42\n        ],\n        [\n          \"2015-12-25\",\n          32.87\n        ],\n        [\n          \"2015-12-26\",\n          6.33\n        ],\n        [\n          \"2015-12-27\",\n          0.28\n        ],\n        [\n          \"2015-12-28\",\n          3.35\n        ],\n        [\n          \"2015-12-29\",\n          36.11\n        ],\n        [\n          \"2015-12-30\",\n          9.5\n        ],\n        [\n          \"2015-12-31\",\n          66.82\n        ]\n      ],\n      \"name\": \"pt one\"\n    }]\n  }]\n}";
            return data;
        }

        private string ConstructParameterString(out string errorMsg, string dataset, ITimeSeriesInput componentInput)
        {
            errorMsg = "";
            StringBuilder sb = new StringBuilder();
            try
            {
                sb.Append("{'metainfo': { },'parameter': [{'name': 'input_zone_features','value': {'type':'FeatureCollection', 'features':" +
                    "[{'type':'Feature', 'properties':{'name': 'pt one','gid': 1},'geometry':{'type':'Point','coordinates':[");

                // Append point longitude and latitude values to string
                sb.Append(componentInput.Geometry.Point.Longitude.ToString() + ", " + componentInput.Geometry.Point.Latitude.ToString());
                sb.Append("],'crs':{ 'type':'name','properties':{ 'name':'EPSG:4326'} } } } ] } }, { " +
                    "'name': 'units', 'value': 'metric'}, {'name': 'climate_data','value': [");

                // Append dataset
                // Valid dataset: 'ppt', 'tmin', 'tmax', 'tdmean', 'vpdmin', vpdmax'
                sb.Append(dataset);
                sb.Append("] }, {'name': 'start_date','value': ");

                // Append start date
                sb.Append(componentInput.DateTimeSpan.StartDate.ToString("yyyy-MM-dd"));
                sb.Append("}, {'name': 'end_date','value': ");
                // Append end date
                sb.Append(componentInput.DateTimeSpan.EndDate.ToString("yyyy-MM-dd"));
                sb.Append("}]}");
            }
            catch(Exception ex)
            {
                errorMsg = "ERROR: " + ex.Message;
            }
            return sb.ToString();
        }

        private async Task<string> DownloadData(string url, string parameters)
        {
            string data = "";
            try
            {
                // TODO: Read in max retry attempt from config file.
                int retries = 5;

                // Response status message
                string status = "";

                var content = new StringContent(parameters, Encoding.UTF8, "application/json");
                using (var client = new HttpClient())
                {
                    while (retries > 0 && !status.Contains("OK"))
                    {
                        Thread.Sleep(100);
                        var response = await client.PostAsync(url, content);
                        data =  await response.Content.ReadAsStringAsync();
                        status = response.StatusCode.ToString();
                        retries -= 1;
                    }
                }
            }
            catch (Exception ex)
            {
                return "ERROR: Unable to download requested prism data. " + ex.Message;
            }
            return data;
        }

        /// <summary>
        /// Takes the data recieved from prism and sets the ITimeSeries object values.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="dataset"></param>
        /// <param name="component"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public ITimeSeriesOutput SetDataToOutput(out string errorMsg, string dataset, string data, ITimeSeriesOutput output, ITimeSeriesInput input)
        {
            errorMsg = "";
            PRISMData.PRISM content;
            try
            {
                content = Newtonsoft.Json.JsonConvert.DeserializeObject<PRISMData.PRISM>(data);
            }
            catch(Newtonsoft.Json.JsonException ex)
            {
                errorMsg = "PRISM JSON Deserialization Error: " + ex.Message;
                return null;
            }
            output.Dataset = dataset;
            output.DataSource = input.Source;
            output.Metadata = SetMetadata(out errorMsg, content, input, output);
            output.Data = SetData(out errorMsg, content, input.DateTimeSpan.DateTimeFormat, input.DataValueFormat);
            return output;
        }

        /// <summary>
        /// Parses data string from prism and sets the metadata for the ITimeSeries object.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="data"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        private Dictionary<string, string> SetMetadata(out string errorMsg, PRISMData.PRISM content, ITimeSeriesInput input, ITimeSeriesOutput output)
        {
            errorMsg = "";
            Dictionary<string, string> meta = output.Metadata;
            foreach(PropertyInfo p in typeof(PRISMData.Metainfo).GetProperties())
            {
                if (!meta.ContainsKey(p.Name) || !meta.ContainsKey("prism_" + p.Name))
                {
                    meta.Add("prism_" + p.Name, p.GetValue(content.metainfo, null).ToString());
                }
            }

            meta.Add("prism_start_date", input.DateTimeSpan.StartDate.ToString());
            meta.Add("prism_end_date", input.DateTimeSpan.EndDate.ToString());
            meta.Add("prism_point_longitude", input.Geometry.Point.Longitude.ToString());
            meta.Add("prism_point_latitude", input.Geometry.Point.Latitude.ToString());
            if (output.Dataset == "Precipitation") {
                meta.Add("prism_unit", "mm");
            }
            else
            {
                meta.Add("prism_unit", "degC");
            }
            return meta;
        }

        /// <summary>
        /// Parses data string from prism and sets the data for the ITimeSeries object.
        /// </summary>
        /// <param name="errorMsg"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        private Dictionary<string, List<string>> SetData(out string errorMsg, PRISMData.PRISM content, string dateFormat, string dataFormat)
        {
            errorMsg = "";
            Dictionary<string, List<string>> dataDict = new Dictionary<string, List<string>>();
            for (int i = 1; i < content.result[0].value[0].data.Count - 1; i++)
            {
                DateTime.TryParseExact(content.result[0].value[0].data[i][0].ToString(), new string[] { "yyyy-MM-dd" }, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None, out DateTime newDate);
                Double.TryParse(content.result[0].value[0].data[i][1].ToString(), out double dataValue);
                dataDict.Add(newDate.ToString(dateFormat), new List<string> { dataValue.ToString(dataFormat) });
            }
            return dataDict;
        }
    }
}
