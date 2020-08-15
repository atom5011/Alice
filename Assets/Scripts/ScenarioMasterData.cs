using System;
using System.Collections.Generic;


[Serializable]
public class ScenarioMasterData
{
    public List<ScenarioData> scenario = new List<ScenarioData>();

    [Serializable]
    public class ScenarioData
    {
        //Jsonのデータを受け取る(JsonとKeyと同じ変数名にする）
        public int scenarioNo;
        public string messageString;
        public string charaNoString;
        public string branchString;
        public string displayCharaString;

        //読み込んだDataを配列に置き換えて代入
        public string[] messages;
        public CHARA_NAME_TYPE[] charaTypes;
        public int[] branchs;
        public Dictionary<int, CHARA_NAME_TYPE[]> displayCharas; 
    }
}


