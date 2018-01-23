using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

using Savonia.Measurements.Clients.Service20;
using System.Threading.Tasks;

namespace SenPro_Application
{
    /// <summary>
    /// This is a wrapper class that puts the sensor readings into an object
    /// hierarchy that SaMi can read, and uploads it.
    /// </summary>
    class SaMi
    {
        MeasurementsServiceClient client;
        MeasurementPackage package;
        List<MeasurementModel> measurements;
        static string serviceUrlForBasicBinding = "https://sami.savonia.fi/Service/2.0/MeasurementsService.svc/json";
        RestService rest;
        string modelTag;

        /// <summary>
        /// Sets up the required class variables. Take note of the write key: do not change this.
        /// This is the key granted for this project by Miko.
        /// </summary>
        public SaMi(string patientName)
        {
            rest = new RestService();
            package = new MeasurementPackage();
            package.Key = "SK507-2017305DCC94F130F11-01";
            measurements = new List<MeasurementModel>();
            modelTag = patientName;
        }

        /// <summary>
        /// This makes a MeasurementModel 
        /// </summary>
        /// <param name="readingInput"></param>
        public void AddMeasurement(SenProReading readingInput)
        {
            MeasurementModel m = new MeasurementModel()
            {
                Object = "Halax Prototype",
                Tag = modelTag,
                Timestamp = readingInput.DateTime
            };

            DataModel sensorOne = new DataModel()
            {
                Tag = "Sensor 1",
                Value = readingInput.Values[0]
            };

            DataModel sensorTwo = new DataModel()
            {
                Tag = "Sensor 2",
                Value = readingInput.Values[1]
            };

            DataModel sensorThree = new DataModel()
            {
                Tag = "Sensor 3",
                Value = readingInput.Values[2]
            };

            DataModel sensorFour = new DataModel()
            {
                Tag = "Sensor 4",
                Value = readingInput.Values[3]
            };

            DataModel sensorFive = new DataModel()
            {
                Tag = "Sensor 5",
                Value = readingInput.Values[4]
            };

            DataModel sensorSix = new DataModel()
            {
                Tag = "Sensor 6",
                Value = readingInput.Values[5]
            };

            m.Data = new DataModel[6];
            m.Data[0] = sensorOne;
            m.Data[1] = sensorTwo;
            m.Data[2] = sensorThree;
            m.Data[3] = sensorFour;
            m.Data[4] = sensorFive;
            m.Data[5] = sensorSix;

            measurements.Add(m);
        }

        public bool MeasurementMinuteCheck()
        {
            if (measurements.Count >= 60)
            {
                return true;
            }
            return false;
        }

        public bool MeasurementAnyCheck()
        {
            if (measurements.Count >= 2)
            {
                return true;
            }
            return false;
        }

        public async Task SaveResults()
        {
            package.Measurements = measurements.ToArray();
            await rest.SavePackageAsync(package);
            package.Measurements = null;
            measurements.Clear();
        }

        public void ClearPackage()
        {
            package.Measurements = null;
        }

        public async Task TestMeasurementSave()
        {
            List<MeasurementModel> testOutput = new List<MeasurementModel>();
            MeasurementModel m = new MeasurementModel()
            {
                Object = "Halax Prototype Test",
                Tag = "Gas Sensors",
                Timestamp = DateTime.Now
            };

            DataModel sensorOne = new DataModel()
            {
                Tag = "Sensor Test-1",
                Value = 12
            };

            DataModel sensorTwo = new DataModel()
            {
                Tag = "Sensor Test-2",
                Value = 15
            };

            m.Data = new DataModel[2];
            m.Data[0] = sensorOne;
            m.Data[1] = sensorTwo;

            testOutput.Add(m);
            package.Measurements = testOutput.ToArray();

            await rest.SavePackageAsync(package);

        }

    }
}