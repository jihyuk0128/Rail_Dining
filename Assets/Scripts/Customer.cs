using System.Collections;
using UnityEngine;
using UnityEngine.Playables;

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
    public CustomerState state;
    public Seat targetSeat;
    public string orderMenu;     // 주문 메뉴 
    public float waitTime = 30f; // 음료 대기 시간
    public float moveSpeed = 2f; // 이동 속도

    private Coroutine waitCoroutine;

    void Start()
    {
        StartCoroutine(CustomerRoutine());
    }

    private IEnumerator CustomerRoutine()
    {
        // 1. 등장
        state = CustomerState.Entering;

        Vector3 entryTarget = GameObject.Find("StartPoint").transform.position;
        yield return MoveToRoutine(entryTarget);

        // 2. SearchingSeat
        state = CustomerState.SearchingSeat;
        float searchTime = Random.Range(1f, 2f);
        yield return new WaitForSeconds(searchTime);

        // 3. 좌석으로 이동
        targetSeat = SeatManager.Instance.GetEmptySeat();
        if (targetSeat != null)
        {
            state = CustomerState.MovingToSeat;
            yield return MoveToRoutine(targetSeat.transform.position);
            SitDown();
        }
        else
        {
            yield return StartCoroutine(LeaveRoutine());
        }
    }

    private void SitDown()
    {
        state = CustomerState.WaitingForOrder;
        targetSeat.IsSeating = true;
        transform.position = targetSeat.GetSeatPosition();
        Debug.Log($"손님이 {targetSeat.SeatId} 번 좌석에 앉았습니다.");
    }

    public void TakeOrder()
    {
        if (state == CustomerState.WaitingForOrder)
        {
            state = CustomerState.WaitingForDrink;
            orderMenu = "임시값(메뉴아이디 들어갈 예정)";
            Debug.Log($"손님이 {orderMenu} 를 주문했습니다!");
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

    public void ServeDrink(string menu)
    {
        if (state == CustomerState.WaitingForDrink && menu == orderMenu)
        {
            Debug.Log("손님이 음료를 받고 돈을 지불합니다.");
            if (waitCoroutine != null) StopCoroutine(waitCoroutine);
            StartCoroutine(LeaveRoutine());
        }
    }

    private IEnumerator LeaveRoutine()
    {
        state = CustomerState.Leaving;
        if (targetSeat != null) targetSeat.IsSeating = false;

        Vector3 exitTarget = GameObject.Find("ExitPoint").transform.position;
        yield return MoveToRoutine(exitTarget);

        Destroy(gameObject);
    }

    private IEnumerator MoveToRoutine(Vector3 pos)
    {
        while (Vector3.Distance(transform.position, pos) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(transform.position, pos, moveSpeed * Time.deltaTime);
            yield return null;
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
            ServeDrink("임시값(메뉴아이디 들어갈 예정)"); // ServeDrink(menu);
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
