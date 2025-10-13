using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    public GameObject localPlayerPrefab;   // 로컬용
    public GameObject remotePlayerPrefab;  // 원격용

    private Dictionary<string, GameObject> players = new();

    public void UpdatePlayer(string name, Vector3 pos)
    {
        // 자기 자신은 이미 생성되어 있으므로 return
        if (name == Managers.Network.player.Username)
            return;

        if (!players.ContainsKey(name))
        {
            // 원격 플레이어 프리팹 생성
            GameObject newPlayer = Instantiate(remotePlayerPrefab, pos, Quaternion.identity);
            newPlayer.name = name;

            // PlayerNetworkSync 추가
            if (newPlayer.GetComponent<PlayerNetworkSync>() == null)
                newPlayer.AddComponent<PlayerNetworkSync>();

            players.Add(name, newPlayer);
        }

        var playerObj = players[name];
        var sync = playerObj.GetComponent<PlayerNetworkSync>();

        // ?? 새 위치를 target으로 설정
        sync.SetTarget(pos, 0f);  // targetRotZ는 현재 안 쓰니까 0으로 둠

        // ?? 부드러운 방향 계산 (직전 위치 대비)
        Vector3 smoothDir = pos - sync.lastPos;
        sync.lastPos = pos;

        // 너무 짧은 변화는 무시 (미세한 흔들림 방지)
        if (smoothDir.magnitude < 0.005f)
            return;

        var spine = playerObj.GetComponent<PlayerSpineController>();
        if (spine != null)
        {
            // 부드러운 방향 전달
            spine.UpdateSpine(new Vector2(smoothDir.x, smoothDir.y), false);
        }
    }

    public void UpdatePlayerSpine(string name, Vector2 moveInput, bool isRunning, bool isFalling)
    {
        if (!players.ContainsKey(name))
        {
            Debug.LogWarning($"[PlayerManager] {name} Spine 업데이트 실패: 존재하지 않음");
            return;
        }

        var playerObj = players[name];
        if (playerObj == null)
        {
            Debug.LogWarning($"[PlayerManager] {name} 오브젝트가 null임");
            return;
        }

        var spine = playerObj.GetComponent<PlayerSpineController>();
        if (spine == null)
        {
            Debug.LogWarning($"[PlayerManager] {name} SpineController 없음");
            return;
        }

        if (isFalling)
        {
            spine.PlayFallAnimation();
            return;
        }

        spine.UpdateSpine(moveInput, isRunning);
    }
}