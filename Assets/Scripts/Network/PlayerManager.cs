using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    //public GameObject localPlayerPrefab;   // 로컬용
    [SerializeField] private GameObject malePrefab;
    [SerializeField] private GameObject femalePrefab;

    private Dictionary<string, GameObject> players = new();

    private GameObject LoadPrefab(string prefabName)
    {
        GameObject prefab = Resources.Load<GameObject>($"Characters/{prefabName}");
        if (prefab == null)
        {
            Debug.LogError($"[PlayerManager] 프리팹을 찾을 수 없음: {prefabName}");
        }
        return prefab;
    }


    public void UpdatePlayer(string name, Vector3 pos)
    {
        // 자기 자신은 이미 생성되어 있으므로 return
        if (name == Managers.Network.player.Username)
            return;

        if (!players.ContainsKey(name))
        {
            GameObject prefab;

            if (Managers.Network.player.IsHost == false)
                prefab = malePrefab;     
            else
                prefab = femalePrefab;

            // 원격 플레이어 프리팹 생성
            GameObject newPlayer = Instantiate(prefab, pos, Quaternion.identity);
            newPlayer.name = name;

            // PlayerNetworkSync 추가
            if (newPlayer.GetComponent<PlayerNetworkSync>() == null)
                newPlayer.AddComponent<PlayerNetworkSync>();

            players.Add(name, newPlayer);
        }

        var playerObj = players[name];
        var sync = playerObj.GetComponent<PlayerNetworkSync>();

        // 새 위치를 target으로 설정
        sync.SetTarget(pos, 0f);  //

        // 부드러운 방향 계산 (직전 위치 대비)
        Vector3 smoothDir = pos - sync.lastPos;
        sync.lastPos = pos;

        // 너무 짧은 변화는 무시 (미세한 흔들림 방지)
        if (smoothDir.magnitude < 0.010f)
            return;

        var spine = playerObj.GetComponent<PlayerSpineController>();
        if (spine != null)
        {
            // 부드러운 방향 전달
            spine.UpdateSpine(new Vector2(smoothDir.x, smoothDir.y), false);
        }
    }

    public void UpdatePlayer(string name, Vector3 pos, Vector2 moveDir, bool isRunning, bool isFalling)
    {
        // 자기 자신은 이미 있으므로 무시
        if (name == Managers.Network.player.Username)
            return;

        // --- 플레이어 생성 ---
        if (!players.ContainsKey(name))
        {
            GameObject prefab;

            Debug.LogWarning($"상대 캐릭터 설정중 {Managers.Network.player.IsHost}가 true라면 상대는 여캐.");
            if (Managers.Network.player.IsHost == false)
                prefab = malePrefab;
            else
                prefab = femalePrefab;

            GameObject newPlayer = Instantiate(prefab, pos, Quaternion.identity);
            newPlayer.name = name;

            // 네트워크 동기화용 컴포넌트 추가
            if (newPlayer.GetComponent<PlayerNetworkSync>() == null)
                newPlayer.AddComponent<PlayerNetworkSync>();

            players.Add(name, newPlayer);
        }

        // --- 참조 캐싱 ---
        var playerObj = players[name];
        var sync = playerObj.GetComponent<PlayerNetworkSync>();

        // --- 이동 보간 대상 설정 ---
        sync.SetTarget(pos, 0f); // targetPos, rotZ
        sync.lastPos = pos;

        // --- Spine 상태 업데이트 ---
        var spine = playerObj.GetComponent<PlayerSpineController>();
        if (spine == null)
            return;

        if (isFalling)
        {
            spine.PlayFallAnimation();
            return;
        }

        // moveDir이 너무 작으면 무시
        if (moveDir.sqrMagnitude < 0.001f)
        {
            spine.UpdateSpine(Vector2.zero, false);
            return;
        }

        // 정규화해서 전달
        moveDir.Normalize();
        spine.UpdateSpine(moveDir, isRunning);
    }
}