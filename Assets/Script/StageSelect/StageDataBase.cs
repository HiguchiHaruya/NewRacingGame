using System.Collections.Generic;
using UnityEngine;
using UniRx;
///<summary> ステージ取得専用 </summary>
[CreateAssetMenu(menuName = ("Stage/StageDataBase"))]
public class StageDataBase : ScriptableObject
{
    [SerializeField] private List<StageData> _stages;
    public IReadOnlyCollection<StageData> StagesList => _stages;
    public StageData GetStages(int index)
    {
        if (index < 0 || index >= _stages.Count)
        {
            Debug.Log("指定された数値のステージはありません");
            return null;
        }
        return _stages[index];
    }
}

