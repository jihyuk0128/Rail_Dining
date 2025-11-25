using System.Collections;
using TMPro;
using UnityEngine;

public enum CustomerState
{
    Entering,        // 바로 들어오는 중
    SearchingSeat,   // 좌석을 살펴보며 잠깐 정지
    MovingToSeat,    // 좌석으로 이동 중
    WaitingForOrder, // 주문 대기 (플레이어가 주문 받아주길 기다림)
    WaitingForDrink, // 음료 대기 (30초 타이머 시작됨)
    Leaving          // 퇴장 중
}

public class Customer : MonoBehaviour, IInteractable
{
    [Header("이동속도설정")]
    public float moveSpeed = 2f; // 이동 속도
    [Header("상태 체크용")]
    public CustomerState state;
    public Seat targetSeat;
    public ItemData orderMenu;     // 주문 메뉴 
    public float waitTime; // 음료 대기 시간 
    public int customerid; // customerid
    public float searchTime;

    private Coroutine waitCoroutine;
    public Vector2 CurrentDirection { get; private set; } // 현재 이동 방향
    private Rigidbody2D rb;
    // 의자에 따라서 정렬용
    private YSort FrontYSort;
    private YSort BehindYSort;
    // 스파인 애니메이션
    private CustomerSpineController spineController;
    // 손님 주문 아이템 UI
    private CustomerOrderUI orderUI;

    public void Init(int id, int gender, float searchtime)
    {
        customerid = id;
        searchTime = searchtime;
        StartCoroutine(CustomerRoutine());
    }

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        FrontYSort = transform.Find("SpinePivot/CustomerFront")?.GetComponent<YSort>();
        BehindYSort = transform.Find("SpinePivot/CustomerBehind")?.GetComponent<YSort>();
        spineController = GetComponent<CustomerSpineController>();
        orderUI = GetComponentInChildren<CustomerOrderUI>();

