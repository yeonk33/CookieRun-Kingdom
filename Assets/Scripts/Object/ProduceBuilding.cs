using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class ProduceBuilding : MonoBehaviour
{
    public string InstanceId { get; private set; } // 같은 건물 여러개일 때 구분용
    public int Level { get; private set; } // 건물 레벨
    private BuildingData _data; // 건물 SO
    public List<ProduceInfo> ProduceList { get; private set; } // 생산 대기열
    private int _curIndex = 0; // 현재 생산 중인 아이템 인덱스

    /// <summary>
    /// JSON에서 데이터 읽어와야함 (instanceId필요)
    /// </summary>
    public void SetBuildingData(BuildingData data, int lv, string instanceId = null)
	{
        InstanceId = instanceId ?? Guid.NewGuid().ToString(); // 인스턴스 ID가 없으면 새로 생성
        Level = lv;
		_data = data;
		this.GetOrAddComponent<SpriteRenderer>().sprite = data.icon;
        ProduceList = new List<ProduceInfo>();
	}

	private void OnMouseUp() // Collider 필요
	{
		ProductionPanel.Instance.OpenPanel(_data, Level, this);
	}

    public void UpdateStatus() // 종료시간 기준으로 생산 완료 여부 업데이트 (ex. 껐다켰을때?)
    {
        var building = ProduceManager.GetBuilding(InstanceId);
        
        foreach (var item in building.ProduceList)
        {
            if (Utils.GetRemainTime(item.endTime) == 0)
            {
                item.isComplete = true; // 생산 완료
            }
            else
            {
                item.isComplete = false; // 아직 생산 중
            }
        }
    }

    public List<ProduceInfo> PickupItems() // 생산 완료된 것들 수거
    {
        var returnList = new List<ProduceInfo>();

        for (int i = 0; i < ProduceList.Count; i++)
        {
            var item = ProduceList[i];
            if (item.isComplete) // 생산 완료
            {
                var production = ProductionDatabase.Get(item.productionId);
                //Inventory.Add(production.outputItemId, production.outputItemAmout); // 아이템 추가, 1개씩 추가한다고 가정
                //Debug.Log($"수거 완료: {production.displayName} x {production.outputItemAmout}");
                returnList.Add(item); // 수거된 아이템 리스트에 추가
                ProduceList.Remove(item);
                i--;
                // @@@@@ UI 수거 알림

            }
        }

        _curIndex = 0; // 수거 후 인덱스 초기화
        if (ProduceList.Count == 0) ProduceManager.UnregistBuilding(InstanceId); // 전부 생산 완료면 건물 Unregist
        
        return returnList; // 수거된 아이템 리스트 반환
    }

    public void RegistProduce(string productionId)
    {
        DateTime startTime = ProduceList.Count > 0 ? ProduceList.Last().endTime : DateTime.UtcNow; // 마지막 생산 종료 시간 또는 현재 시간

        var production = ProductionDatabase.Get(productionId);
        ProduceList.Add(new ProduceInfo
        {
            productionId = productionId,
            endTime = startTime.AddSeconds(production.timeCost), // 앞 생산품 종료 시간 + 생산 시간
            isComplete = false,
            count = production.outputItemAmout // 생산될 아이템 수량
        });
    }


    public void StartProduce(string productionid) // 매니저에서 명령 내리는 함수
	{
        RegistProduce(productionid); // 생산 대기열에 등록

        if (ProduceList.Count == 1) StartCoroutine(ProduceCoroutine(ProduceList[_curIndex])); // 첫 등록이면 생산 시작
    }

    private IEnumerator ProduceCoroutine(ProduceInfo info)
	{
        Debug.Log($"생산 시작: {info.productionId}");
        var remainT = Utils.GetRemainTime(info.endTime);
		yield return new WaitForSeconds(remainT);
        Debug.Log($"생산 완료: {info.productionId}");
        info.isComplete = true; // 생산 완료 표시

        if (ProduceList.Count > _curIndex+1) // 다음 생산이 있다면
        {
            _curIndex++; // 현재 인덱스 증가
            StartCoroutine(ProduceCoroutine(ProduceList[_curIndex])); // 다음 생산 시작
        }
    }
}


[System.Serializable]
public class ProduceInfo
{
    [ProductionIdAttribute]
    public string productionId;
    public DateTime endTime; // 생산 종료 시간
    public bool isComplete; // 생산 완료 여부
    public int count; // 생산될 아이템 수량
}
