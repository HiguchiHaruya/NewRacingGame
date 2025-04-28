using UnityEngine;
/// <summary>ステージのデータだけ持たせてる</summary>
[CreateAssetMenu(menuName = "Stage/StageData")]
public class StageData : ScriptableObject
{
    [field: SerializeField] public string StageName { get; private set; } // memo field:を付けるとプロパティごとSerializeField出来る。すっきり
    [field: SerializeField] public Sprite StageView { get; private set; }
    [field: SerializeField] public string SceneName { get; private set; }
}
