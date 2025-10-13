using System.Collections;
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
    public float waitTime = 30f; // 음료 대기 시간 

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

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        FrontYSort = transform.Find("SpinePivot/CustomerFront")?.GetComponent<YSort>();
        BehindYSort = transform.Find("SpinePivot/CustomerBehind")?.GetComponent<YSort>();
        spineController = GetComponent<CustomerSpineController>();
        orderUI = GetComponentInChildren<CustomerOrderUI>();

        StartCoroutine(CustomerRoutine());
    }

    private IEnumerator CustomerRoutine() // 서버에서 
    {
        // 1. 등장
        state = CustomerState.Entering;
        Vector3 entryTarget = GameObject.Find("StartPoint").transform.position;
        yield return MoveToRoutine(entryTarget);

        // 2. 좌석찾는 시간
        state = CustomerState.SearchingSeat;
        float searchTime = Random.Range(1f, 2f);
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
    }

    public void TakeOrder()
    {
        if (state == CustomerState.WaitingForOrder)
        {
            state = CustomerState.WaitingForDrink;
            int ItemID = 1;
            orderMenu = Managers.Data.ItemDict[ItemID];
            Debug.Log($"손님이 {orderMenu.name} 를 주문했습니다!");
            orderUI.ShowOrder(orderMenu, 0, 1);
            waitCoroutine = StartCoroutine(WaitForDrink()); 
        }
    }

    
     private IEnumerator WaitForDrink()
    {
        yield return new WaitForSeconds(waitTime);
        if (state == CustomerState.WaitingForDrink)
        {
            Debug.Log("손님이 기다리다 떠났습니다.");
            yield return StartCoroutine(LeaveRoutine());
        }
    }
    

    public void ServeDrink(ItemData menu)
    {
        if (state == CustomerState.WaitingForDrink && menu.id == orderMenu.id)
        {

            Debug.Log("손님이 음료를 받고 돈을 지불합니다.");
            //MoneyManager.Instance.AddMoney(price);
            //인벤토리에서 현재 가리키고 있는 아이템을 지우기?
            
            CustomerSpawner spawner = transform.parent?.GetComponent<CustomerSpawner>();
            if (spawner != null)
            {
                spawner.AddSuccessCount();
            }

            if (waitCoroutine != null) StopCoroutine(waitCoroutine);
            StartCoroutine(LeaveRoutine());
        }
    }

    private IEnumerator LeaveRoutine()
    {
        state = CustomerState.Leaving;
        orderUI.HideOrder();
        float leaveTime = Random.Range(2f, 3f);
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
        if (state == CustomerState.WaitingForOrder)
        {
            TakeOrder();
        }
        else if(state == CustomerState.WaitingForDrink)
        {
            string menu = player.GetComponent<PlayerState>().CocktailName;           
            ServeDrink(orderMenu); // ServeDrink(menu); player.currentItem 이런거? 플레이어가 현재 가리키는 아이템 데이터 넘기기
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
}
