using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine;
using Water;

public class StageDoor : InteractionObj
{
    private SpriteRenderer spr;

    private WorldIcon icon;

    //public FakeSpriteOutline fsOut;

    public StageDataSO nextStageData;

    //[SerializeField] private bool notExistMap; //test

    public DoorDirType dirType; //door dir
    public Transform playerSpawnPos;

    public GameObject detectorObj;
    public Light2D doorLight;

    private bool isOpen; //문으로 입장할 수 있는 상태가 되어서 상호작용 키를 눌렀을 때 true로
    private bool isExitDoor; //이 문이 입구였는가
    public bool IsExitDoor { set => isExitDoor = value; }

    public bool IsBlindState => StateManager.Instance.stateCountDict[StateAbnormality.Blind] > 0;

    private void Awake()
    {
        spr = GetComponent<SpriteRenderer>();

        float rz = 0f;
        switch(dirType)
        {
            case DoorDirType.FRONT:
                rz = -180f;
                break;
            case DoorDirType.RIGHT:
                rz = 90f;
                break;
            case DoorDirType.LEFT:
                rz = -90f;
                break;
            case DoorDirType.BACK:
                rz = 0f;
                break;
        }
        doorLight.transform.rotation = Quaternion.Euler(0, 0, rz);
    }

    public override void Interaction()
    {
        if(StageManager.Instance.IsStageClear)
        {
            if(nextStageData.areaType == AreaType.BOSS)
            {
                UIManager.Instance.RequestSystemMsg("아직 여긴 못 간다니까");
                return;
            }

            if (!isOpen && !isExitDoor) //문을 열었거나 입구로 쓴 문이면 상호작용 아예 안되게
            {
                isOpen = true;
                StageManager.Instance.PassDir = dirType;
                UIManager.Instance.StartLoading(() => StageManager.Instance.NextStage(nextStageData.stageID), () => EventManager.TriggerEvent("StartNextStage"));
            }
        }
        else
        {
            if (StageManager.Instance.CurrentAreaType == AreaType.MONSTER)
                UIManager.Instance.RequestSystemMsg("몬스터가 남아있을 때는 다음 지역으로 입장할 수 없습니다.");
            else
                UIManager.Instance.RequestSystemMsg("아직은 지나갈 수 없습니다.");
        }
    }

    public void Open()
    {
        if (isExitDoor || !gameObject.activeSelf) return;

        spr.sprite = StageManager.Instance.doorSprDic[dirType.ToString() + "Open"];
        isOpen = false;
        objName = IsBlindState ? "???" : Global.AreaTypeToString(nextStageData.areaType);

        detectorObj.SetActive(true);
        doorLight.gameObject.SetActive(true);
        doorLight.intensity = 0.15f;
        doorLight.DOIntensity(0.75f, 0.5f);
    }

    public void Close()
    {
        if (isExitDoor || !gameObject.activeSelf) return;

        spr.sprite = StageManager.Instance.doorSprDic[dirType.ToString() + "Close"];
        doorLight.gameObject.SetActive(false);
        detectorObj.SetActive(false);
    }

    public void Pass()
    {
        spr.sprite = StageManager.Instance.doorSprDic[dirType.ToString() + "Exit"];
        isExitDoor = true;
        doorLight.gameObject.SetActive(false);
        detectorObj.SetActive(false);
    }

    public override void SetInteractionUI(bool on)
    {
        if (StageManager.Instance.IsStageClear && !isExitDoor)
        {
            base.SetInteractionUI(on);

            if (nextStageData.areaType == AreaType.MONSTER && !IsBlindState)  //담 스테이지가 몬스터 맵이면 종족 아이콘 띄움
            {
                if (on)
                {
                    if (!icon)
                    {
                        icon = PoolManager.GetItem<WorldIcon>("MobSpeciesIcon");
                        icon.Set(itrUI.GetComponent<RectTransform>(), nextStageData.enemySpeciesArea);
                    }
                }
                else
                {
                    if (icon)
                    {
                        icon.gameObject.SetActive(false);
                        icon = null;
                    }
                }
            }
            /*if(fsOut)
               fsOut.gameObject.SetActive(on); */
        }
    }

    public override void SetUI(bool on)
    {
        if (StageManager.Instance.IsStageClear && !isExitDoor)
        {
            base.SetUI(on);
        }
    }
}
