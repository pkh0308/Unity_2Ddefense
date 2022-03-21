using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Tilemaps;
using UnityEngine.EventSystems;

public class OperDrag : MonoBehaviour, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    GameObject dragImg;
    Image dragImgLogic;
    public Canvas canvas;
    public GameManager gameManager;
    Tilemap tileMap;

    public GameObject upperGnd;
    public GameObject lowerGnd;
    Operator.Type type;
    public ObjectManager objManager;

    Vector3 worldPos;
    Vector3 roundedPos;
    Vector3 screenPos;
    Vector3Int cellPos;

    public GameObject oper;
    Operator operLogic;
    int cost;
    public GameObject activePanel;
    public Text costText;
    GameObject attackRange;

    public void Initialize()
    {
        string name = oper.name.ToLower();
        cost = objManager.GetOne(name).GetComponent<Operator>().SpawnCost;
        costText.text = cost.ToString();

        type = oper.GetComponent<Operator>().type;
        operLogic = oper.GetComponent<Operator>();
    }

    // UI 갱신
    // 배치 코스트가 모자랄 경우 비활성화, 충분할 경우 활성화
    void Update()
    {
        if(gameManager.CanOperSpawn(cost))
        {
            activePanel.SetActive(false);
        }
        else
        {
            activePanel.SetActive(true);
        }
    }

    public void OnBeginDrag(PointerEventData data)
    {
        // 일시정지 중이거나 코스트가 모자랄 경우 미동작
        if (gameManager.isPaused || !gameManager.CanOperSpawn(cost))
            return;

        // 드래그 중 감속, 해당 오퍼 이미지 생성
        gameManager.TimeControl(0.3f);
        switch(operLogic.operID)
        {
            case 10001:
                dragImg = objManager.MakeObj("swordmanIcon");
                break;
            case 20001:
                dragImg = objManager.MakeObj("archerIcon");
                break;
            case 30001:
                dragImg = objManager.MakeObj("casterIcon");
                break;
            case 40001:
                dragImg = objManager.MakeObj("defenderIcon");
                break;
        }
        dragImgLogic = dragImg.GetComponent<Image>();
        dragImgLogic.color = new Color(1, 1, 1, 0.4f);
        dragImgLogic.rectTransform.position = data.position;

        // 오퍼 타입에 따라 타일셋 활성화, 원거리일 경우 공격범위 표시 생성
        switch (type)
        {
            case Operator.Type.Melee:
                lowerGnd.SetActive(true);
                tileMap = lowerGnd.GetComponent<Tilemap>();
                break;
            case Operator.Type.Range:
                upperGnd.SetActive(true);
                tileMap = upperGnd.GetComponent<Tilemap>();
                attackRange = objManager.MakeObj("attackRange");
                float operRange = operLogic.attackRange * 1.8f;
                attackRange.transform.localScale = new Vector3(operRange, operRange, 1);
                break;
        }
    }

    public void OnDrag(PointerEventData data)
    {
        if (gameManager.isPaused || !gameManager.CanOperSpawn(cost))
            return;

        if (dragImg != null)
        {
            worldPos = Camera.main.ScreenToWorldPoint(data.position);
            float x = Mathf.Round(worldPos.x);
            float y = Mathf.Round(worldPos.y);
            roundedPos = new Vector3(x, y, 0);
            screenPos = Camera.main.WorldToScreenPoint(roundedPos);
            cellPos = tileMap.WorldToCell(roundedPos);
            
            if(tileMap.GetTile(cellPos) != null)
            {
                dragImg.transform.position = screenPos;
                if(operLogic.type == Operator.Type.Range)
                {
                    attackRange.transform.position = roundedPos;
                    if (!attackRange.activeSelf)
                        attackRange.SetActive(true);
                }
            }
            else
            {
                dragImg.transform.position = data.position;
                if (operLogic.type == Operator.Type.Range)
                {
                    if (attackRange.activeSelf)
                        attackRange.SetActive(false);
                }
            }
        }
    }

    public void OnEndDrag(PointerEventData data)
    {
        if (gameManager.isPaused || !gameManager.CanOperSpawn(cost))
            return;

        if (dragImg != null)
        {
            gameManager.TimeControl(0.0f);
            Drop();
            dragImg.SetActive(false);
            upperGnd.SetActive(false);
            lowerGnd.SetActive(false);
            if (operLogic.type == Operator.Type.Range)
                attackRange.SetActive(false);
        }
    }

    void Drop()
    {
        if(tileMap.GetTile(cellPos) == null)
            return;

        switch(operLogic.operID)
        {
            case 10001:
                GameObject instantSwordman = objManager.MakeObj("swordman");
                instantSwordman.transform.position = roundedPos;
                Operator swordmanLogic = instantSwordman.GetComponent<Operator>();
                swordmanLogic.Initialize();
                break;
            case 20001:
                GameObject instantArcher = objManager.MakeObj("archer");
                instantArcher.transform.position = roundedPos;
                Operator archerLogic = instantArcher.GetComponent<Operator>();
                archerLogic.Initialize();
                break;
            case 30001:
                GameObject instantCaster = objManager.MakeObj("caster");
                instantCaster.transform.position = roundedPos;
                Operator casterLogic = instantCaster.GetComponent<Operator>();
                casterLogic.Initialize();
                break;
            case 40001:
                GameObject instantDefender = objManager.MakeObj("defender");
                instantDefender.transform.position = roundedPos;
                Operator defenderLogic = instantDefender.GetComponent<Operator>();
                defenderLogic.Initialize();
                break;
        }
        gameManager.CostSpend(cost);
        gameManager.OperCount(1);
        gameManager.soundManager.PlaySfx(5);

        if(upperGnd.activeSelf)
        {
            TilemapController upperTile = upperGnd.GetComponent<TilemapController>();
            upperTile.DestroyDot(roundedPos);
        }
        if(lowerGnd.activeSelf)
        {
            TilemapController lowerTile = lowerGnd.GetComponent<TilemapController>();
            lowerTile.DestroyDot(roundedPos);
        }
    }
}