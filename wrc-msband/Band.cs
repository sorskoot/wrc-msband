using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Band;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using System.Threading.Tasks;

namespace TimmyTools
{
    public sealed class BandInfo
    {
        public int ConnectionType { get; set; }
        public string Name { get; set; }
    }
    public sealed class SensorReading
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
        public DateTimeOffset Timestamp { get; set; }
    }

    public sealed class Band : IDisposable
    {
        IBandClient connection;

        public event EventHandler<object> AccelerometerChanged;
        public event EventHandler<object> StatusChanged;

        /// <summary>
        /// Get a list of bands that are currently paired
        /// </summary>
        /// <returns>a list of bands that are currently paired</returns>
        public IAsyncOperation<IList<BandInfo>> GetPaired()
        {
            return AsyncInfo.Run<IList<BandInfo>>((token) =>

                 Task.Run<IList<BandInfo>>(async () =>
                {
                    IBandInfo[] bandInfo = await BandClientManager.Instance.GetBandsAsync();

                    return bandInfo.Select(b => new BandInfo()
                    {
                        ConnectionType = (int)b.ConnectionType,
                        Name = b.Name
                    }).ToList();
                }));
        }

        public IAsyncOperation<bool> Connect(string name)
        {
            return AsyncInfo.Run<bool>((token) =>

                 Task.Run<bool>(async () =>
                 {
                     var paired = await BandClientManager.Instance.GetBandsAsync();
                     var band = paired.FirstOrDefault(x => x.Name == name);
                     if (band != null)
                     {
                         StatusChanged?.Invoke(this, "connecting");
                         this.connection = await BandClientManager.Instance.ConnectAsync(band);
                         StatusChanged?.Invoke(this, "connected");
                         connection.SensorManager.Accelerometer.ReadingChanged += (s, args) =>
                         {
                             if (AccelerometerChanged != null)
                             {
                                 AccelerometerChanged(this, new SensorReading()
                                 {
                                     X = args.SensorReading.AccelerationX,
                                     Y = args.SensorReading.AccelerationY,
                                     Z = args.SensorReading.AccelerationZ,
                                     Timestamp = args.SensorReading.Timestamp
                                 });
                             }
                         };
                         await connection.SensorManager.Accelerometer.StartReadingsAsync();
                     }
                     return true;
                 }));
        }

        public void Dispose()
        {
            if (connection != null)
            {
                connection.Dispose();
            }
        }

    }
}
