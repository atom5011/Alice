using UnityEngine;
using System;
using System.IO;
using System.Text;

public class JsonHelper
{
    ///<summary>
    ///JSONファイルをstringで読み込みます
    ///</summary>
    ///<param name=="filePath">streamingAssetsフォルダからのパス</param>
    ///<param name="fileName">ファイル名</param>
    ///<returns>jsonのstringデータ</returns>
    public static String GetJsonFile(String filePath, String fileName)
    {
        string fileText = "";

        //Jsonファイルを読み込む
        FileInfo fi = new FileInfo(Application.streamingAssetsPath + filePath + fileName);
        try
        {
            //一行毎読み込み
            using(StreamReader sr = new StreamReader(fi.OpenRead(), Encoding.UTF8))
            {
                fileText = sr.ReadToEnd();
            }
        }
        catch(Exception e)
        {
            //改行コード
            fileText += e + "\n";
        }

        return fileText;
    }
}