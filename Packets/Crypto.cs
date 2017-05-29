using System;
using System.Collections.Generic;
using System.Text;
using System.Security.Cryptography;

namespace D2NG.Tools
{
    public class Crypto
    {
        #region md5Hashing
        public static string md5Hashing(string data)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                return GetMd5Hash(md5Hash, data);
            }
        }
        static string GetMd5Hash(MD5 md5Hash, string input)
        {
            byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder sBuilder = new StringBuilder();
            for (int i = 0; i < data.Length; i++)
            {
                sBuilder.Append(data[i].ToString("x2"));
            }
            return sBuilder.ToString();
        }

        #endregion
        public static string GenKey
        {
            get
            {
                System.Random rng = new System.Random();
                string buff = md5Hashing(rng.Next(100000000, Int32.MaxValue).ToString()) + md5Hashing(rng.Next(100000000, Int32.MaxValue).ToString()) +
                    md5Hashing(rng.Next(100000000, Int32.MaxValue).ToString()) + md5Hashing(rng.Next(100000000, Int32.MaxValue).ToString());
                return buff;
            }
        }
    }
}
