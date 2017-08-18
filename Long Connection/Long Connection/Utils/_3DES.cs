using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

class _3DES
{
    static String desKey = "1dcrm4goRY8KODsgV1PPuHLB";//24位密钥
    static String desIv = "QCsJ2SKR"; ///8位向量

    /// <summary>
    ///3DES加密
    /// </summary>
    /// <param name="originalValue">加密数据</param>
    /// <param name="key">24位字符的密钥字符串</param>
    /// <param name="IV">8位字符的初始化向量字符串</param>
    /// <returns></returns>
    public static string DESEncrypt(string originalValue)
    {

        SymmetricAlgorithm sa;
        ICryptoTransform ct;
        MemoryStream ms;
        CryptoStream cs;
        byte[] byt;
        sa = new TripleDESCryptoServiceProvider();
        sa.Key = Encoding.UTF8.GetBytes(desKey);
        sa.IV = Encoding.UTF8.GetBytes(desIv);
        ct = sa.CreateEncryptor();
        byt = Encoding.UTF8.GetBytes(originalValue);
        ms = new MemoryStream();
        cs = new CryptoStream(ms, ct, CryptoStreamMode.Write);
        cs.Write(byt, 0, byt.Length);
        cs.FlushFinalBlock();
        cs.Close();

        return Convert.ToBase64String(ms.ToArray());
    }
    /// <summary>
    /// 3DES解密
    /// </summary>
    /// <param name="data">解密数据</param>
    /// <param name="key">24位字符的密钥字符串(需要和加密时相同)</param>
    /// <param name="iv">8位字符的初始化向量字符串(需要和加密时相同)</param>
    /// <returns></returns>
    public static string DESDecrypst(string data)
    {
        SymmetricAlgorithm mCSP = new TripleDESCryptoServiceProvider();
        mCSP.Key = Encoding.UTF8.GetBytes(desKey);
        mCSP.IV = Encoding.UTF8.GetBytes(desIv);
        ICryptoTransform ct;
        MemoryStream ms;
        CryptoStream cs;
        byte[] byt;
        ct = mCSP.CreateDecryptor(mCSP.Key, mCSP.IV);
        byt = Convert.FromBase64String(data);
        ms = new MemoryStream();
        cs = new CryptoStream(ms, ct, CryptoStreamMode.Write);
        cs.Write(byt, 0, byt.Length);
        cs.FlushFinalBlock();
        cs.Close();

        return Encoding.UTF8.GetString(ms.ToArray());
    }
}
