using IdentityModel.Client;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ApiAccountService.Controllers
{
    /// <summary>
    /// summary for TokenController
    /// </summary>
    [Route("api/accounts/token")]
    public class TokenController : Controller
    {
        /// <summary>
        /// get token
        /// </summary>
        /// <returns>token</returns>
        /// <response code="200">returns token</response>
        [HttpGet]
        [ProducesResponseType(typeof(string), 200)]
        public async Task<string> Get()
        {
            var tokenClient = new TokenClient($"{Environment.GetEnvironmentVariable("IS_SERVER")}/connect/token", "client", "secret");
            var tokenResponse = await tokenClient.RequestClientCredentialsAsync("api");
            return tokenResponse.AccessToken;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string EncryptString(string text)
        {
            var key = Encoding.UTF8.GetBytes("E546C8DF278CD5931069B522E695D4F2");

            using (var aesAlg = Aes.Create())
            {
                using (var encryptor = aesAlg.CreateEncryptor(key, aesAlg.IV))
                {
                    using (var msEncrypt = new MemoryStream())
                    {
                        using (var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                        using (var swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(text);
                        }

                        var iv = aesAlg.IV;

                        var decryptedContent = msEncrypt.ToArray();

                        var result = new byte[iv.Length + decryptedContent.Length];

                        Buffer.BlockCopy(iv, 0, result, 0, iv.Length);
                        Buffer.BlockCopy(decryptedContent, 0, result, iv.Length, decryptedContent.Length);

                        return Convert.ToBase64String(result);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cipherText"></param>
        /// <returns></returns>
        public static string DecryptString(string cipherText)
        {
            var fullCipher = Convert.FromBase64String(cipherText);

            var iv = new byte[16];
            var cipher = new byte[16];

            Buffer.BlockCopy(fullCipher, 0, iv, 0, iv.Length);
            Buffer.BlockCopy(fullCipher, iv.Length, cipher, 0, iv.Length);
            var key = Encoding.UTF8.GetBytes("E546C8DF278CD5931069B522E695D4F2");

            using (var aesAlg = Aes.Create())
            {
                using (var decryptor = aesAlg.CreateDecryptor(key, iv))
                {
                    string result;
                    using (var msDecrypt = new MemoryStream(cipher))
                    {
                        using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {
                            using (var srDecrypt = new StreamReader(csDecrypt))
                            {
                                result = srDecrypt.ReadToEnd();
                            }
                        }
                    }

                    return result;
                }
            }
        }

        /// <summary>
        /// encrypt text
        /// </summary>
        /// <param name="plainText">text encrypt</param>
        /// <returns>string</returns>
        /// <response code="200">returns string</response>
        [HttpGet]
        [Route("encrypt")]
        [ProducesResponseType(typeof(string), 200)]
        public string Encrypt([FromQuery]string plainText)
        {
            var data = EncryptString(plainText);
            return data;
        }

        /// <summary>
        /// decrypt text
        /// </summary>
        /// <param name="encryptedText">text decrypt</param>
        /// <returns>return string</returns>
        /// <response code="200">returns string</response>
        [HttpGet]
        [Route("decrypt")]
        [ProducesResponseType(typeof(string), 200)]
        public string Decrypt([FromQuery]string encryptedText)
        {
            var data = DecryptString(encryptedText);
            return data;
        }
    }
}
