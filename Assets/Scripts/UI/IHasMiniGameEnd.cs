using System;
using UnityEngine;

public interface IHasMiniGameEnd
{
    event Action<bool> OnMiniGameEnd;
}
