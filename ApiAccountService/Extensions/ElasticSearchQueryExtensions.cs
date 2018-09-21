using ApiAccountService.Models;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;

namespace ApiAccountService.Extensions
{
    /// <summary>
    /// 
    /// </summary>
    public static class ElasticSearchQueryExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static string GetHashValue(this ElasticSearchQuery data)
        {
            var hashValue = string.Join("", MD5.Create().ComputeHash(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(data))).Select(s => s.ToString("x2")));
            return hashValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public static string GetHashValueWithMoreParams(this ElasticSearchQuery data, dynamic[] param)
        {
            var hashValue = string.Join("", MD5.Create().ComputeHash(Encoding.ASCII.GetBytes(JsonConvert.SerializeObject(data) + JsonConvert.SerializeObject(param))).Select(s => s.ToString("x2")));
            return hashValue;
        }

        private static readonly Object locker = new Object();

        private static byte[] ObjectToByteArray(Object objectToSerialize)
        {
            MemoryStream fs = new MemoryStream();
            BinaryFormatter formatter = new BinaryFormatter();
            try
            {
                //Here's the core functionality! One Line!
                //To be thread-safe we lock the object
                lock (locker)
                {
                    formatter.Serialize(fs, objectToSerialize);
                }
                return fs.ToArray();
            }
            catch (SerializationException se)
            {
                Console.WriteLine("Error occurred during serialization. Message: " +
                se.Message);
                return null;
            }
            finally
            {
                fs.Close();
            }
        }
    }
}