        Managers.Network.OnOrderMenu += TakeOrder;
    }

    private IEnumerator CustomerRoutine() // 서버에서 
    {
        // 1. 등장
        state = CustomerState.Entering;
        Vector3 entryTarget = GameObject.Find("StartPoint").transform.position;
        yield return MoveToRoutine(entryTarget);

        // 2. 좌석찾는 시간
        state = CustomerState.SearchingSeat;
        yield return new WaitForSeconds(searchTime);

        // 3. 좌석으로 이동
        targetSeat = SeatManager.Instance.GetEmptySeat();
        if (targetSeat != null)
        {
            state = CustomerState.MovingToSeat;
            targetSeat.IsSeating = true;
            yield return MoveToSeatWithRoute(targetSeat);
        }
        else
        {
            yield return StartCoroutine(LeaveRoutine());
        }

    }

    private void SitDown()
    {
        state = CustomerState.WaitingForOrder;
        Vector3 curVec = targetSeat.GetSeatPosition();
        curVec.z = transform.position.z;
        transform.position = curVec;

        // ui + 애니메이션
        orderUI.ShowWaiting();
        spineController.isSiting = true;
        spineController.seatDirection = targetSeat.seatDirection;
        spineController.UpdateSpine(Vector2.zero);

        Managers.Network.TakeOrder(customerid);
    }

    public void TakeOrder(int id ,int ordermenu, float time)
    {
        Managers.MainThread.Enqueue(() =>
        {
            if (state != CustomerState.WaitingForOrder)
                return;

            UI_BasicScene uiScene = FindObjectOfType<UI_BasicScene>();
            if (uiScene == null)
            {
                Debug.LogWarning("UI_BasicScene을 찾을 수 없습니다!");
                return;
            }

            if (uiScene.GetOrderCount() > 8)
            {
                Debug.Log("주문이 가득 차서 손님이 주문하지 못했습니다!");
                return;
            }

            state = CustomerState.WaitingForDrink;
            orderMenu = GameManager.Instance.Getorder(ordermenu);
            waitTime = time;

            Debug.Log($"손님이 {orderMenu.name} 를 주문했습니다!");

            //uiScene.AddOrder(orderMenu); // 주문 UI 추가
            orderUI.ShowOrder(orderMenu, 0, 1);
            SoundManager.Instance.PlaySFX("OrderAccept_SFX");
            waitCoroutine = StartCoroutine(WaitForDrink());
        });
    }

    private IEnumerator WaitForDrink() // 음료대기 
    {
        yield return new WaitForSeconds(waitTime);
        if (state == CustomerState.WaitingForDrink)
        {
            Debug.Log("손님이 기다리다 떠났습니다.");
            UI_BasicScene uiScene = FindObjectOfType<UI_BasicScene>();
            uiScene.RemoveOrder(orderMenu); // 주문 제거
            yield return StartCoroutine(LeaveRoutine());
        }
    }
    
   
    public void ServeDrink(GameObject player) // 주문완료
    {
        if (state == CustomerState.WaitingForDrink ) 
        {
            //아이템이 없으면 실패 처리
            if (!Managers.NewInventory.HasItem(orderMenu.id))
            {
                Debug.Log("플레이어가 올바른 음료를 가지고 있지 않습니다!");
                //StartCoroutine(ShowOrderHint());
                return;
            }
            Managers.NewInventory.RemoveItem(orderMenu.id);
            Debug.Log("손님이 음료를 받고 돈을 지불합니다.");
            //MoneyManager.Instance.AddMoney(price);
            //인벤토리에서 현재 가리키고 있는 아이템을 지우기?
            Debug.Log($"{customerid}번 손님 주문완료");
            // 손님 반응 애니메이션 재생
            spineController.isSuccess = true; // 주문 완료시에 반응 애니메이션 재생용
            spineController.UpdateSpine(Vector2.zero);
            Managers.Network.OrderSuccess(customerid);
        }
    }

    private IEnumerator ShowOrderHint()
    {
        var basicUI = FindObjectOfType<UI_BasicScene>();
        basicUI?.SetRecipeVisible(true);
        yield return new WaitForSeconds(2f);
        basicUI?.SetRecipeVisible(false);
    }

    private IEnumerator LeaveRoutine()
    {
        state = CustomerState.Leaving;
        orderUI.HideOrder();
        float leaveTime = 3.0f;
        yield return new WaitForSeconds(leaveTime);

        if (targetSeat != null) targetSeat.IsSeating = false;

        FrontYSort.isSeat = false;
        BehindYSort.isSeat = false;

        // 좌석의 퇴장 루트를 따라서 이동
        if (targetSeat != null && targetSeat.exitRoutePoints != null)
        {
            foreach (Transform point in targetSeat.exitRoutePoints)
            {
                yield return MoveToRoutine(point.position);
            }
        }

        Vector3 exitTarget = GameObject.Find("ExitPoint").transform.position;
        yield return MoveToRoutine(exitTarget);

        Destroy(gameObject);
    }
     
    // 웨이포인트 따라서 좌석으로 이동
    private IEnumerator MoveToSeatWithRoute(Seat seat)
    {
        // 경유지가 설정되어 있다면 순서대로 이동
        if (seat.entryRoutePoints != null && seat.entryRoutePoints.Length > 0)
        {
            foreach (Transform point in seat.entryRoutePoints)
            {
                yield return MoveToRoutine(point.position);
            }
        }

        // SeatYSort 가져오기 (Seat이 직접 or 부모 소파)
        SeatYSort seatYSort = targetSeat.seatYSort;
        FrontYSort.isSeat = true;
        FrontYSort.seatYSort = seatYSort;
        BehindYSort.isSeat = true;
        BehindYSort.seatYSort = seatYSort;

        // 마지막으로 좌석 위치로 이동
        yield return MoveToRoutine(seat.GetSeatPosition());
        SitDown();
    }

    // 이동 루틴
    private IEnumerator MoveToRoutine(Vector3 pos)
    {
        pos.z = transform.position.z; // z 고정
        Vector2 dir = (pos - transform.position).normalized;
        spineController.UpdateSpine(dir);
        while (Vector3.Distance(transform.position, pos) > 0.1f)
        {
            Vector2 newPos = Vector2.MoveTowards(rb.position, pos, moveSpeed * Time.fixedDeltaTime);
            rb.MovePosition(newPos);
            yield return new WaitForFixedUpdate();
        }
    }

    // IInteractable 구현
    public void Interact(GameObject player)
    {
        if(state == CustomerState.WaitingForDrink)
        {
            ServeDrink(player); // ServeDrink(menu); player.currentItem 이런거? 플레이어가 현재 가리키는 아이템 데이터 넘기기
        }
    }

    public Vector3 GetPosition() => transform.position;

    public int GetPriority(GameObject player)
    {
        var playerState = player.GetComponent<PlayerState>();
        if (playerState != null && playerState.HasCocktail) // 플레이어가 칵테일을 가지고 있을 때
        {
            return 0; // 우선순위 높음
        }
        return 1; // 우선순위 낮음
    }

    private void RemoveItemFromInventory(int itemId)
    {
        var inv = Managers.Inventory.InventorySlots;
        foreach (var slot in inv)
        {
            if (slot.Item != null && slot.Item.id == itemId)
            {
                slot.Amount--;
                if (slot.Amount <= 0) slot.Clear();
                Managers.Inventory.RefreshAllUI();
                Debug.Log($"인벤토리에서 {itemId} 제거 완료"); 
                return;
            }
        }
    }

    public void Leave()
    {
        // 주문내역 ui 삭제
        UI_BasicScene uiScene = FindObjectOfType<UI_BasicScene>();
        uiScene.RemoveOrder(orderMenu); // 주문 제거
        CustomerSpawner spawner = transform.parent?.GetComponent<CustomerSpawner>();
        if (spawner != null)
        {
            spawner.AddSuccessCount();
        }
        // 떠나는 사운드 재생 
        SoundManager.Instance.PlaySFX("OrderDelivery_SFX");
        // 대기루틴 정지
        if (waitCoroutine != null) StopCoroutine(waitCoroutine);
        // 떠나기 루틴
        StartCoroutine(LeaveRoutine());
    }

    private void OnDestroy()
    {
        Managers.Network.OnOrderMenu -= TakeOrder;
    }
}
