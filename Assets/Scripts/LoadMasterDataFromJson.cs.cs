
using UnityEngine;

public class LoadMasterDataFromJson
{/// <summary>
/// JsonHelperを利用し、Jsonファイルを読み込んでscenarioMasterDataに代入し、GameDirectorに結果を返す
/// </summary>
/// <returns></returns>
    public static ScenarioMasterData LoadScenarioMasterDataFromJson()
    {
        //Jsonファイルを読み込んでscenarioMasterDataに代入する
        return JsonUtility.FromJson<ScenarioMasterData>(JsonHelper.GetJsonFile("/", "scenario.json"));
    }
}

   